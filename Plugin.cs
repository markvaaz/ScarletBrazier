using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using ScarletBrazier.Services;
using ScarletCore.Data;
using ScarletCore.Events;
using ScarletCore.Systems;
using VampireCommandFramework;

namespace ScarletBrazier;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("markvaaz.ScarletCore")]
[BepInDependency("gg.deca.VampireCommandFramework")]
public class Plugin : BasePlugin {
  static Harmony _harmony;
  public static Harmony Harmony => _harmony;
  public static Plugin Instance { get; private set; }
  public static ManualLogSource LogInstance { get; private set; }
  public static Settings Settings { get; private set; }
  public static Database Database { get; private set; }

  public override void Load() {
    Instance = this;
    LogInstance = Log;

    Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

    _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
    _harmony.PatchAll(Assembly.GetExecutingAssembly());

    Settings = new Settings(MyPluginInfo.PLUGIN_GUID, Instance);

    LoadSettings();

    CommandRegistry.RegisterAll();

    GameSystems.OnInitialize(BrazierService.Initialize);
  }

  public override bool Unload() {
    _harmony?.UnpatchSelf();
    CommandRegistry.UnregisterAssembly();
    EventManager.UnregisterAssembly(Assembly.GetExecutingAssembly());
    ActionScheduler.UnregisterAssembly(Assembly.GetExecutingAssembly());
    return true;
  }

  public static void LoadSettings() {
    Settings.Section("General").Add(BrazierService.ENABLE_GLOBALLY, true, "If true, all braziers will work for free without needing bones as fuel.");
  }
}