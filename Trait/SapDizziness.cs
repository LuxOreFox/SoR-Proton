using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RogueLibsCore;

namespace Proton.Trait;

public class SapDizziness : CustomTrait
{
    [RLSetup]
    private static void Setup()
    {
        RogueLibs.CreateCustomTrait<SapDizziness>()
            .WithName(new CustomNameInfo()
            {
                English = "Sap Dizziness", 
                Chinese = "吸耗眩晕"
            })
            .WithDescription(new CustomNameInfo()
            {
                English = "Power Sap always inflicts Dizzy on people in its radius.", 
                Chinese = "电力吸耗总是使处于其效力半径内的人眩晕。"
            })
            .WithUnlock(new TraitUnlock
            {
                CharacterCreationCost = 5,
                SpecialAbilities = new List<string> { VanillaAbilities.PowerSap },
                IsAvailable = false,
                IsAvailableInCC = false
            });
    }

    public override void OnAdded() { }

    public override void OnRemoved() { }
}

[HarmonyPatch(typeof(Explosion))]
public static class SapDizziness_Patch
{
    [HarmonyPatch(nameof(Explosion.ExplosionHit))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler_AddChecker(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        for (int i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == OpCodes.Ldarg_0 
                && codes[i + 1].opcode == OpCodes.Ldfld 
                && codes[i + 1].operand.ToString().Contains("gc") 
                && codes[i + 2].opcode == OpCodes.Ldc_I4_S 
                && (sbyte)codes[i + 2].operand == 25 
                && codes[i + 3].opcode == OpCodes.Callvirt 
                && codes[i + 3].operand.ToString().Contains("percentChance"))
            {
                codes[i + 1] = new CodeInstruction(
                    OpCodes.Call, typeof(SapDizziness_Patch).GetMethod(
                        nameof(CheckSapDizziness), BindingFlags.NonPublic | BindingFlags.Static));
                codes[i + 2] = new CodeInstruction(OpCodes.Nop);
                codes[i + 3] = new CodeInstruction(OpCodes.Nop);
            }
        }

        return codes;
    }

    private static bool CheckSapDizziness(Explosion explosion)
    {
        if (explosion.explosionType == "PowerSap" 
            && explosion.agent != null 
            && (explosion.agent.HasTrait(nameof(SapDizziness)) 
                || explosion.agent.agentName == VanillaAgents.Robot && explosion.agent.oma.superSpecialAbility)) 
            return true;
        return explosion.gc.percentChance(25);
    }
}
