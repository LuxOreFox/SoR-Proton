using HarmonyLib;
using RogueLibsCore;
using Neutron.Extension;
using Neutron.Helper;

namespace Proton.Item;

[ItemCategories(RogueCategories.Drugs)]
public class Tranquilizer : CustomItem, IItemUsable
{
    [RLSetup]
    private static void Setup()
    {
        RogueLibs.CreateCustomItem<Tranquilizer>()
            .WithName(new CustomNameInfo()
            {
                English = "Tranquilizer",
                Chinese = "麻醉剂"
            })
            .WithDescription(new CustomNameInfo()
            {
                English = "Is it an oral solution? Is it a cleaner? It's a tranquilizer.",
                Chinese = "这是口服液吗？这是清洁剂吗？这是麻醉剂。"
            })
            .WithSprite(Properties.ItemSprites.Tranquilizer, 48)
            .WithUnlock(new ItemUnlock { CharacterCreationCost = 3 });

        RandomTableHelper.AddRandomElement(new RandomTableInfo()
        {
            TableName = "Drugs3",
            TableCategory = "Items",
            ObjectType = "Item"
        }, new Neutron.Helper.RandomElement()
        {
            ElementName = nameof(Tranquilizer),
            ElementChance = 3
        });
    }

    public override void SetupDetails()
    {
        Item.itemType = ItemTypes.Consumable;
        Item.initCount = 1;
        Item.itemValue = 45;
        Item.stackable = true;
        Item.statusEffect = VanillaEffects.Tranquilized;
    }

    public bool UseItem()
    {
        Owner.AddEffect(Item.statusEffect);
        Owner.Vocalize(VanillaAudio.UseSyringe);
        Count--;
        return true;
    }
}

internal static class Tranquilizer_Patch
{
    [HarmonyPatch(typeof(StatusEffects))]
    private static class Part_StatusEffects
    {
        [HarmonyPatch(nameof(StatusEffects.UpdateStatusEffect))]
        [HarmonyPostfix]
        private static void Postfix(StatusEffects __instance, StatusEffect myStatusEffect)
        {
            if (myStatusEffect.statusEffectName == VanillaEffects.Tranquilized)
            {
                __instance.agent.tranqer = myStatusEffect.causingAgent;
            }
        }
    }
}
