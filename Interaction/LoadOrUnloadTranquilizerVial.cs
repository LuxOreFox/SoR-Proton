using System;
using HarmonyLib;
using RogueLibsCore;
using Neutron.Extension;
using Proton.Item;

namespace Proton.Interaction;

public static class LoadOrUnloadTranquilizerVial
{
    [RLSetup]
    private static void Setup()
    {
        RogueInteractions.CreateProvider<AmmoDispenser>(static h =>
        {
            if (h.Helper.interactingFar) return;

            if (!CanProvide(h.Object) || !CanReceive(h.Agent)) return;

            h.AddButton(BT_LoadOrUnloadTranquilizer, static m => m.Object.ShowUseOn(BT_LoadOrUnloadTranquilizer));
        });

        RogueLibs.CreateCustomName(BT_LoadOrUnloadTranquilizer, NameTypes.Interface, new CustomNameInfo()
        {
            English = "Load Or Unload Tranquilizer Vial",
            Chinese = "安装或拆卸麻醉剂瓶"
        });
        RogueLibs.CreateCustomName(DL_CantLoadTranquilizer, NameTypes.Dialogue, new CustomNameInfo()
        {
            English = "Tranquilizer capacity full.",
            Chinese = "麻醉剂容量满了。"
        });
        RogueLibs.CreateCustomName(DL_CantUnloadTranquilizer, NameTypes.Dialogue, new CustomNameInfo()
        {
            English = "Tranquilizer capacity low.",
            Chinese = "麻醉剂余量不足。"
        });
    }

    public static bool CanProvide(AmmoDispenser provider)
    {
        return provider.functional;
    }

    public static bool CanReceive(Agent receiver)
    {
        return receiver.inventory.HasItem(VanillaItems.TranquilizerGun);
    }

    public const string BT_LoadOrUnloadTranquilizer = "BT_LoadOrUnloadTranquilizer";

    public const string DL_CantLoadTranquilizer = "DL_CantLoadTranquilizer";
    public const string DL_CantUnloadTranquilizer = "DL_CantUnloadTranquilizer";
}

[HarmonyPatch(typeof(AmmoDispenser))]
internal static class LoadOrUnloadTranquilizerVial_Patch
{
    [HarmonyPatch(nameof(AmmoDispenser.UseItemOnObject))]
    [HarmonyPrefix]
    private static bool Prefix_AddFunction(bool __runOriginal, AmmoDispenser __instance, ref bool __result, InvItem item, string combineType, string useOnType)
    {
        if (__runOriginal == false) return false;

        if (useOnType != LoadOrUnloadTranquilizerVial.BT_LoadOrUnloadTranquilizer) return true;

        if (item.invItemName == nameof(Tranquilizer))
        {
            if (combineType == "Combine")
            {
                Agent agent = __instance.interactingAgent;
                InvItem tranquilizerGun = agent.inventory.FindItem(VanillaItems.TranquilizerGun);
                if (tranquilizerGun.invItemCount < tranquilizerGun.maxAmmo)
                {
                    agent.inventory.SubtractFromItemCount(item, 1);
                    tranquilizerGun.invItemCount += Math.Min(4, tranquilizerGun.maxAmmo - tranquilizerGun.invItemCount);
                    __instance.Vocalize(VanillaAudio.BuyItem);
                    __instance.PlayAnim("MachineOperate", agent);
                }
                else
                {
                    agent.Talk(LoadOrUnloadTranquilizerVial.DL_CantLoadTranquilizer, VanillaAudio.CantDo);
                }
            }
            __result = true;
        }
        else if (item.invItemName == VanillaItems.TranquilizerGun)
        {
            if (combineType == "Combine")
            {
                Agent agent = __instance.interactingAgent;
                if (item.invItemCount >= 4)
                {
                    item.invItemCount -= 4;
                    agent.inventory.AddItemOrDrop(nameof(Tranquilizer), 1);
                    __instance.PlayAnim("MachineOperate", agent);
                }
                else
                {
                    agent.Talk(LoadOrUnloadTranquilizerVial.DL_CantUnloadTranquilizer, VanillaAudio.CantDo);
                }
            }
            __result = true;
        }
        else
        {
            __result = false;
        }
        return false;
    }
}
