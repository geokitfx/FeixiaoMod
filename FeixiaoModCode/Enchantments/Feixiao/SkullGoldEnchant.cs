using BaseLib.Abstracts;
using FeixiaoMod.FeixiaoModCode.Patches;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;

public class SkullGoldEnchant : CustomEnchantmentModel
{
    public override bool HasExtraCardText => true;
    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType is CardType.Skill;
    }
    
    public override int EnchantPlayCount(int originalPlayCount)
    {
        // 1. If card is currently executing an AutoPlay (e.g., Decisions Decisions), do not grant extra replay
        if (GoldSpendFields.IsAutoPlay(Card))
        {
            return originalPlayCount;
        }

        // 2. Otherwise (UI text display or standard manual play), show Replay if affordable with gold
        var energyValue = Card.EnergyCost.CostsX
            ? Card.Owner?.PlayerCombatState?.Energy ?? 0
            : Math.Max(0, Card.EnergyCost.GetWithModifiers(CostModifiers.All));

        if (GoldSpendHelper.CanPayWithGold(Card, energyValue, out var goldRequired) && goldRequired > 0)
        {
            return originalPlayCount + 1;
        }

        return originalPlayCount;
    }
    
    protected override string CustomIconPath => "res://FeixiaoMod/images/enchantments/feibalesacle_icon.png";
}