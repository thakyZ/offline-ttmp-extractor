using System;
using System.Collections.Generic;

using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Files;
using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;

using OtterGui.Filesystem;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manager;

public partial class MetaManager {
  private EstFile? _estFaceFile = null;
  private EstFile? _estHairFile = null;
  private EstFile? _estBodyFile = null;
  private EstFile? _estHeadFile = null;

  private readonly List< EstManipulation > _estManipulations = new();

  public bool ApplyMod(EstManipulation m) {
    _estManipulations.AddOrReplace(m);
    var file = m.Slot switch
        {
          EstManipulation.EstType.Hair => _estHairFile ??= new EstFile(EstManipulation.EstType.Hair),
          EstManipulation.EstType.Face => _estFaceFile ??= new EstFile(EstManipulation.EstType.Face),
          EstManipulation.EstType.Body => _estBodyFile ??= new EstFile(EstManipulation.EstType.Body),
          EstManipulation.EstType.Head => _estHeadFile ??= new EstFile(EstManipulation.EstType.Head),
          _                            => throw new ArgumentOutOfRangeException(),
        };
    return m.Apply(file);
  }

  public void DisposeEst() {
    _estFaceFile?.Dispose();
    _estHairFile?.Dispose();
    _estBodyFile?.Dispose();
    _estHeadFile?.Dispose();
    _estFaceFile = null;
    _estHairFile = null;
    _estBodyFile = null;
    _estHeadFile = null;
    _estManipulations.Clear();
  }
}
