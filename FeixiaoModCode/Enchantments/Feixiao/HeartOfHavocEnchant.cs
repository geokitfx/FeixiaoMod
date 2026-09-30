using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;

public class HeartOfHavocEnchant : CustomEnchantmentModel
{
    
    public override bool HasExtraCardText => true;
    
    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType == CardType.Attack;
    }
    
    public override decimal EnchantDamageMultiplicative(decimal originalDamage, ValueProp props)
    {
        return !props.IsPoweredAttack() ? 1 : 3;
    }
    
    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, Card.Owner.Creature, Amount, Card.Owner.Creature, Card);
    } 
    
    protected override string CustomIconPath => "res://FeixiaoMod/images/enchantments/feiheartofhavoc_icon.png";
}