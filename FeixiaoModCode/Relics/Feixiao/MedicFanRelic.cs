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
public class MedicFanRelic() : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new ("feiVulEnVal", 2)];

    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        VulnerableEnchant canonicalVulnerableEnchant = ModelDb.Enchantment<VulnerableEnchant>();
        foreach (CardModel item in await CardSelectCmd.FromDeckForEnchantment(Owner, canonicalVulnerableEnchant, DynamicVars["feiVulEnVal"].IntValue, prefs))
        {
            CardCmd.Enchant(canonicalVulnerableEnchant.ToMutable(), item, DynamicVars["feiVulEnVal"].IntValue);
            CardCmd.Preview(item);
        }
    }

    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiMedicFan_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiMedicFan_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiMedicFan_relic.png";
}