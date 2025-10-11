using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using RogueLibsCore;
using Neutron;

namespace Proton;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency(RogueLibs.GUID, RogueLibs.CompiledVersion)]
[BepInDependency(NeutronPlugin.PluginGuid, NeutronPlugin.PluginVersion)]
public class ProtonPlugin : BaseUnityPlugin
{
    public const string PluginGuid = "luxorefox.streetsofrogue.proton";
    public const string PluginName = "Proton";
    public const string PluginVersion = "0.1.0";

    public const string PluginFullName = "Proton: Vanilla Expansion";

    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        try
        {
            var harmony = new Harmony(PluginGuid);
            harmony.PatchAll();
            RogueLibs.LoadFromAssembly();
            Logger.LogInfo("⚛️Initialization complete✅");
        }
        catch (Exception e)
        {
            Logger.LogError($"⚛️Initialization failed❌：{e}");
        }
    }
}
