using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace FeixiaoMod.FeixiaoModCode.Cards.Feixiao;

[Pool(typeof(EventCardPool))]
public class CalciumDeficiencyCard() : CustomCardModel(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [new HoverTip(new LocString("cards", "FEIXIAOMOD-CALCIUM_DEFICIENCY_CARD.flavor"))];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate, CardKeyword.Retain, CardKeyword.Unplayable];
    
    public override bool CanBeGeneratedByModifiers => false;
    
    public override int MaxUpgradeLevel => 0;
    
    public override string CustomPortraitPath => "res://FeixiaoMod/images/cards/feiMilk.png";
    
}