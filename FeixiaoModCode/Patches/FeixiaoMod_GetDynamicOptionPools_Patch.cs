﻿using BaseLib.Abstracts;
using BaseLib.Utils;
using FeixiaoMod.FeixiaoModCode.Ancients.Feixiao;
using HarmonyLib;
 
namespace FeixiaoMod.FeixiaoModCode.Patches;

[HarmonyPatch(typeof(CustomAncientModel), nameof(CustomAncientModel.OptionPools), MethodType.Getter)]
public static class FeixiaoMod_GetDynamicOptionPools_Patch
{
    static bool Prefix(CustomAncientModel __instance, ref OptionPools __result)
    {
        if (__instance is FeixiaoAncient myModel)
        {
            // Compute dynamically every time, no caching
            __result = myModel.GetDynamicOptionPools();
            return false; // skip original getter
        }
        return true; // run original getter for all other classes
    }
}