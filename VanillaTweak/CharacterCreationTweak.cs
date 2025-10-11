using System;
using HarmonyLib;
using RogueLibsCore;

namespace Proton.VanillaOptimized;

[HarmonyPatch(typeof(CharacterCreation))]
internal static class CharacterCreationTweak
{
    private static bool _hasAddedBodyType = false;

    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    private static void Postfix_AddBlhadBody(CharacterCreation __instance)
    {
        try
        {
            if (!_hasAddedBodyType)
            {
                int index = __instance.bodyTypes.IndexOf(VanillaAgents.GangsterCrepe) + 1;
                __instance.bodyTypes.Insert(index, VanillaAgents.GangsterBlahd);
                _hasAddedBodyType = true;
            }
        }
        catch (Exception e)
        {
            ProtonPlugin.Logger.LogError(e);
        }
    }
}
