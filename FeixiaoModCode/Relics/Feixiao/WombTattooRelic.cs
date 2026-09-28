using BaseLib.Abstracts;
using BaseLib.Utils;
using FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;

namespace FeixiaoMod.FeixiaoModCode.Relics.Feixiao;

[Pool(typeof(EventRelicPool))]
public class WombTattooRelic() : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    public override bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> cardRewards, CardCreationOptions options)
    {
        CardSkillCheck(cardRewards);
        return true;
    }

    public override void ModifyMerchantCardCreationResults(Player player, List<CardCreationResult> cards)
    {
        CardSkillCheck(cards);
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        newCard = null;
        if (!ModelDb.Enchantment<RuneHuntEnchant>().CanEnchant(card))
        {
            return false;
        }
        newCard = EnchantCard(card);
        return true;
    }

    private void CardSkillCheck(List<CardCreationResult> options)
    {
        RuneHuntEnchant se2 = ModelDb.Enchantment<RuneHuntEnchant>();
        foreach (CardCreationResult option in options)
        {
            CardModel card = option.Card;
            if (se2.CanEnchant(card))
            {
                option.ModifyCard(EnchantCard(card), this);
            }
        }
    }

    private CardModel EnchantCard(CardModel card)
    {
        CardModel item = Owner.RunState.CloneCard(card);
        CardCmd.Enchant<RuneHuntEnchant>(item, 1);
        CardCmd.Preview(item);
        return item;
    }


    public override async Task AfterObtained()
    {
        IEnumerable<CardModel> enumerable = PileType.Deck.GetPile(Owner).Cards.ToList();
        foreach (CardModel item in enumerable)
        {
            if (item.Tags.Contains(CardTag.Strike) && ModelDb.Enchantment<RuneHuntEnchant>().CanEnchant(item))
            {
                CardCmd.Enchant<RuneHuntEnchant>(item, 1);
                CardCmd.Preview(item);
            }
        }
    }
    
    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiWombTattoo_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiWombTattoo_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiWombTattoo_relic.png";
}