using RogueLibsCore;
using Neutron.Category;
using Neutron.Helper;

namespace Proton.Item;

[ItemCategories(NItemCategories.Resource, RogueCategories.Technology)]
public class AugmentationCanisterProton : CustomItem
{
    [RLSetup]
    private static void Setup()
    {
        RogueLibs.CreateCustomItem<AugmentationCanisterProton>()
            .WithName(new CustomNameInfo()
            {
                English = "Augmentation Canister", 
                Chinese = "强化罐"
            })
            .WithDescription(new CustomNameInfo()
            {
                English = "Daedalus's Gift. " 
                + "This latest masterpiece of body enhancement technology " 
                + "allows the Augmentation Booth to enhance your attributes and abilities directly.", 
                Chinese = "代达罗斯的赠礼。身体增强技术的这一最新杰作，能够让强化亭直接增强你的属性和能力。"
            })
            .WithSprite(Properties.ItemSprites.AugmentationCanisterProton, 60)
            .WithUnlock(new ItemUnlock { CharacterCreationCost = 4 });

        RandomTableHelper.AddRandomElement(new RandomTableInfo()
        {
            TableName = "ShopkeeperSpecialInv",
            TableCategory = "Items",
            ObjectType = "Item"
        }, new Neutron.Helper.RandomElement()
        {
            ElementName = nameof(AugmentationCanisterProton),
            ElementChance = 2
        });
    }

    public override void SetupDetails()
    {
        Item.itemType = ItemTypes.Tool;
        Item.initCount = 1;
        Item.itemValue = 200;
        Item.stackable = true;
    }
}
