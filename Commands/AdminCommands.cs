using ScarletBrazier.Services;
using ScarletCore.Data;
using VampireCommandFramework;
using ScarletCore.Services;
using System.Collections.Generic;

namespace ScarletBrazier.Commands;

[CommandGroup("brazier")]
public static class AdminCommands {
  public static Settings Settings => Plugin.Settings;
  [Command("enable", "Enables free braziers globally", adminOnly: true)]
  public static void Enable(ChatCommandContext ctx) {
    Settings.Set(BrazierService.ENABLE_GLOBALLY, true);
    BrazierService.SetAllBraziersFree();
    ctx.Reply($"~Free Braziers~ have been enabled globally! All braziers now work without needing bones as fuel.".Format());
  }

  [Command("disable", "Disables free braziers globally", adminOnly: true)]
  public static void Disable(ChatCommandContext ctx) {
    Settings.Set(BrazierService.ENABLE_GLOBALLY, false);
    BrazierService.ClearAllBraziers();
    ctx.Reply($"~Free Braziers~ have been disabled globally. Braziers returned to normal operation.".Format());
  }

  [Command("force", "Forces all braziers to be free", adminOnly: true)]
  public static void Force(ChatCommandContext ctx) {
    BrazierService.SetAllBraziersFree();
    ctx.Reply($"All ~Braziers~ have been forced to be free! They now burn indefinitely without fuel.".Format());
  }

  [Command("create", adminOnly: true)]
  public static void SpawnInvisible(ChatCommandContext ctx, string id) {
    var player = ctx.User.GetPlayerData();

    BrazierService.SpawnInvisible(player.Position, id);

    ctx.Reply($"An invisible brazier has been spawned at your location with ID '{id}'.".Format());
  }

  [Command("removeid", "rid", adminOnly: true)]
  public static void RemoveInvisible(ChatCommandContext ctx, string id) {
    var player = ctx.User.GetPlayerData();

    if (BrazierService.Remove(id)) {
      ctx.Reply($"The invisible brazier with ID '{id}' has been removed.".Format());
    } else {
      ctx.Reply($"No invisible brazier with ID '{id}' was found.".Format());
    }
  }

  [Command("remove", "r", adminOnly: true)]
  public static void RemoveClosestInvisible(ChatCommandContext ctx, float range = 2f) {
    var player = ctx.User.GetPlayerData();

    if (BrazierService.Remove(player.Position, range)) {
      ctx.Reply($"All invisible braziers in range have been removed.".Format());
    } else {
      ctx.Reply($"No invisible brazier was found nearby.".Format());
    }
  }

  [Command("showid", adminOnly: true)]
  public static void ShowInvisibleById(ChatCommandContext ctx, string id) {
    if (BrazierService.ShowById(id)) {
      ctx.Reply($"The invisible brazier with ID '{id}' has been made visible.".Format());
    } else {
      ctx.Reply($"No invisible brazier with ID '{id}' was found.".Format());
    }
  }

  [Command("show", adminOnly: true)]
  public static void ShowClosestInvisible(ChatCommandContext ctx, float range = 2f) {
    var player = ctx.User.GetPlayerData();

    if (BrazierService.ShowRange(player.Position, range)) {
      ctx.Reply($"All invisible braziers in range have been made visible.".Format());
    } else {
      ctx.Reply($"No invisible brazier was found nearby.".Format());
    }
  }

  [Command("list", "Lists invisible braziers", adminOnly: true)]
  public static void List(ChatCommandContext ctx) {
    if (BrazierService.InvisibleBraziers.Count == 0) {
      ctx.Reply($"No invisible brazier was found.".Format());
      return;
    }

    var lines = new List<string>();
    foreach (var kv in BrazierService.InvisibleBraziers) {
      var id = kv.Key;
      var brazier = kv.Value;
      if (!brazier.Exists()) continue;
      var pos = brazier.Position();
      var visible = !BuffService.HasBuff(brazier, BrazierService.InvisibleBuff);
      lines.Add($"{id} - {(visible ? "visible" : "hidden")} at X:{pos.x:0.0} Y:{pos.y:0.0} Z:{pos.z:0.0}");
    }

    ctx.Reply(string.Join("\n", lines).Format());
  }

  [Command("hideid", adminOnly: true)]
  public static void HideInvisibleById(ChatCommandContext ctx, string id) {
    if (BrazierService.HideById(id)) {
      ctx.Reply($"The invisible brazier with ID '{id}' has been hidden.".Format());
    } else {
      ctx.Reply($"No invisible brazier with ID '{id}' was found.".Format());
    }
  }

  [Command("hide", adminOnly: true)]
  public static void HideClosestInvisible(ChatCommandContext ctx, float range = 2f) {
    var player = ctx.User.GetPlayerData();

    if (BrazierService.HideRange(player.Position, range)) {
      ctx.Reply($"All invisible braziers in range have been hidden.".Format());
    } else {
      ctx.Reply($"No invisible brazier was found nearby.".Format());
    }
  }

  [Command("goto", adminOnly: true)]
  public static void GotoInvisibleById(ChatCommandContext ctx, string id) {
    var player = ctx.User.GetPlayerData();

    var brazier = BrazierService.GetInvisible(id);
    if (brazier.Exists()) {
      var brazierPosition = brazier.Position();
      TeleportService.TeleportToPosition(player.CharacterEntity, brazierPosition);
      ctx.Reply($"You have been teleported to the invisible brazier with ID '{id}'.".Format());
    } else {
      ctx.Reply($"No invisible brazier with ID '{id}' was found.".Format());
    }
  }
}