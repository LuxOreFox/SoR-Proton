using RogueLibsCore;
using Neutron.Category;
using Neutron.Helper;

namespace Proton.Item;

[ItemCategories(NItemCategories.FunctionalAgent, RogueCategories.Drugs)]
public class CannedLuck : CustomItem, IItemUsable
{
    [RLSetup]
    private static void Setup()
    {
        RogueLibs.CreateCustomItem<CannedLuck>()
            .WithName(new CustomNameInfo()
            {
                English = "Canned Luck", 
                Chinese = "罐装幸运"
            })
            .WithDescription(new CustomNameInfo()
            {
                English = "Some attribute it to a technological marvel, while others maintain it's merely the placebo effect.", 
                Chinese = "一些人相信这是技术的奇迹，而另一些人认为这只是安慰剂效应。"
            })
            .WithSprite(Properties.ItemSprites.CannedLuck)
            .WithUnlock(new ItemUnlock { CharacterCreationCost = 2 });

        RandomTableHelper.AddRandomElement(new RandomTableInfo()
        {
            TableName = "Drugs3",
            TableCategory = "Items",
            ObjectType = "Item"
        }, new Neutron.Helper.RandomElement()
        {
            ElementName = nameof(CannedLuck),
            ElementChance = 3
        });
    }

    public override void SetupDetails()
    {
        Item.itemType = ItemTypes.Consumable;
        Item.initCount = 1;
        Item.itemValue = 60;
        Item.stackable = true;
        Item.statusEffect = StatusEffectHelper.SetEffectWithInfo(VanillaEffects.FeelingLucky, effectInfo);
    }

    public static readonly CreateEffectInfo effectInfo = new()
    {
        IgnoreElectronic = true,
    };

    public bool UseItem()
    {
        Owner.AddEffect(Item.statusEffect);
        gc.audioHandler.Play(Owner, VanillaAudio.UseCocaine);
        Count--;
        return true;
    }
}
