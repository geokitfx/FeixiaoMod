using System.Runtime.CompilerServices;
using FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers.Models;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace FeixiaoMod.FeixiaoModCode.Patches;

#region Helper Classes

/// <summary>
/// Handles gold payment evaluation and exchange rates for enchanted cards.
/// </summary>
public static class GoldSpendHelper
{
    public const int GoldPerEnergy = 3;

    public static bool CanPayWithGold(CardModel? card, int energyCost, out int goldRequired)
    {
        goldRequired = 0;
        
        // 1. Must possess the SkullGoldEnchantment
        if (card?.Enchantment is not SkullGoldEnchant) return false;

        // 2. Calculate required gold based on exchange rate
        var energy = Math.Max(0, energyCost);
        goldRequired = energy * GoldPerEnergy;

        // 3. If the card costs 0 energy (and thus 0 gold), bypass gold interaction entirely
        if (goldRequired <= 0) return false;

        // 4. Verify owner has sufficient gold in their wallet
        var currentGold = card.Owner?.Gold ?? 0;
        return currentGold >= goldRequired;
    }
}

/// <summary>
/// Attaches temporary gold cost metadata and play execution state to card instances.
/// </summary>
public static class GoldSpendFields
{
    private class IntBox
    {
        public int Value;
        public bool IsAutoPlay;
    }
    
    private static readonly ConditionalWeakTable<CardModel, IntBox> Table = new();

    public static int Get(CardModel card)
    {
        return Table.TryGetValue(card, out var box) ? box.Value : 0;
    }

    public static void Set(CardModel card, int value)
    {
        Table.GetOrCreateValue(card).Value = value;
    }

    public static bool IsAutoPlay(CardModel card)
    {
        return Table.TryGetValue(card, out var box) && box.IsAutoPlay;
    }

    public static void SetAutoPlay(CardModel card, bool isAutoPlay)
    {
        Table.GetOrCreateValue(card).IsAutoPlay = isAutoPlay;
    }
}

public static class FeixiaoEnums
{
    // Custom enum value to represent Gold cost state for card frame coloring
    public const CardCostColor CostColorGold = (CardCostColor)100;
}

#endregion

#region Harmony Patches

// 1. Allows cards to bypass the "Not Enough Energy" playability restriction if player has sufficient gold
[HarmonyPatch(
    typeof(CardModel), 
    nameof(CardModel.CanPlay),
    [typeof(UnplayableReason), typeof(AbstractModel)],
    [ArgumentType.Out, ArgumentType.Out]
)]
internal class CardModelCanPlayGoldPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref UnplayableReason reason, ref bool __result)
    {
        if (!reason.HasFlag(UnplayableReason.EnergyCostTooHigh)) return;

        var energyCost = __instance.EnergyCost.GetWithModifiers(CostModifiers.All);

        if (!GoldSpendHelper.CanPayWithGold(__instance, energyCost, out _)) return;
        
        // Remove energy restriction flag and clear unplayable state
        reason &= ~UnplayableReason.EnergyCostTooHigh;
        __result = reason is UnplayableReason.None;
    }
}

// 2. Intercepts energy spending: Tricks base method into draining 0 energy and rewrites captured X-cost state
[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendEnergy))]
internal class CardModelSpendEnergyGoldPatch
{
    [HarmonyPrefix]
    private static void Prefix(CardModel __instance, ref int amount, out int __state)
    {
        __state = -999; 

        var actualEnergy = __instance.EnergyCost.CostsX
            ? __instance.Owner.PlayerCombatState?.Energy ?? 0
            : amount;

        if (GoldSpendHelper.CanPayWithGold(__instance, actualEnergy, out var goldRequired))
        {
            GoldSpendFields.Set(__instance, goldRequired);
            __state = actualEnergy; 
            amount = 0; 
        }
        else
        {
            GoldSpendFields.Set(__instance, 0);
        }
    }

    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref int amount, int __state)
    {
        if (__state is -999) return;

        amount = __state;

        if (__instance.EnergyCost.CostsX)
        {
            __instance.EnergyCost.CapturedXValue = __state;
        }
    }
}

