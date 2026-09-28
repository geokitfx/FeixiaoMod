using BaseLib.Config;

namespace FeixiaoMod.FeixiaoModCode;

public class FeixiaoMod_ModConfig : SimpleModConfig
{
    [ConfigSection("Feixiao In Act 1")] //
    public static bool Disable_Feixiao { get; set; } = true;
    
    [ConfigSection("Base Game Ancients")] //
    public static bool Disable_Neow { get; set; } = false;
    public static bool Disable_Base_Game_Ancients { get; set; } = false;
}