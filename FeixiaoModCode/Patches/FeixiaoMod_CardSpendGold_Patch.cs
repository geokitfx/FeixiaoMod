using System.Runtime.CompilerServices;
using FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace FeixiaoMod.FeixiaoModCode.Patches;

#region Helper Classes

/// <summary>
/// Handles gold payment evaluation and exchange rates.
/// </summary>
public static class GoldSpendHelper
{
    public const int GoldPerEnergy = 3;

    public static bool CanPayWithGold(CardModel? card, int energyCost, out int goldRequired)
    {
        goldRequired = 0;
        
        // 1. Must have the SkullGoldEnchantment
        if (card?.Enchantment is not SkullGoldEnchant) return false;

        // 2. Calculate required gold
        var energy = Math.Max(0, energyCost);
        goldRequired = energy * GoldPerEnergy;

        // 3. Verify owner has sufficient gold
        var currentGold = card.Owner?.Gold ?? 0;
        return currentGold >= goldRequired;
    }
}

/// <summary>
/// Attaches temporary gold cost metadata to card instances across execution frames.
/// </summary>
public static class GoldSpendFields
{
    private class IntBox { public int Value; }
    
    private static readonly ConditionalWeakTable<CardModel, IntBox> Table = new();

    public static int Get(CardModel card)
    {
        return Table.TryGetValue(card, out var box) ? box.Value : 0;
    }

    public static void Set(CardModel card, int value)
    {
        Table.GetOrCreateValue(card).Value = value;
    }
}

#endregion

#region Harmony Patches

// 1. Allows cards to bypass the "Not Enough Energy" playability restriction if player has gold
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
        reason &= ~UnplayableReason.EnergyCostTooHigh;
        __result = (reason == UnplayableReason.None);
    }
}

// 2. Intercepts energy spending: zeros energy cost and registers gold amount if player can afford it
[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendEnergy))]
internal class CardModelSpendEnergyGoldPatch
{
    [HarmonyPrefix]
    private static void Prefix(CardModel __instance, ref int amount)
    {
        if (GoldSpendHelper.CanPayWithGold(__instance, amount, out var goldRequired))
        {
            GoldSpendFields.Set(__instance, goldRequired);
            amount = 0; // Nullify energy cost
        }
        else
        {
            GoldSpendFields.Set(__instance, 0);
        }
    }
}

// 3. Deducts gold asynchronously after card execution completes
[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
internal class CardModelOnPlayWrapperGoldPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, PlayerChoiceContext choiceContext, ref Task __result)
    {
        __result = ExecuteGoldPayment(__instance, choiceContext, __result);
    }

    private static async Task ExecuteGoldPayment(CardModel __instance, PlayerChoiceContext choiceContext, Task originalTask)
    {
        await originalTask;

        var goldToSpend = GoldSpendFields.Get(__instance);

        if (goldToSpend > 0 && __instance.CombatState != null)
        {
            await PlayerCmd.LoseGold(goldToSpend, __instance.Owner);
            GoldSpendFields.Set(__instance, 0);
        }
    }
}

#endregion