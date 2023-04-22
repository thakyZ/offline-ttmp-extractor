using System.Collections.Generic;

using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Files;
using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;

using OtterGui.Filesystem;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manager;

public partial class MetaManager {
  private          CmpFile?                _cmpFile          = null;
  private readonly List< RspManipulation > _cmpManipulations = new();

  public bool ApplyMod(RspManipulation manip) {
    _cmpManipulations.AddOrReplace(manip);
    _cmpFile ??= new CmpFile();
    return manip.Apply(_cmpFile);
  }

  public void DisposeCmp() {
    _cmpFile?.Dispose();
    _cmpFile = null;
    _cmpManipulations.Clear();
  }
}
