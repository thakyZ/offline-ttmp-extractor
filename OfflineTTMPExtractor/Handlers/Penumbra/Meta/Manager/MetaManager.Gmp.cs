using System.Collections.Generic;

using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Files;
using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;

using OtterGui.Filesystem;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manager;

public partial class MetaManager {
  private          ExpandedGmpFile?        _gmpFile          = null;
  private readonly List< GmpManipulation > _gmpManipulations = new();

  public bool ApplyMod(GmpManipulation manip) {
    _gmpManipulations.AddOrReplace(manip);
    _gmpFile ??= new ExpandedGmpFile();
    return manip.Apply(_gmpFile);
  }

  public void DisposeGmp() {
    _gmpFile?.Dispose();
    _gmpFile = null;
    _gmpManipulations.Clear();
  }
}
