using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;


namespace FeixiaoMod.FeixiaoModCode.Cards.Feixiao;


[Pool(typeof(EventCardPool))]
public class FangedGrinCard() : CustomCardModel(2, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override void AddExtraArgsToDescription(LocString description)
    {
        var playerCount = RunState?.Players.Count ?? 1; // choose desired default here
                          description.Add("IsMultiplayer", playerCount > 1);
    }
    
    public override bool CanBeGeneratedInCombat => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(damage: 10, ValueProp.Move) , new("Healing", 7)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Fatal)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = CombatState;
        if (combatState != null)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            bool shouldTriggerFatal = cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
            AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_bite", null, "blunt_attack.mp3").Execute(choiceContext);
            if (shouldTriggerFatal && attackCommand.Results.SelectMany(r => r).Any(r => r.WasTargetKilled))
            {
                foreach (var players in combatState.GetTeammatesOf(Owner.Creature).Where(c => c is { IsAlive: true, IsPlayer: true }))
                { 
                    await CreatureCmd.Heal(players, DynamicVars["Healing"].BaseValue);;
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Healing"].UpgradeValueBy(3);
    }
    
    public override string PortraitPath   => "res://FeixiaoMod/images/cards/feiBlood_Moon.png";
    public override string CustomPortraitPath => "res://FeixiaoMod/images/cards/feiBlood_Moon.png";
}