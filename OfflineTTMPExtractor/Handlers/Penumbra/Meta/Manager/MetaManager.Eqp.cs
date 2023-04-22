using System.Collections.Generic;

using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Files;
using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;

using OtterGui.Filesystem;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manager;

public partial class MetaManager {
  private          ExpandedEqpFile?        _eqpFile          = null;
  private readonly List< EqpManipulation > _eqpManipulations = new();

  public bool ApplyMod(EqpManipulation manip) {
    _eqpManipulations.AddOrReplace(manip);
    _eqpFile ??= new ExpandedEqpFile();
    return manip.Apply(_eqpFile);
  }

  public void DisposeEqp() {
    _eqpFile?.Dispose();
    _eqpFile = null;
    _eqpManipulations.Clear();
  }
}
