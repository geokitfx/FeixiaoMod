using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace FeixiaoMod.FeixiaoModCode.Relics.Feixiao;

[Pool(typeof(EventRelicPool))]
public class GoldKunouRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    private const string RoomCount = "RoomCount";
    private int _roomCounter;

    public override bool ShowCounter => true;
    public override int DisplayAmount => _roomCounter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new (RoomCount, 3)];

    private int RoomCounter
    {
        get => _roomCounter;
        set
        {
            AssertMutable();
            _roomCounter = value;
            InvokeDisplayAmountChanged();
        }
    }
    
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (!(room.RoomType == RoomType.Monster || room.RoomType == RoomType.Elite || room.RoomType == RoomType.Boss)) 
            return;
        RoomCounter++;
        Flash();
        if (RoomCounter >= DynamicVars["RoomCount"].BaseValue)
        {
            await PlayerCmd.GainGold(Owner.Gold, Owner);
            RoomCounter = 0;
        }
    }

    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiGoldKunou_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiGoldKunou_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiGoldKunou_relic.png";
}