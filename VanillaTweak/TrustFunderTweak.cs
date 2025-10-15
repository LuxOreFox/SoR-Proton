using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RogueLibsCore;

namespace Proton.VanillaTweak;

[HarmonyPatch(typeof(Agent))]
public static class TrustFunderTweak
{
    [HarmonyPatch(nameof(Agent.AddMoneyAtPlayerStart))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler_TrustFunder(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        for (int i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == OpCodes.Ldstr 
                && codes[i].operand.ToString().Contains(VanillaTraits.TrustFunder2) 
                && codes[i + 4].opcode == OpCodes.Ldc_I4_S 
                && (sbyte)codes[i + 4].operand == 80 
                && codes[i + 8].opcode == OpCodes.Ldc_I4_S 
                && (sbyte)codes[i + 8].operand == 40)
            {
                codes[i + 4] = new CodeInstruction(OpCodes.Ldc_I4, IncomeOfTrustFunder2);
                codes[i + 8] = new CodeInstruction(OpCodes.Ldc_I4_S, IncomeOfTrustFunder);
                break;
            }
        }

        return codes;
    }

    public static int IncomeOfTrustFunder { get; private set; } = 100;
    public static int IncomeOfTrustFunder2 { get; private set; } = 200;
}
