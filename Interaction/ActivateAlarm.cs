using RogueLibsCore;
using Neutron.Extension;

namespace Proton.Interaction;

public static class ActivateAlarm
{
    [RLSetup]
    private static void Setup()
    {
        RogueInteractions.CreateProvider<SecurityCam>(static h =>
        {
            if (!h.Helper.interactingFar) return;

            if (!CanProvide(h.Object) || !CanReceive(h.Agent)) return;

            h.AddButton(BT_ActivateAlarm, static m =>
            {
                m.gc.spawnerMain.SpawnNoise(m.Object.tr.position, AlarmVolume, m.Object, "Normal");
                m.Object.Vocalize(VanillaAudio.Hack);
                m.Object.SpawnParticleEffect("Hack", m.Object.tr.position);
                m.gc.spawnerMain.SpawnStateIndicator(m.Object, "HighVolume");
            });
        });

        RogueInteractions.CreateProvider<Turret>(static h =>
        {
            if (!h.Helper.interactingFar) return;

            if (!CanProvide(h.Object) || !CanReceive(h.Agent)) return;

            h.AddButton(BT_ActivateAlarm, static m =>
            {
                m.gc.spawnerMain.SpawnNoise(m.Object.tr.position, AlarmVolume, m.Object, "Normal");
                m.Object.Vocalize(VanillaAudio.Hack);
                m.Object.SpawnParticleEffect("Hack", m.Object.tr.position);
                m.gc.spawnerMain.SpawnStateIndicator(m.Object, "HighVolume");
            });
        });

        RogueLibs.CreateCustomName(BT_ActivateAlarm, NameTypes.Interface, new CustomNameInfo()
        {
            English = "Activate the Alarm", 
            Chinese = "激活警报"
        });
    }

    public static bool CanProvide(PlayfieldObject provider)
    {
        return provider.functional;
    }

    public static bool CanReceive(Agent receiver)
    {
        return receiver.HasTrait(VanillaTraits.TechExpert);
    }

    public const string BT_ActivateAlarm = "BT_ActivateAlarm";

    public static float AlarmVolume { get; private set; } = 3f;
}
