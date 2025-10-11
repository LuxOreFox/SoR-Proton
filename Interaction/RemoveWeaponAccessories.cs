using HarmonyLib;
using RogueLibsCore;

namespace Proton.Interaction;

public static class RemoveWeaponAccessories
{
    [RLSetup]
    private static void Setup()
    {
        RogueInteractions.CreateProvider<AmmoDispenser>(static h =>
        {
            if (h.Helper.interactingFar) return;

            if (!CanProvide(h.Object) || !CanReceive(h.Agent)) return;

            h.AddButton(BT_RemoveWeaponAccessories, static m => m.Object.ShowUseOn(BT_RemoveWeaponAccessories));
        });

        RogueLibs.CreateCustomName(BT_RemoveWeaponAccessories, NameTypes.Interface, new CustomNameInfo()
        {
            English = "Remove Weapon Accessories",
            Chinese = "移除武器配件"
        });
    }

    public static bool CanProvide(AmmoDispenser provider)
    {
        return provider.functional;
    }

    public static bool CanReceive(Agent receiver)
    {
        foreach (InvItem weapon in receiver.inventory.weaponList)
        {
            if (weapon.invItemName == VanillaItems.Fist) continue;

            if (weapon.weaponCode == weaponType.WeaponProjectile || weapon.weaponCode == weaponType.WeaponMelee) return true;
        }
        return false;
    }

    public const string BT_RemoveWeaponAccessories = "BT_RemoveWeaponAccessories";
}

[HarmonyPatch(typeof(AmmoDispenser))]
internal static class RemoveWeaponAccessories_Patch
{
    [HarmonyPatch(nameof(AmmoDispenser.UseItemOnObject))]
    [HarmonyPrefix]
    private static bool Prefix_AddFunction(bool __runOriginal, AmmoDispenser __instance, ref bool __result, InvItem item, string combineType, string useOnType)
    {
        if (__runOriginal == false) return false;

        if (useOnType != RemoveWeaponAccessories.BT_RemoveWeaponAccessories) return true;

        if ((item.weaponCode == weaponType.WeaponProjectile | item.weaponCode == weaponType.WeaponMelee) && item.contents.Count > 0 && !item.weaponToBeLoaded)
        {
            if (combineType == "Combine")
            {
                Agent agent = __instance.interactingAgent;
                int index = item.contents.Count - 1;
                string accessory = item.contents[index];
                if (accessory == VanillaItems.AmmoCapacityMod)
                {
                    item.maxAmmo = TryRestoreOriginalValue(item.maxAmmo);
                    item.itemValue = TryRestoreOriginalValue(item.itemValue);
                }
                agent.inventory.AddItemOrDrop(accessory, 1);
                item.contents.RemoveAt(index);
                __instance.PlayAnim("MachineOperate", agent);
            }
            __result = true;
        }
        else
        {
            __result = false;
        }
        return false;
    }

    private static int TryRestoreOriginalValue(int currentValue)
    {
        int originalValue = (int)(currentValue / 1.4f);
        if ((originalValue * 1.4f) != currentValue)
        {
            originalValue++;
        }
        return originalValue;
    }
}
