using BaseLib.Abstracts;
using BaseLib.Utils;
using FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;
using FeixiaoMod.FeixiaoModCode.RestSite;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace FeixiaoMod.FeixiaoModCode.Relics.Feixiao;

[Pool(typeof(EventRelicPool))]
public class TouchFluffyTailRelic() : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        CostCloneEnchant canonicalCostCloneEnchant = ModelDb.Enchantment<CostCloneEnchant>();
        foreach (CardModel item in await CardSelectCmd.FromDeckForEnchantment(Owner, canonicalCostCloneEnchant, 1, prefs))
        {
            CardCmd.Enchant<CostCloneEnchant>(item, 1);
            CardCmd.Preview(item);
        }
    }
    
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner)
        {
            return false;
        }
        options.Add(new CostCloneRestSiteOption(player));
        return true;
    }
    
    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiTouchFluffyTail_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiTouchFluffyTail_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiTouchFluffyTail_relic.png";
}