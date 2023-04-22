using System.Collections.Generic;

using Penumbra.String.Classes;
using Penumbra.Util;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;

public partial class Mod {
  public partial class Editor {
    private SubMod _subMod;

    public readonly Dictionary<Utf8GamePath, FullPath> CurrentSwaps = new();

    public void SetSubMod(ISubMod? subMod) {
      _subMod = subMod as SubMod ?? _mod._default;
      UpdateFiles();
      RevertSwaps();
      RevertManipulations();
    }

    public void RevertSwaps() {
      CurrentSwaps.SetTo(_subMod.FileSwaps);
    }
  }
}
