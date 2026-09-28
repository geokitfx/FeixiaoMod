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
public class SwaddlingClothRelic() : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RegenPower>(5)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<RegenPower>()];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType == RoomType.Elite || room.RoomType == RoomType.Boss)
        {
            Flash();
            await PowerCmd.Apply<RegenPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["RegenPower"].BaseValue, Owner.Creature, null);
        }
    }
    
    
    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiSwaddlingCloth_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiSwaddlingCloth_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiSwaddlingCloth_relic.png";
}