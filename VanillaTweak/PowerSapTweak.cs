using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Proton.VanillaTweak;

[HarmonyPatch(typeof(Explosion))]
internal static class PowerSapTweak
{
    [HarmonyPatch(nameof(Explosion.ExplosionHit))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler_RemoveHasRechargedCheck(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        for (int i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == OpCodes.Ldloc_S 
                && codes[i + 1].opcode == OpCodes.Brfalse 
                && codes[i + 2].opcode == OpCodes.Ldarg_0 
                && codes[i + 3].opcode == OpCodes.Ldfld 
                && codes[i + 3].operand.ToString().Contains("hasRecharged") 
                && codes[i + 4].opcode == OpCodes.Brtrue)
            {
                codes[i + 2] = new CodeInstruction(OpCodes.Nop);
                codes[i + 3] = new CodeInstruction(OpCodes.Nop);
                codes[i + 4] = new CodeInstruction(OpCodes.Nop);
                break;
            }
        }

        return codes;
    }
}
