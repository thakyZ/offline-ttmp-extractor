using System;
using System.Collections.Generic;
using System.Linq;

using OtterGui;

using Penumbra.String.Classes;
using Penumbra.Util;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;

public sealed partial class Mod {
  public sealed partial class Manager {

    private static void UpdateSubModPositions(Mod mod, int fromGroup) {
      foreach (var (group, groupIdx) in mod._groups.WithIndex().Skip(fromGroup)) {
        foreach (var (o, optionIdx) in group.OfType<SubMod>().WithIndex()) {
          o.SetPosition(groupIdx, optionIdx);
        }
      }
    }

    public void OptionSetFiles(Mod mod, int groupIdx, int optionIdx, Dictionary<Utf8GamePath, FullPath> replacements) {
      var subMod = GetSubMod( mod, groupIdx, optionIdx );
      if (subMod.FileData.SetEquals(replacements)) {
        return;
      }

      subMod.FileData = replacements;
    }

    private static SubMod GetSubMod(Mod mod, int groupIdx, int optionIdx) {
      if (groupIdx == -1 && optionIdx == 0) {
        return mod._default;
      }

      return mod._groups[groupIdx] switch {
        SingleModGroup s => s.OptionData[optionIdx],
        MultiModGroup m => m.PrioritizedOptions[optionIdx].Mod,
        _ => throw new InvalidOperationException(),
      };
    }
  }
}
