using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace FeixiaoMod.FeixiaoModCode.Cards.Feixiao;

[Pool(typeof(EventCardPool))]
public class AxeCard : CustomCardModel
{
    public AxeCard() : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self) {}

    private const string Hpl = "hpl";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new (Hpl, 5), new PowerVar<StrengthPower>(7)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars["hpl"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,  this);
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["StrengthPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthPower"].UpgradeValueBy(3);
    }
    
    public override string PortraitPath   => "res://FeixiaoMod/images/cards/feiAxe.png";
    public override string CustomPortraitPath => "res://FeixiaoMod/images/cards/feiAxe.png";
}