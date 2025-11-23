using HarmonyLib;
using ProjectM;
using ScarletBrazier.Services;
using Stunlock.Core;
using Unity.Collections;

namespace ScarletBrazier.Patches;

[HarmonyPatch(typeof(SetTeamOnSpawnSystem), nameof(SetTeamOnSpawnSystem.OnUpdate))]
public static class SetTeamOnSpawnSystemPatch {
  [HarmonyPostfix]
  public static void Postfix(SetTeamOnSpawnSystem __instance) {
    if (Plugin.Settings.Get<bool>(BrazierService.ENABLE_GLOBALLY)) return;

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