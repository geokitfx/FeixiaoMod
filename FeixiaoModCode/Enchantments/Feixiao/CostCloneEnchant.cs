using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Enchantments;


namespace FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;

public class CostCloneEnchant : CustomEnchantmentModel
{
    
    // protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<Clone>();
    
    public override bool CanEnchantCardType(CardType cardType)
    {
        // return cardType is CardType.Attack or CardType.Skill; (This is to come back and steal for other shit later.)
        return cardType is CardType.Attack or CardType.Skill;
    }
    
    protected override void OnEnchant()
    {
        Card.EnergyCost.UpgradeBy(-1);
        Card.AddKeyword(CardKeyword.Exhaust);
    }
    
    /*
     I just didn't want to delete this because I was very proud of it. Would this bloat file sizes?
     Maybe but the mod is small AF anyway.
     
    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        Card.EnergyCost.SetThisCombat(ECost());
        return Task.CompletedTask;
    }
    
    private int ECost()
    {
        var ECost = Card.EnergyCost.GetAmountToSpend();
        if (ECost <= -1)
        {
            return 0;
        }
        return ECost -1;
    }
    */

    protected override string CustomIconPath => "res://FeixiaoMod/images/enchantments/feitouchfluffytail_icon.png";
}