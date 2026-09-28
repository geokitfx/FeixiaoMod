using BaseLib.Abstracts;
using BaseLib.Utils;
using FeixiaoMod.FeixiaoModCode.Cards.Feixiao;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace FeixiaoMod.FeixiaoModCode.Relics.Feixiao;

[Pool(typeof(EventRelicPool))]
public class AxeRelic() : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    public override async Task AfterObtained()
    {
        CustomCardModel axeCard = Owner.RunState.CreateCard<AxeCard>(Owner);
        var cardPileAddResult = await CardPileCmd.Add(axeCard, PileType.Deck);
        CardCmd.PreviewCardPileAdd(cardPileAddResult, 2f);
    }
    
    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiAxe_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiAxe_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiAxe_relic.png";
}