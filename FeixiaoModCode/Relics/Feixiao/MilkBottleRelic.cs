using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace FeixiaoMod.FeixiaoModCode.Relics.Feixiao;

[Pool(typeof(EventRelicPool))]
public class MilkBottleRelic() : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        return player != Owner ? amount : amount + DynamicVars.Energy.IntValue;
    }
    
    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiMilkBottle_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiMilkBottle_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiMilkBottle_relic.png";
}