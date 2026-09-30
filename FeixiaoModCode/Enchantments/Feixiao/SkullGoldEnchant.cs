using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;

public class SkullGoldEnchant : CustomEnchantmentModel
{
    private int _goldCost = 2;
    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType is CardType.Skill;
    }
   /*
    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        _goldCost = Card.EnergyCost.GetAmountToSpend();
        return Task.CompletedTask;
    } */
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal feiOriginalCost, out decimal feiModifiedCost)
    {
        int goldCost = _goldCost;
        feiModifiedCost = feiOriginalCost;
        if (card.Enchantment is SkullGoldEnchant && Card.Owner.Gold >= goldCost)
        {
            feiModifiedCost = 0;
            return true;
        }
        return false;
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        int goldCost = _goldCost;
        if (Card.Owner.Gold >= goldCost)
        {
            await PlayerCmd.LoseGold(goldCost, Card.Owner);
        }
    }
    
    protected override string CustomIconPath => "res://FeixiaoMod/images/enchantments/feibalesacle_icon.png";
}