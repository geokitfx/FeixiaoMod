using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;

public class SkullGoldEnchant : CustomEnchantmentModel
{
    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType is CardType.Skill;
    }
    
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. Verify that the card played is the one attached to this enchantment
        if (cardPlay.Card != Card) return Task.CompletedTask;

        // 2. Only grant Replay on the initial play, preventing recursive triggers during replay loops
        if (cardPlay.IsFirstInSeries) Card.BaseReplayCount++;

        return Task.CompletedTask;
    }
    
    protected override string CustomIconPath => "res://FeixiaoMod/images/enchantments/feibalesacle_icon.png";
}