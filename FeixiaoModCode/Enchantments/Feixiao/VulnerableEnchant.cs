using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;

public class VulnerableEnchant : CustomEnchantmentModel
{
    public override bool HasExtraCardText => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];
    
    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType is CardType.Attack;
    }
    
    private List<Creature> _hitEnemies = [];
    
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (cardSource == Card && Status == EnchantmentStatus.Normal && !_hitEnemies.Contains(target)){
            await PowerCmd.Apply<VulnerablePower>(choiceContext, target, Amount, dealer, cardSource);
            _hitEnemies.Add(target);
        }
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // The following disables the enchantment for the rest of combat.
        // if (cardPlay.Card == Card && Status == EnchantmentStatus.Normal)
        //     Status = EnchantmentStatus.Disabled;
        _hitEnemies.Clear();
        return base.AfterCardPlayed(choiceContext, cardPlay);
    }


    /* public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (cardSource == Card)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, target, Amount, dealer, cardSource);
        }
    } 
    */
    
    protected override string CustomIconPath => "res://FeixiaoMod/images/enchantments/feivulnerable_enchant_icon.png";
}