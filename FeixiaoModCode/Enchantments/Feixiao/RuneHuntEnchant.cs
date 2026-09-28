using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.ValueProps;

namespace FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;

public class RuneHuntEnchant : CustomEnchantmentModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<Corrupted>();
    
    public override bool HasExtraCardText => true;
    
    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType == CardType.Attack;
    }
    
    public override decimal EnchantDamageMultiplicative(decimal originalDamage, ValueProp props)
    {
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }
        return 1.5m;
    }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, Card.Owner.Creature, 2m, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, Card);
    }
    
    protected override void OnEnchant()
    {
        Card.AddKeyword(CardKeyword.Eternal);
    }
    
    protected override string CustomIconPath => "res://FeixiaoMod/images/enchantments/feihuntarrow_icon.png";
}