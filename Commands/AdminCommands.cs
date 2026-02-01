using ScarletBrazier.Services;
using ScarletCore.Data;
using ScarletCore.Services;
using System.Collections.Generic;
using ScarletCore.Commanding;
using ScarletCore.Localization;

namespace ScarletBrazier.Commands;

[CommandGroup("brazier", Language.English, adminOnly: true)]
public static class AdminCommands {
  public static Settings Settings => Plugin.Settings;
  [Command("enable", Language.English)]
  public static void Enable(CommandContext ctx) {
    Settings.Set(BrazierService.ENABLE_GLOBALLY, true);
    BrazierService.SetAllBraziersFree();
    ctx.Reply($"~Free Braziers~ have been enabled globally! All braziers now work without needing bones as fuel.".Format());
  }

  [Command("disable", Language.English)]
  public static void Disable(CommandContext ctx) {
    Settings.Set(BrazierService.ENABLE_GLOBALLY, false);
    BrazierService.DisableAllBraziers();
    ctx.Reply($"~Free Braziers~ have been disabled globally. Braziers returned to normal operation.".Format());
  }

  [Command("force", Language.English)]
  public static void Force(CommandContext ctx) {
    BrazierService.SetAllBraziersFree();
    ctx.Reply($"All ~Braziers~ have been forced to be free! They now burn indefinitely without fuel.".Format());
  }

  [Command("create", Language.English)]
  public static void SpawnInvisible(CommandContext ctx, string id) {
    var player = ctx.Sender;

    BrazierService.SpawnInvisible(player.Position, id);

    ctx.Reply($"An invisible brazier has been spawned at your location with ID '{id}'.".Format());
  }

  [Command("removeid", Language.English)]
  public static void RemoveInvisible(CommandContext ctx, string id) {
    var player = ctx.Sender;

    if (BrazierService.Remove(id)) {
      ctx.Reply($"The invisible brazier with ID '{id}' has been removed.".Format());
    } else {
      ctx.Reply($"No invisible brazier with ID '{id}' was found.".Format());
    }
  }

  [Command("remove", Language.English)]
  public static void RemoveClosestInvisible(CommandContext ctx, float range = 2f) {
    var player = ctx.Sender;

    if (BrazierService.Remove(player.Position, range)) {
      ctx.Reply($"All invisible braziers in range have been removed.".Format());
    } else {
      ctx.Reply($"No invisible brazier was found nearby.".Format());
    }
  }

  [Command("showid", Language.English)]
  public static void ShowInvisibleById(CommandContext ctx, string id) {
    if (BrazierService.ShowById(id)) {
      ctx.Reply($"The invisible brazier with ID '{id}' has been made visible.".Format());
    } else {
      ctx.Reply($"No invisible brazier with ID '{id}' was found.".Format());
    }
  }

  [Command("show", Language.English)]
  public static void ShowClosestInvisible(CommandContext ctx, float range = 2f) {
    var player = ctx.Sender;

    if (BrazierService.ShowRange(player.Position, range)) {
      ctx.Reply($"All invisible braziers in range have been made visible.".Format());
    } else {
      ctx.Reply($"No invisible brazier was found nearby.".Format());
    }
  }

  [Command("list", Language.English)]
  public static void List(CommandContext ctx) {
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

  [Command("hideid", Language.English)]
  public static void HideInvisibleById(CommandContext ctx, string id) {
    if (BrazierService.HideById(id)) {
      ctx.Reply($"The invisible brazier with ID '{id}' has been hidden.".Format());
    } else {
      ctx.Reply($"No invisible brazier with ID '{id}' was found.".Format());
    }
  }

  [Command("hide", Language.English)]
  public static void HideClosestInvisible(CommandContext ctx, float range = 2f) {
    var player = ctx.Sender;

    if (BrazierService.HideRange(player.Position, range)) {
      ctx.Reply($"All invisible braziers in range have been hidden.".Format());
    } else {
      ctx.Reply($"No invisible brazier was found nearby.".Format());
    }
  }

  [Command("goto", Language.English)]
  public static void GotoInvisibleById(CommandContext ctx, string id) {
    var player = ctx.Sender;

    var brazier = BrazierService.GetInvisible(id);
    if (brazier.Exists()) {
      var brazierPosition = brazier.Position();
      TeleportService.TeleportToPosition(player.CharacterEntity, brazierPosition);
      ctx.Reply($"You have been teleported to the invisible brazier with ID '{id}'.".Format());
    } else {
      ctx.Reply($"No invisible brazier with ID '{id}' was found.".Format());
    }
  }

  [Command("clearall", Language.English)]
  public static void ClearAllInvisible(CommandContext ctx) {
    var count = BrazierService.InvisibleBraziers.Count;
    BrazierService.ClearAllBraziers();
    ctx.Reply($"All {count} invisible braziers have been removed.".Format());
  }
}