using BaseLib.Abstracts;
using FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace FeixiaoMod.FeixiaoModCode.RestSite;

public class CostCloneRestSiteOption : CustomRestSiteOption
{
    public override string OptionId => "CLONE";

    public override LocString Description
    {
        get
        {
            LocString description = base.Description;
            description.Add("EnchantmentName", ModelDb.Enchantment<CostCloneEnchant>().Title.GetFormattedText());
            return description;
        }
    }

    public CostCloneRestSiteOption(Player owner)
        : base(owner)
    {
    }

    public override async Task<bool> OnSelect()
    {
        IEnumerable<CardModel> enumerable = Owner.Deck.Cards.Where((c) => c.Enchantment is CostCloneEnchant).ToList();
        List<CardPileAddResult> results = new List<CardPileAddResult>();
        foreach (CardModel item in enumerable)
        {
            CardModel card = Owner.RunState.CloneCard(item);
            List<CardPileAddResult> list = results;
            list.Add(await CardPileCmd.Add(card, PileType.Deck));
        }
        CardCmd.PreviewCardPileAdd(results, 1.2f, CardPreviewStyle.MessyLayout);
        return true;
    }

    public override Task DoLocalPostSelectVfx(CancellationToken ct = default(CancellationToken))
    {
        NGame.Instance?.ScreenShake(ShakeStrength.Strong, ShakeDuration.Short);
        return Task.CompletedTask;
    }

    public override Task DoRemotePostSelectVfx()
    {
        var nRestSiteCharacter = NRestSiteRoom.Instance?.Characters.First((c) => c.Player == Owner);
        nRestSiteCharacter?.Shake();
        var nRelicFlashVfx = NRelicFlashVfx.Create(ModelDb.Relic<PaelsGrowth>());
        if (nRelicFlashVfx == null)
        {
            return Task.CompletedTask;
        }
        nRestSiteCharacter?.AddChildSafely(nRelicFlashVfx);
        nRelicFlashVfx.Position = Vector2.Zero;
        return Task.CompletedTask;
    }
}