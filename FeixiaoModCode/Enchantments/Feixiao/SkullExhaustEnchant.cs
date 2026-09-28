using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;

public class SkullExhaustEnchant : CustomEnchantmentModel
{
    
    public override bool CanEnchantCardType(CardType cardType)
    {
        // return cardType is CardType.Attack or CardType.Skill; (This is to come back and steal for other shit later.)
        return cardType is CardType.Skill;
    }

    protected override void OnEnchant()
    {
        Card.AddKeyword(CardKeyword.Exhaust);
    }
    
    protected override string CustomIconPath => "res://FeixiaoMod/images/enchantments/feibalesacle_icon.png";
}