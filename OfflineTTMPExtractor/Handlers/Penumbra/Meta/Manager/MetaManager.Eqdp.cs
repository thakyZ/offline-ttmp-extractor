using System;
using System.Collections.Generic;

using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Files;
using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;

using OtterGui.Filesystem;

using Penumbra.GameData.Enums;
using Penumbra.Interop.Structs;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manager;

public partial class MetaManager {
  private readonly ExpandedEqdpFile?[] _eqdpFiles = new ExpandedEqdpFile[CharacterUtility.EqdpIndices.Length]; // TODO: female Hrothgar

  private readonly List< EqdpManipulation > _eqdpManipulations = new();

  public bool ApplyMod(EqdpManipulation manip) {
    _eqdpManipulations.AddOrReplace(manip);
    var file = _eqdpFiles[ Array.IndexOf( CharacterUtility.EqdpIndices, manip.FileIndex() ) ] ??=
            new ExpandedEqdpFile( Names.CombinedRace( manip.Gender, manip.Race ), manip.Slot.IsAccessory() ); // TODO: female Hrothgar
    return manip.Apply(file);
  }

  public void DisposeEqdp() {
    for (var i = 0; i < _eqdpFiles.Length; ++i) {
      _eqdpFiles[i]?.Dispose();
      _eqdpFiles[i] = null;
    }

    _eqdpManipulations.Clear();
  }
}
