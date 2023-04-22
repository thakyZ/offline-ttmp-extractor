using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Files;
using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;
using OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;
using OfflineTTMPExtractor.Utils;

using Penumbra.Collections;
using Penumbra.Interop.Structs;

using static Penumbra.Interop.CharacterUtility.List;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manager;

public partial class MetaManager : IDisposable, IEnumerable<KeyValuePair<MetaManipulation, IMod>> {
  private readonly Dictionary< MetaManipulation, IMod > _manipulations = new();
  private readonly ModCollection                        _collection;

  public int Count
      => _manipulations.Count;

  public IReadOnlyCollection<MetaManipulation> Manipulations
      => _manipulations.Keys;

  public IEnumerator<KeyValuePair<MetaManipulation, IMod>> GetEnumerator()
      => _manipulations.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

  public MetaManager(ModCollection collection) {
    _collection = collection;
    SetupImcDelegate();
    //if (!App.CharacterUtility.Ready) {
    //  App.CharacterUtility.LoadingFinished += ApplyStoredManipulations;
    //}
  }

  public void Dispose() {
    _manipulations.Clear();
    //App.CharacterUtility.LoadingFinished -= ApplyStoredManipulations;
    DisposeEqp();
    DisposeEqdp();
    DisposeCmp();
    DisposeGmp();
    DisposeEst();
    DisposeImc();
  }

  // Use this when CharacterUtility becomes ready.
  private void ApplyStoredManipulations() {
    //if (!App.CharacterUtility.Ready) {
    //  return;
    //}

    var loaded = 0;
    foreach (var manip in Manipulations) {
      loaded += manip.ManipulationType switch {
        MetaManipulation.Type.Eqp => ApplyMod(manip.Eqp),
        MetaManipulation.Type.Gmp => ApplyMod(manip.Gmp),
        MetaManipulation.Type.Eqdp => ApplyMod(manip.Eqdp),
        MetaManipulation.Type.Est => ApplyMod(manip.Est),
        MetaManipulation.Type.Rsp => ApplyMod(manip.Rsp),
        MetaManipulation.Type.Imc => ApplyMod(manip.Imc),
        MetaManipulation.Type.Unknown => false,
        _ => false,
      }
          ? 1
          : 0;
    }

    /*
     * if (Penumbra.CollectionManager.Default == _collection) {
     *   SetFiles();
     *   Penumbra.ResidentResources.Reload();
     * }
     */

    //App.CharacterUtility.LoadingFinished -= ApplyStoredManipulations;
    Logger.Log.Debug($"{_collection.AnonymizedName}: Loaded {loaded} delayed meta manipulations.");
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
  private static unsafe void SetFile(MetaBaseFile? file, CharacterUtility.Index index) {
    if (file == null) {
      //App.CharacterUtility.ResetResource(index);
    } else {
      //App.CharacterUtility.SetResource(index, (IntPtr)file.Data, file.Length);
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
  private static unsafe Penumbra.Interop.CharacterUtility.List.MetaReverter TemporarilySetFile(MetaBaseFile? file, CharacterUtility.Index index)
      => new MetaReverter(new Penumbra.Interop.CharacterUtility.List(new Penumbra.Interop.CharacterUtility.InternalIndex(0)), 0, 0);
  //file == null
  // ? App.CharacterUtility.TemporarilyResetResource(index)
  // : App.CharacterUtility.TemporarilySetResource(index, (IntPtr)file.Data, file.Length);
}
