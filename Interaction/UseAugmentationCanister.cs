using System.Collections.Generic;
using System.Linq;
using RogueLibsCore;
using Neutron.Extension;
using Neutron.Helper;
using Neutron.Other;
using Proton.Item;

namespace Proton.Interaction;

public static class UseAugmentationCanister
{
    [RLSetup]
    private  static void Setup()
    {
        RogueInteractions.CreateProvider<AugmentationBooth>(static h =>
        {
            if (h.Helper.interactingFar) return;

            if (!CanAllow(h.gc) || !CanProvide(h.Object) || !CanReceive(h.Agent)) return;

            string extraText = $" -1 {h.gc.nameDB.GetName(Unit_Canister, NameTypes.Interface)}";

            h.AddButton(BT_UpgradeRandomAttribute, extraText, static m =>
            {
                var upgradable = new List<AgentAttributes>()
                    {
                        AgentAttributes.Endurance,
                        AgentAttributes.Speed,
                        AgentAttributes.Strength,
                        AgentAttributes.Accuracy
                    }.Where(attr => AttributeCanBeUpgraded(attr, m.Agent)).ToList();

                if (upgradable.Count == 0)
                {
                    m.Agent.Talk(DL_NoUpgradableAttribute);
                    return;
                }

                int index = UnityEngine.Random.Range(0, upgradable.Count);
                AgentAttributes selected = upgradable[index];
                m.Agent.AdjustAttribute(selected, 1);

                string name = m.gc.nameDB.GetName(selected.ToString(), NameTypes.Interface);
                m.Object.TalkSimple($"{name} +1", VanillaAudio.AddTrait);
                m.Agent.inventory.SubtractFromItemCount(m.Agent.inventory.FindItem(nameof(AugmentationCanisterProton)), 1);
            });

            h.AddButton(BT_GetRandomExclusiveTrait, extraText, static m =>
            {
                if (m.Agent.oma.superSpecialAbility)
                {
                    m.Agent.Talk(DL_HaveSuperSpecialAbility);
                    return;
                }

                IEnumerable<string> collected = Enumerable.Empty<string>();

                if (!string.IsNullOrEmpty(m.Agent.specialAbility) 
                && UnlockHelper.SpecialAbilityTrait.TryGetValue(m.Agent.specialAbility, out var specialAbilityTraits))
                {
                    collected = collected.Union(specialAbilityTraits);
                }

                foreach (var trait in m.Agent.statusEffects.TraitList)
                {
                    if (UnlockHelper.TraitLeadingTrait.TryGetValue(trait.traitName, out var traitLeadingTraits))
                    {
                        collected = collected.Union(traitLeadingTraits);
                    }
                }

                foreach (var item in m.Agent.inventory.InvItemList)
                {
                    if (item.invItemName != null && UnlockHelper.ItemLeadingTrait.TryGetValue(item.invItemName, out var itemLeadingTraits))
                    {
                        collected = collected.Union(itemLeadingTraits);
                    }
                }

                if (!string.IsNullOrEmpty(m.Agent.bigQuest) 
                && UnlockHelper.BigQuestLeadingTrait.TryGetValue(m.Agent.bigQuest, out var bigQuestLeadingTraits))
                {
                    collected = collected.Union(bigQuestLeadingTraits);
                }

                var available = collected.Where(tra => TraitCanBeGot(tra, m.Agent)).ToList();
                if (available.Count > 0)
                {
                    int index = UnityEngine.Random.Range(0, available.Count);
                    string selected = available[index];
                    m.Agent.AddTrait(selected);

                    string name = m.gc.nameDB.GetName(selected, NameTypes.StatusEffect);
                    m.Object.TalkSimple(name, VanillaAudio.AddTrait);
                    m.Agent.inventory.SubtractFromItemCount(m.Agent.inventory.FindItem(nameof(AugmentationCanisterProton)), 1);
                    return;
                }

                m.Agent.Talk(DL_NoAvailableTrait);
            });
        });

        RogueLibs.CreateCustomName(Unit_Canister, NameTypes.Interface, new CustomNameInfo()
        {
            English = "Canister",
            Chinese = "罐"
        });
        RogueLibs.CreateCustomName(BT_UpgradeRandomAttribute, NameTypes.Interface, new CustomNameInfo()
        {
            English = "Upgrade a Random Attribute", 
            Chinese = "升级随机属性"
        });
        RogueLibs.CreateCustomName(BT_GetRandomExclusiveTrait, NameTypes.Interface, new CustomNameInfo()
        {
            English = "Get a Random Exclusive Trait", 
            Chinese = "获得随机专属特性"
        });
        RogueLibs.CreateCustomName(DL_NoUpgradableAttribute, NameTypes.Dialogue, new CustomNameInfo()
        {
            English = "No attributes upgradable.",
            Chinese = "没有可以升级的属性。"
        });
        RogueLibs.CreateCustomName(DL_HaveSuperSpecialAbility, NameTypes.Dialogue, new CustomNameInfo()
        {
            English = "I already have the Super Special Ability.",
            Chinese = "我已经有超级特殊能力了。"
        });
        RogueLibs.CreateCustomName(DL_NoAvailableTrait, NameTypes.Dialogue, new CustomNameInfo()
        {
            English = "No traits available.",
            Chinese = "没有可以获得的特性。"
        });
    }

    public static bool CanAllow(GameController gc)
    {
        return !gc.challenges.Contains(VanillaMutators.NewCharacterEveryLevel.Replace(" ", ""));
    }

    public static bool CanProvide(AugmentationBooth provider)
    {
        return provider.functional;
    }

    public static bool CanReceive(Agent receiver)
    {
        return receiver.inventory.HasItem(nameof(AugmentationCanisterProton)) && !receiver.possessing && !receiver.mechFilled;
    }

    private static bool AttributeCanBeUpgraded(AgentAttributes attribute, Agent agent)
    {
        return agent.GetAttribute(attribute) < 4 && !agent.AttributeIsLocked_Temp(attribute);
    }

    private static bool TraitCanBeGot(string traitName, Agent agent)
    {
        return !agent.HasTrait(traitName) && !UnlockHelper.UpgradedTraits.Contains(traitName);
    }

    public const string Unit_Canister = "Unit_Canister";

    public const string BT_UpgradeRandomAttribute = "BT_UpgradeRandomAttribute";
    public const string BT_GetRandomExclusiveTrait = "BT_GetRandomExclusiveTrait";

    public const string DL_NoUpgradableAttribute = "DL_NoUpgradableAttribute";
    public const string DL_HaveSuperSpecialAbility = "DL_HaveSuperSpecialAbility";
    public const string DL_NoAvailableTrait = "DL_NoAvailableTrait";
}