// 3a. Deducts gold on the VERY FIRST cardplay in a series (flags auto-plays without deducting gold)
[HarmonyPatch(typeof(Hook), nameof(Hook.BeforeCardPlayed))]
internal class HookBeforeCardPlayedGoldPatch
{
    [HarmonyPostfix]
    private static void Postfix(ICombatState combatState, CardPlay cardPlay, ref Task __result)
    {
        __result = DeductGoldOnFirstPlay(cardPlay, __result);
    }

    private static async Task DeductGoldOnFirstPlay(CardPlay cardPlay, Task originalTask)
    {
        await originalTask;

        // If card is auto-played, flag it so EnchantPlayCount suppresses actual extra plays
        if (cardPlay.IsAutoPlay)
        {
            GoldSpendFields.SetAutoPlay(cardPlay.Card, true);
            return;
        }

        bool isFirstPlay = cardPlay.IsFirstInSeries || cardPlay.PlayIndex == 0;

        if (isFirstPlay)
        {
            var card = cardPlay.Card;
            var goldToSpend = GoldSpendFields.Get(card);

            // Replaced cardPlay.Player with card.Owner which is universally available on CardModel
            if (goldToSpend > 0 && card.CombatState is not null && card.Owner is not null)
            {
                await PlayerCmd.LoseGold(goldToSpend, card.Owner);
            }
        }
    }
}

// 3b. Cleans up temporary gold metadata and resets auto-play status after play finishes
[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
internal class CardModelOnPlayWrapperGoldPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        __result = CleanupGoldFields(__instance, __result);
    }

    private static async Task CleanupGoldFields(CardModel __instance, Task originalTask)
    {
        try
        {
            await originalTask;
        }
        finally
        {
            GoldSpendFields.Set(__instance, 0);
            GoldSpendFields.SetAutoPlay(__instance, false);
        }
    }
}

// 3c. Intercepts CardCmd.AutoPlay to flag the card during auto-play tasks
[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.AutoPlay))]
internal class CardCmdAutoPlayGoldPatch
{
    [HarmonyPrefix]
    private static void Prefix(CardModel card)
    {
        if (card != null)
        {
            GoldSpendFields.SetAutoPlay(card, true);
        }
    }

    [HarmonyPostfix]
    private static void Postfix(CardModel card, ref Task __result)
    {
        if (card != null)
        {
            __result = ResetAutoPlayState(card, __result);
        }
    }

    private static async Task ResetAutoPlayState(CardModel card, Task originalTask)
    {
        try
        {
            await originalTask;
        }
        finally
        {
            GoldSpendFields.SetAutoPlay(card, false);
        }
    }
}

// 4. Assigns custom enum value during data phase when card is affordable with gold
[HarmonyPatch(typeof(CardCostHelper), nameof(CardCostHelper.GetEnergyCostColor))]
internal class CardCostHelperGetEnergyCostColorPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel card, ref CardCostColor __result)
    {
        var energyValue = card.EnergyCost.CostsX
            ? card.Owner.PlayerCombatState?.Energy ?? 0
            : card.EnergyCost.GetWithModifiers(CostModifiers.All);

        if (GoldSpendHelper.CanPayWithGold(card, energyValue, out var goldRequired) && goldRequired > 0)
        {
            __result = FeixiaoEnums.CostColorGold; 
        }
    }
}

// 5. Intercepts UI text color fetch for our custom Gold cost enum
[HarmonyPatch(typeof(NCard), nameof(NCard.GetCostTextColorInHand))]
public class NCardGetCostTextColorInHandPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardCostColor costColor, ref Color __result)
    {
        if (costColor is not FeixiaoEnums.CostColorGold) return true; 
        __result = new Color(1.0f, 0.843f, 0.0f); // Bright Gold
        return false; 
    }
}

// 6. Intercepts UI outline color fetch to ensure strong contrast against gold text
[HarmonyPatch(typeof(NCard), nameof(NCard.GetCostOutlineColorInHand))]
public class NCardGetCostOutlineColorInHandPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardCostColor costColor, ref Color __result)
    {
        if (costColor is not FeixiaoEnums.CostColorGold) return true;
        __result = new Color(0.35f, 0.25f, 0.0f); // Dark Golden Brown
        return false;
    }
}

