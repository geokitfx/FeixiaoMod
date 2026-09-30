using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace FeixiaoMod.FeixiaoModCode.Cards.Feixiao;

[Pool(typeof(EventCardPool))]
public class YasakaFumoCard() : CustomCardModel(1, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DoubleDamagePower>(1)];

    protected override void AddExtraArgsToDescription(LocString description)
    {
        var playerCount = RunState?.Players.Count ?? 1; // choose desired default here
                          description.Add("IsMultiplayer", playerCount > 1);
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = CombatState;
        if (combatState != null)
        {
            foreach (var players in combatState.GetTeammatesOf(Owner.Creature).Where(c => c is { IsAlive: true, IsPlayer: true }))
            { 
                await PowerCmd.Apply<DoubleDamagePower>(choiceContext, players, DynamicVars["DoubleDamagePower"].BaseValue, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
    // PortraitPath 
    public override string PortraitPath   => "res://FeixiaoMod/images/cards/feiWife_Plush_Power.png";
    public override string CustomPortraitPath => "res://FeixiaoMod/images/cards/feiWife_Plush_Power.png";
}