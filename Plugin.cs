using System;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using ProjectM;
using ScarletCore;
using ScarletCore.Data;
using ScarletCore.Events;
using ScarletCore.Services;
using ScarletCore.Systems;
using ScarletCore.Utils;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
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
  public const string ENABLE_GLOBALLY = "EnableGlobally";

  public override void Load() {
    Instance = this;
    LogInstance = Log;

    Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

    _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
    _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

    Settings = new Settings(MyPluginInfo.PLUGIN_GUID, Instance);

    LoadSettings();

    EventManager.OnInitialize += (_, _) => {
      if (Settings.Get<bool>(ENABLE_GLOBALLY)) {
        SetAllBraziersFree();
      } else {
        ClearAllBraziers();
      }
    };

    CommandRegistry.RegisterAll();
  }

  public override bool Unload() {
    _harmony?.UnpatchSelf();
    CommandRegistry.UnregisterAssembly();
    return true;
  }

  public static void ReloadSettings() {
    Settings.Dispose();
    LoadSettings();
  }

  public static void LoadSettings() {
    Settings.Section("General").Add(ENABLE_GLOBALLY, true, "If true, all braziers will work for free without needing bones as fuel.");
  }

  public static void SetAllBraziersFree() {
    var query = GameSystems.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Bonfire>()).ToEntityArray(Allocator.Temp);

    foreach (var brazier in query) {
      if (brazier.IsNull() || !brazier.Exists() || !brazier.Has<Bonfire>()) continue;

      brazier.With((ref Bonfire bonfire) => {
        bonfire.IsActive = true;
        bonfire.BurnTime = 31536000f; // Set burn time to 1 year to simulate infinite burning
      });
    }
  }

  public static void ClearAllBraziers() {
    var braziers = GameSystems.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Bonfire>()).ToEntityArray(Allocator.Temp);
    try {
      foreach (var brazier in braziers) {
        if (brazier.IsNull() || !brazier.Exists() || !brazier.Has<Bonfire>()) continue;

        brazier.With((ref Bonfire bonfire) => {
          bonfire.IsActive = false;
          bonfire.BurnTime = 60f; // Set burn time back to 1 minute to clear the effect
        });
      }
    } catch (Exception ex) {
      LogInstance.LogError($"Error while clearing braziers: {ex.Message}");
      LogInstance.LogDebug(ex.StackTrace);
    } finally {
      braziers.Dispose();
    }
  }

  [CommandGroup("brazier")]
  public class BrazierCommands {
    [Command("enable", "Enables free braziers globally", adminOnly: true)]
    public static void Enable(ChatCommandContext ctx) {
      Settings.Set(ENABLE_GLOBALLY, true);
      SetAllBraziersFree();
      ctx.Reply($"~Free Braziers~ have been enabled globally! All braziers now work without needing bones as fuel.".Format());
    }

    [Command("disable", "Disables free braziers globally", adminOnly: true)]
    public static void Disable(ChatCommandContext ctx) {
      Settings.Set(ENABLE_GLOBALLY, false);
      ClearAllBraziers();
      ctx.Reply($"~Free Braziers~ have been disabled globally. Braziers returned to normal operation.".Format());
    }

    [Command("force", "Forces all braziers to be free", adminOnly: true)]
    public static void Force(ChatCommandContext ctx) {
      SetAllBraziersFree();
      ctx.Reply($"All ~Braziers~ have been forced to be free! They now burn indefinitely without fuel.".Format());
    }

    [Command("spawninvisible", "si", adminOnly: true)]
    public static void SpawnInvisible(ChatCommandContext ctx) {
      if (!PlayerService.TryGetById(ctx.User.PlatformId, out var player)) {
        ctx.Reply("Player not found.".Format());
        return;
      }

      var position = player.Position;
      var brazier = UnitSpawnerService.ImmediateSpawn(new(1756900697), position, 0f, 0f);

      brazier.With((ref Bonfire bonfire) => {
        bonfire.IsActive = true;
        bonfire.BurnTime = 31536000f;
      });

      brazier.With((ref Interactable interactable) => {
        interactable.Disabled = true;
      });

      BuffService.TryApplyBuff(brazier, new(1880224358), -1f);
      BuffService.TryApplyBuff(brazier, new(227784838), -1f);

      ctx.Reply("Spawned an invisible brazier at your location.".Format());
    }
  }
}

// Handle new Brazier placing
[HarmonyPatch(typeof(SetTeamOnSpawnSystem), nameof(SetTeamOnSpawnSystem.OnUpdate))]
public static class SetTeamOnSpawnSystemPatch {
  [HarmonyPostfix]
  public static void Postfix(SetTeamOnSpawnSystem __instance) {
    if (Plugin.Settings.Get<bool>(Plugin.ENABLE_GLOBALLY)) return;

    var query = __instance.__query_57018132_0.ToEntityArray(Allocator.Temp);

    foreach (var entity in query) {
      if (entity.IsNull() || !entity.Exists() || !entity.Has<Bonfire>() || !entity.Has<PrefabGUID>() || entity.Read<PrefabGUID>().GuidHash != 1756900697) continue;
      entity.With((ref Bonfire bonfire) => {
        bonfire.IsActive = true;
        bonfire.BurnTime = 31536000f;
      });
    }
  }
}