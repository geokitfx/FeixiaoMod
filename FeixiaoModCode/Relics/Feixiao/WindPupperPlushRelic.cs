using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace FeixiaoMod.FeixiaoModCode.Relics.Feixiao;

[Pool(typeof(EventRelicPool))]
public class WindPupperPlushRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PlatingPower>(5M)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            await AddPlating();
        }
    }

    private async Task AddPlating()
    {
            var creature = Owner.Creature;
            var missingHp = creature.MaxHp - creature.CurrentHp;
            var interval = (missingHp * 4) / creature.MaxHp;
            if (interval > 0)
            {
                Flash();
                var windPlating = interval * DynamicVars["PlatingPower"].BaseValue;
                await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, windPlating, Owner.Creature, null);
            }
    }
    
    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiWindPupperPlush_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiWindPupperPlush_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiWindPupperPlush_relic.png";
}