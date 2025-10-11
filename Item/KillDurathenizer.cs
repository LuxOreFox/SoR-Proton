using System;
using HarmonyLib;
using Neutron.Category;
using RogueLibsCore;

namespace Proton.Item;

[ItemCategories(RogueCategories.Passive, RogueCategories.Melee, RogueCategories.MeleeAccessory)]
public class KillDurathenizer : CustomItem
{
    [RLSetup]
    private static void Setup()
    {
        RogueLibs.CreateCustomItem<KillDurathenizer>()
            .WithName(new CustomNameInfo()
            {
                English = "Kill Durathenizer", 
                Chinese = "杀戮耐久奖励器"
            })
            .WithDescription(new CustomNameInfo()
            {
                English = "Killing people repairs your melee weapon and armor. But it won't mend your shattered soul!", 
                Chinese = "杀人会修补你的近战武器和护甲。但无法修复你破碎的灵魂！"
            })
            .WithSprite(Properties.ItemSprites.KillProfiterDurability)
            .WithUnlock(new ItemUnlock { CharacterCreationCost = 4 });
    }

    public override void SetupDetails()
    {
        Item.itemType= ItemTypes.Tool;
        Item.initCount = 1;
        Item.itemValue = 100;
        Item.stackable = false;
        Item.notInLoadoutMachine = true;
    }

    public static int DefaultIncrement { get; private set; } = 15;
}

[HarmonyPatch(typeof(StatusEffects))]
public static class KillDurathenizer_Patch
{
    [HarmonyPatch(nameof(StatusEffects.ActivateKillProfiter))]
    [HarmonyPostfix]
    private static void Postfix_AddActivator(StatusEffects __instance)
    {
        Agent target = __instance.agent.lastHitByAgent;
        if (__instance.killProfiterActivated && target.inventory.HasItem(nameof(KillDurathenizer)))
        {
            target.statusEffects.ActivateKillDurathenizer(KillDurathenizer.DefaultIncrement);
        }
    }

    public static void ActivateKillDurathenizer(this StatusEffects statusEffects, int durabilityIncrement)
    {
        foreach (InvItem item in statusEffects.agent.agentInvDatabase.InvItemList)
        {
            if (item.weaponCode != weaponType.WeaponMelee && item.itemType != ItemTypes.Wearable) continue;

            if (item.Categories.Contains(NItemCategories.Rechargeable)) continue;

            if (item.invItemCount < item.initCount)
            {
                int actualIncrement = Math.Min(durabilityIncrement, item.initCount - item.invItemCount);
                item.invItemCount += actualIncrement;
                GameController.gameController.spawnerMain.SpawnStatusText(statusEffects.agent, 
                    "ItemPickup", item.invItemName, "", 
                    "ItemPickUp", actualIncrement.ToString());
            }
        }
    }
}
