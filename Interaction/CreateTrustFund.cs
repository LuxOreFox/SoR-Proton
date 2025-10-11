using RogueLibsCore;
using Neutron.Extension;

namespace Proton.Interaction;

public static class CreateTrustFund
{
    [RLSetup]
    private static void Setup()
    {
        RogueInteractions.CreateProvider<Agent>(static h =>
        {
            if (!CanProvide(h.Object) || !CanReceive(h.Agent)) return;

            h.AddButton(BT_CreateTrustFund, $" - ${TrustCreationCost}", static m =>
            {
                InvItem money = m.Agent.inventory.money;
                if (money.invItemCount >= TrustCreationCost)
                {
                    m.Agent.inventory.SubtractFromItemCount(money, TrustCreationCost);
                    m.Agent.AddTrait(VanillaTraits.TrustFunder);
                    m.Object.Talk(DL_CreateTrustFund, VanillaAudio.UseDeliveryApp);
                }
                else
                {
                    m.Agent.Talk("NA_NeedCash");
                }
                m.StopInteraction();
            });
        });

        RogueLibs.CreateCustomName(BT_CreateTrustFund, NameTypes.Interface, new CustomNameInfo()
        {
            English = "Create Trust Fund", 
            Chinese = "建立信托基金"
        });
        RogueLibs.CreateCustomName(DL_CreateTrustFund, NameTypes.Dialogue, new CustomNameInfo()
        {
            English = "Congratulations! You are now the holder of a trust fund.",
            Chinese = "恭喜！您已成为信托基金的持有人。"
        });
    }

    public static bool CanProvide(Agent provider)
    {
        return provider.agentName == VanillaAgents.Clerk && provider.startingChunkRealDescription == "Bank" && provider.ownerID != 0;
    }

    public static bool CanReceive(Agent receiver)
    {
        return !receiver.HasTrait(VanillaTraits.TrustFunder) && !receiver.HasTrait(VanillaTraits.TrustFunder2);
    }

    public const string BT_CreateTrustFund = "BT_CreateTrustFund";

    public const string DL_CreateTrustFund = "DL_CreateTrustFund";

    public static int TrustCreationCost { get; private set; } = 300;
}
