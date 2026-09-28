using BaseLib.Abstracts;
using BaseLib.Utils;
using FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace FeixiaoMod.FeixiaoModCode.Relics.Feixiao;

[Pool(typeof(EventRelicPool))]
public class OfferingPlateRelic() : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new ("feiDoomEnVal", 10)];
    
    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        DoomEnchant canonicalDoomEnchant = ModelDb.Enchantment<DoomEnchant>();
        foreach (CardModel item in await CardSelectCmd.FromDeckForEnchantment(Owner, canonicalDoomEnchant, DynamicVars["feiDoomEnVal"].IntValue, prefs))
        {
            CardCmd.Enchant(canonicalDoomEnchant.ToMutable(), item, DynamicVars["feiDoomEnVal"].IntValue);
            CardCmd.Preview(item);
        }
    }
    
    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiOfferingPlate_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiOfferingPlate_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiOfferingPlate_relic.png";
}