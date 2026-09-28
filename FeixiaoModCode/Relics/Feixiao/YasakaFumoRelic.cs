using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using FeixiaoMod.FeixiaoModCode.Cards.Feixiao;

namespace FeixiaoMod.FeixiaoModCode.Relics.Feixiao;


[Pool(typeof(EventRelicPool))]
public class YasakaFumoRelic() : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    public override bool HasUponPickupEffect => true;
    
    public override async Task AfterObtained()
    {
        CustomCardModel yasakaFumo = Owner.RunState.CreateCard<YasakaFumoCard>(Owner);
        var cardPileAddResult = await CardPileCmd.Add(yasakaFumo, PileType.Deck);
        CardCmd.PreviewCardPileAdd(cardPileAddResult, 2f);
    }
    
    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiYasakaFumo_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiYasakaFumo_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiYasakaFumo_relic.png";
}