// 7. Handles real-time visual updates (icon texture & text) on hand cards dynamically
[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateEnergyCostVisuals))]
public static class NCard_UpdateEnergyCostVisuals_GoldTextPatch
{
    // Lazy-load and cache custom gold coin texture
    private static Texture2D? _goldIcon;
    private static Texture2D GoldIcon => _goldIcon ??= GD.Load<Texture2D>("res://FeixiaoMod/images/enchantments/feihuntarrow_icon.png");

    // Tracks default base game energy textures per NCard instance for bidirectional switching
    private static readonly ConditionalWeakTable<NCard, Texture2D> OriginalIcons = new();

    [HarmonyPostfix]
    public static void Postfix(NCard __instance)
    {
        if (!__instance.IsNodeReady() || __instance.Model is not { } card) return;

        // 1. Securely capture base energy texture prior to mutation
        if (__instance._energyIcon.Texture != GoldIcon)
        {
            if (!OriginalIcons.TryGetValue(__instance, out _))
            {
                OriginalIcons.Add(__instance, __instance._energyIcon.Texture);
            }
        }

        // 2. Evaluate current affordability

        var energyValue = card.EnergyCost.CostsX 
            ? card.Owner.PlayerCombatState?.Energy ?? 0 
            : Math.Max(0, card.EnergyCost.GetWithModifiers(CostModifiers.All));
        var isFreeOrZero = energyValue is 0;
        var goldCost = 0;
        var canPayWithGold = !isFreeOrZero && GoldSpendHelper.CanPayWithGold(card, energyValue, out goldCost);

        // 3. Apply state-dependent visuals
        if (canPayWithGold)
        {
            __instance._energyLabel.SetTextAutoSize(goldCost.ToString());
            __instance._energyIcon.Texture = GoldIcon;
        }
        else
        {
            // Player cannot afford gold; revert icon back to standard energy orb
            if (OriginalIcons.TryGetValue(__instance, out var originalTexture))
            {
                __instance._energyIcon.Texture = originalTexture;
            }
        }
    }
}

// 8. Forces hand cards to re-evaluate costs when gold is gained in-combat
[HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.GainGold))]
internal class PlayerCmdGainGoldRefreshPatch
{
    [HarmonyPostfix]
    private static void Postfix(Player player, ref Task __result)
    {
        __result = ExecuteAfterGainGold(player, __result);
    }

    private static async Task ExecuteAfterGainGold(Player player, Task originalTask)
    {
        // Await gold transaction completion
        await originalTask;

        var cardsInHand = PileType.Hand.GetPile(player).Cards;

        foreach (var card in cardsInHand)
        {
            if (card.Enchantment is not SkullGoldEnchant) continue;

            // Bypasses C# private event restrictions via Harmony Traverse to raise EnergyCostChanged
            var onCostChanged = Traverse.Create(card).Field("EnergyCostChanged").GetValue<Action>();
            onCostChanged?.Invoke();
        }
    }
}

// 9. Forces gold text recoloring when viewing enchanted cards in piles other than the Hand
[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateEnergyCostColor))]
public static class NCard_UpdateEnergyCostColor_GoldPatch
{
    [HarmonyPostfix]
    public static void Postfix(NCard __instance, PileType pileType)
    {
        // 1. Hand cards are already handled by GetCostTextColorInHand / GetCostOutlineColorInHand patches
        if (pileType is PileType.Hand) return;
        if (!__instance.IsNodeReady() || __instance.Model is not { } card) return;

        // 2. Check if the card has the gold enchantment
        if (card.Enchantment is not SkullGoldEnchant) return;

        // 3. Evaluate energy value
        var energyValue = card.EnergyCost.CostsX
            ? card.Owner?.PlayerCombatState?.Energy ?? 0
            : Math.Max(0, card.EnergyCost.GetWithModifiers(CostModifiers.All));

        // 4. If payable with gold and cost > 0, apply gold text and outline theme overrides
        if (!GoldSpendHelper.CanPayWithGold(card, energyValue, out var goldRequired) || goldRequired <= 0) return;
        var goldTextColor = new Color(1.0f, 0.843f, 0.0f);       // Bright Gold
        var goldOutlineColor = new Color(0.35f, 0.25f, 0.0f);    // Dark Golden Brown

        __instance._energyLabel.AddThemeColorOverride(ThemeConstants.Label.FontColor, goldTextColor);
        __instance._energyLabel.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, goldOutlineColor);
    }
}
#endregion