using BaseLib.Abstracts;
using FeixiaoMod.FeixiaoModCode.Patches;
using MegaCrit.Sts2.Core.Entities.Cards;

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
        // If gold was flagged for spending on this card instance, grant +1 replay
        if (GoldSpendFields.Get(Card) > 0)
        {
            return originalPlayCount + 1;
        }

        return originalPlayCount;
    }
    
    protected override string CustomIconPath => "res://FeixiaoMod/images/enchantments/feibalesacle_icon.png";
}