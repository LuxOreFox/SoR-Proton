using HarmonyLib;
using RogueLibsCore;

namespace Proton.VanillaTweak;

[HarmonyPatch(typeof(ItemFunctions))]
internal static class MiniFridgeTweak
{
    [HarmonyPatch(nameof(ItemFunctions.DetermineHealthChange))]
    [HarmonyPostfix]
    private static void Postfix_MiniFridge(ref int __result, InvItem item, Agent agent)
    {
        if (item.Categories.Contains(RogueCategories.Alcohol) 
            && !item.Categories.Contains(RogueCategories.Food) 
            && agent.inventory.HasItem(VanillaItems.MiniFridge))
        {
            float newResult = (float)__result;
            newResult *= 1.2f;
            __result = (int)newResult;
        }
    }
}
