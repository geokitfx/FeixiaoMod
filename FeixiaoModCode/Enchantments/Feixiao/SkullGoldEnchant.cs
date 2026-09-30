using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace FeixiaoMod.FeixiaoModCode.Enchantments.Feixiao;

public class SkullGoldEnchant : CustomEnchantmentModel
{
    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType is CardType.Skill;
    }
    
    protected override string CustomIconPath => "res://FeixiaoMod/images/enchantments/feibalesacle_icon.png";
}