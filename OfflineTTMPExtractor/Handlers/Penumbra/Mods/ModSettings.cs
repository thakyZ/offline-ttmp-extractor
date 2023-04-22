using System;
using System.Collections.Generic;

using Penumbra.Api.Enums;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;

// Contains the settings for a given mod.
public class ModSettings {
  public List<uint> Settings { get; private init; } = new();
  public int Priority { get; set; }
  public bool Enabled { get; set; }

  // Ensure that a value is valid for a group.
  private static uint FixSetting(IModGroup group, uint value)
      => group.Type switch {
        GroupType.Single => (uint)Math.Min(value, group.Count - 1),
        GroupType.Multi => (uint)(value & ((1ul << group.Count) - 1)),
        _ => value,
      };

  // Add defaulted settings up to the required count.
  private bool AddMissingSettings(Mod mod) {
    var changes = false;
    for (var i = Settings.Count; i < mod.Groups.Count; ++i) {
      Settings.Add(mod.Groups[i].DefaultSettings);
      changes = true;
    }

    return changes;
  }
}
