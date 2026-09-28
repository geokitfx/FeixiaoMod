using BaseLib.Abstracts;
using BaseLib.Utils;
using FeixiaoMod.FeixiaoModCode.Cards.Feixiao;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace FeixiaoMod.FeixiaoModCode.Relics.Feixiao;

[Pool(typeof(EventRelicPool))]
public class FangedGrinRelic() : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    public override async Task AfterObtained()
    {
        CustomCardModel fangedGrinCard = Owner.RunState.CreateCard<FangedGrinCard>(Owner);
        var cardPileAddResult = await CardPileCmd.Add(fangedGrinCard, PileType.Deck);
        CardCmd.PreviewCardPileAdd(cardPileAddResult, 2f);
    }
    
    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiFangedGrin_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiFangedGrin_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiFangedGrin_relic.png";
}