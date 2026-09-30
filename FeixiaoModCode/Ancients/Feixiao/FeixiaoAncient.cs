using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using FeixiaoMod.FeixiaoModCode.Relics.Feixiao;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace FeixiaoMod.FeixiaoModCode.Ancients.Feixiao;

public class FeixiaoAncient  : CustomAncientModel 
{ 
    public override string CustomScenePath => "res://FeixiaoMod/ancients/scenes/feixiao.tscn";
    public override string CustomMapIconPath => "res://FeixiaoMod/ancients/images/packed/map/ancients/ancient_node_feixiaomod-feixiaoancient.png";
    public override string CustomMapIconOutlinePath => "res://FeixiaoMod/ancients/images/packed/map/ancients/ancient_node_feixiaomod-feixiaoancient_outline.png";
    public override string CustomRunHistoryIconPath => "res://FeixiaoMod/ancients/ui/run_history/feixiaomod-feixiaoancient.png";
    public override string CustomRunHistoryIconOutlinePath => "res://FeixiaoMod/ancients/ui/run_history/feixiaomod-feixiaoancient_outline.png";
    protected override OptionPools MakeOptionPools
    {
        get
        {
            List<AncientOption> listOfAncientOptions = [];

            // If Owner is null -> return ALL options
            if (Owner == null)
            {
                AddGlobalOptions(listOfAncientOptions, null, null);
                return new OptionPools(MakePool(listOfAncientOptions.ToArray()));
            }
            var characterId = Owner.Character.Id;
            bool isSinglePlayer = Owner.RunState.CardMultiplayerConstraint == CardMultiplayerConstraint.SingleplayerOnly;
            int actNumber = Owner.RunState.Act.ActNumber();
            AddGlobalOptions(listOfAncientOptions, isSinglePlayer, actNumber);
            return new OptionPools(MakePool(listOfAncientOptions.ToArray()));
        }
    }

    public OptionPools GetDynamicOptionPools()
    {
        return MakeOptionPools;
    }

    private void AddGlobalOptions(List<AncientOption> options, bool? isSinglePlayer, int? actNumber)
    {
        // Lock Relics to Specifically Act 2
        if (actNumber is null or 2)
        {
            options.Add(AncientOption<WindPupperPlushRelic>(weight: 100));
        }

        // Lock Relics to Specifically Act 3
        if (actNumber is null or 3)
        { 
            options.Add(AncientOption<GoldKunouRelic>(weight: 100));
        }
        
        // Relics here aren't locked to an Act
        options.Add(AncientOption<YasakaFumoRelic>(weight: 100));
        options.Add(AncientOption<SwaddlingClothRelic>(weight: 100));
        options.Add(AncientOption<AxeRelic>(weight: 100));
        options.Add(AncientOption<OfferingPlateRelic>(weight: 100));
        options.Add(AncientOption<FangedGrinRelic>(weight: 100));
        options.Add(AncientOption<TouchFluffyTailRelic>(weight: 100));
        options.Add(AncientOption<WombTattooRelic>(weight: 100));
        options.Add(AncientOption<MilkBottleRelic>(weight: 100));
        // options.Add(AncientOption<PreysSkullRelic>(weight: 100));
        options.Add(AncientOption<MedicFanRelic>(weight: 100));
            
        /*
        // Relics here are Locked to Single Player
        if (isSinglePlayer is null or true)
        {
            // Lock Relics to Specifically Act 2
            if (actNumber is null or 2)
            {

            }
            // Lock Relics to Specifically Act 3
            if (actNumber is null or 3)
            {
                
            }
            // Relics here aren't locked to an Act
        }
        */
    }

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 1 && !FeixiaoMod_ModConfig.Disable_Feixiao || act.ActNumber() == 2 || act.ActNumber() == 3;
    }
}