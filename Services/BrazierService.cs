using System;
using System.Collections.Generic;
using ProjectM;
using ScarletCore.Services;
using ScarletCore.Systems;
using Stunlock.Core;
using Unity.Entities;
using Unity.Mathematics;

namespace ScarletBrazier.Services;

public static class BrazierService {
  public const string ENABLE_GLOBALLY = "EnableGlobally";
  public static readonly PrefabGUID BrazierPrefab = new(1756900697);
  public static readonly PrefabGUID BonePrefab = new(1821405450);
  public static readonly PrefabGUID InvisibleBuff = new(1880224358);
  public static readonly PrefabGUID ImmaterialBuff = new(1360141727);
  public const float HeightOffset = 100f;
  public const string BrazierIdPrefix = "SBZ_";
  public static Dictionary<string, Entity> InvisibleBraziers = new();

  public static void Initialize() {
    LoadInvisibleBraziers();
    if (Plugin.Settings.Get<bool>(BrazierService.ENABLE_GLOBALLY)) {
      SetAllBraziersFree();
    } else {
      ClearAllBraziers();
    }
  }

  public static void SpawnInvisible(float3 position, string brazierId) {
    var id = $"{BrazierIdPrefix}{brazierId}";
    var brazier = SpawnerService.ImmediateSpawn(BrazierPrefab, new(position.x, position.y - HeightOffset, position.z));

    brazier.SetId(id);

    Hide(brazier);

    brazier.With((ref Bonfire bonfire) => {
      bonfire.IsActive = true;
      bonfire.BurnTime = 31536000f;
      bonfire.Strength = 1f;
    });

    ActionScheduler.DelayedFrames(() => {
      InventoryService.AddItem(brazier, BonePrefab, 100);
    }, 5);

    InvisibleBraziers.Add(brazierId, brazier);
  }

  public static void Show(Entity brazier) {
    brazier.With((ref Interactable interactable) => {
      interactable.Disabled = false;
    });
    var position = brazier.Position();
    TeleportService.TeleportToPosition(brazier, new float3(position.x, position.y + HeightOffset, position.z));
    BuffService.TryRemoveBuff(brazier, InvisibleBuff);
    BuffService.TryRemoveBuff(brazier, ImmaterialBuff);
  }

  public static void Hide(Entity brazier) {
    brazier.With((ref Interactable interactable) => {
      interactable.Disabled = true;
    });
    var position = brazier.Position();
    TeleportService.TeleportToPosition(brazier, new float3(position.x, position.y - HeightOffset, position.z));
    BuffService.TryApplyBuff(brazier, InvisibleBuff, -1f);
    BuffService.TryApplyBuff(brazier, ImmaterialBuff, -1f);
  }

  public static bool ShowRange(float3 position, float range) {
    var any = false;
    foreach (var brazier in InvisibleBraziers.Values) {
      var brazierPosition = brazier.Position();
      if (math.distance(brazierPosition.xz, position.xz) > range) continue;
      Show(brazier);
      any = true;
    }

    return any;
  }

  public static bool ShowById(string brazierId) {
    if (InvisibleBraziers.TryGetValue(brazierId, out var brazier)) {
      Show(brazier);
      return true;
    }

    return false;
  }

  public static bool HideRange(float3 position, float range) {
    var any = false;
    foreach (var brazier in InvisibleBraziers.Values) {
      var brazierPosition = brazier.Position();
      if (math.distance(brazierPosition.xz, position.xz) < range) {
        Hide(brazier);
        any = true;
      }
    }

    return any;
  }

  public static bool HideById(string brazierId) {
    if (InvisibleBraziers.TryGetValue(brazierId, out var brazier)) {
      Hide(brazier);
      return true;
    }

    return false;
  }

  public static Entity GetInvisible(float3 position, float range) {
    foreach (var brazier in InvisibleBraziers.Values) {
      var brazierPosition = brazier.Position();
      if (math.distance(brazierPosition.xz, position.xz) < range) {
        return brazier;
      }
    }

    return Entity.Null;
  }

  public static Entity GetInvisible(string brazierId) {
    if (InvisibleBraziers.TryGetValue(brazierId, out var brazier)) {
      return brazier;
    }

    return Entity.Null;
  }

  public static bool Remove(float3 position, float range) {
    var toRemove = new List<string>();

    foreach (var kv in InvisibleBraziers) {
      var brazier = kv.Value;
      var brazierPosition = brazier.Position();
      if (math.distance(brazierPosition.xz, position.xz) < range) {
        toRemove.Add(kv.Key);
      }
    }

    if (toRemove.Count == 0) return false;

    foreach (var id in toRemove) {
      if (InvisibleBraziers.TryGetValue(id, out var brazier)) {
        InvisibleBraziers.Remove(id);
        brazier.Destroy();
      }
    }

    return true;
  }

  public static bool Remove(string brazierId) {
    if (!InvisibleBraziers.ContainsKey(brazierId)) return false;

    var brazier = InvisibleBraziers[brazierId];
    InvisibleBraziers.Remove(brazierId);

    brazier.Destroy();
    return true;
  }

  public static void LoadInvisibleBraziers() {
    var query = EntityLookupService.Query(EntityQueryOptions.IncludeDisabled, typeof(Bonfire));

    try {

      foreach (var brazier in query) {
        if (!brazier.Exists() || !brazier.Has<Bonfire>()) continue;

        if (brazier.IdStartsWith(BrazierIdPrefix)) {
          var brazierId = brazier.GetId();
          if (!InvisibleBraziers.ContainsKey(brazierId)) {
            InvisibleBraziers.Add(brazierId, brazier);
          }
        }
      }
    } finally {
      query.Dispose();
    }
  }

  public static void SetAllBraziersFree() {
    var query = EntityLookupService.Query(EntityQueryOptions.IncludeDisabled, typeof(Bonfire));

    try {
      foreach (var brazier in query) {
        if (!brazier.Exists() || !brazier.Has<Bonfire>()) continue;

        brazier.With((ref Bonfire bonfire) => {
          bonfire.IsActive = true;
          bonfire.BurnTime = 31536000f; // Set burn time to 1 year to simulate infinite burning
        });
      }
    } finally {
      query.Dispose();
    }
  }

  public static void ClearAllBraziers() {
    var query = EntityLookupService.Query(EntityQueryOptions.IncludeDisabled, typeof(Bonfire));

    try {
      foreach (var brazier in query) {
        if (!brazier.Exists() || !brazier.Has<Bonfire>()) continue;

        brazier.With((ref Bonfire bonfire) => {
          bonfire.IsActive = false;
          bonfire.BurnTime = 60f; // Set burn time back to 1 minute to clear the effect
        });
      }
    } catch (Exception ex) {
      Log.Error($"Error while clearing braziers: {ex.Message}");
      Log.Error(ex.StackTrace);
    } finally {
      query.Dispose();
    }
  }
}