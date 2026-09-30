﻿using BaseLib.Abstracts;
using BaseLib.Utils;
using FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace FeixiaoMod.FeixiaoModCode.Relics.Feixiao;

[Pool(typeof(EventRelicPool))]
public class PreysSkullRelic() : CustomRelicModel
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
        if (!ModelDb.Enchantment<SkullGoldEnchant>().CanEnchant(card))
        {
            return false;
        }
        newCard = EnchantCard(card);
        return true;
    }

    private void CardSkillCheck(List<CardCreationResult> options)
    {
        SkullGoldEnchant se2 = ModelDb.Enchantment<SkullGoldEnchant>();
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
        CardCmd.Enchant<SkullGoldEnchant>(item, 1);
        CardCmd.Preview(item);
        return item;
    }


    public override async Task AfterObtained()
    {
        IEnumerable<CardModel> enumerable = PileType.Deck.GetPile(Owner).Cards.ToList();
        foreach (CardModel item in enumerable)
        {
            if (ModelDb.Enchantment<SkullGoldEnchant>().CanEnchant(item))
            {
                CardCmd.Enchant<SkullGoldEnchant>(item, 1);
                CardCmd.Preview(item);
            }
        }
    }
    
    public override string PackedIconPath => "res://FeixiaoMod/images/relics/feiPreysSkull_relic.png";
    protected override string PackedIconOutlinePath => "res://FeixiaoMod/images/relics/feiPreysSkull_relic.png";
    protected override string BigIconPath  => "res://FeixiaoMod/images/relics/feiPreysSkull_relic.png";
}