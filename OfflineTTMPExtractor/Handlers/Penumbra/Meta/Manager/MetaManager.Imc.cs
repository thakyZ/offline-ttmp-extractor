using System;
using System.Collections.Generic;

using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Files;
using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;
using OfflineTTMPExtractor.Utils;
//using FFXIVClientStructs.FFXIV.Client.System.Resource;
using OtterGui.Filesystem;

using Penumbra.String.Classes;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manager;

public partial class MetaManager {
  private readonly Dictionary<Utf8GamePath, ImcFile> _imcFiles         = new();
  private readonly List<ImcManipulation>             _imcManipulations = new();
  //private static   int                               _imcManagerCount;

  public bool ApplyMod(ImcManipulation manip) {
    if (!manip.Valid) {
      return false;
    }

    _imcManipulations.AddOrReplace(manip);
    var path = manip.GamePath();
    try {
      if (!_imcFiles.TryGetValue(path, out var file)) {
        file = new ImcFile(manip);
      }

      if (!manip.Apply(file)) {
        return false;
      }

      _imcFiles[path] = file;
      var fullPath = CreateImcPath( path );
      if (_collection.HasCache) {
        //_collection.ForceFile(path, fullPath);
      }

      return true;
    } catch (ImcException e) {
      //Penumbra.ValidityChecker.ImcExceptions.Add(e);
      Logger.Log.Error(e.ToString());
    } catch (Exception e) {
      Logger.Log.Error($"Could not apply IMC Manipulation {manip}:\n{e}");
    }

    return false;
  }

  public void DisposeImc() {
    foreach (var file in _imcFiles.Values) {
      file.Dispose();
    }

    _imcFiles.Clear();
    _imcManipulations.Clear();
    RestoreImcDelegate();
  }

  private static unsafe void SetupImcDelegate() {
    /*
     * if (_imcManagerCount++ == 0) {
     *   Penumbra.ResourceLoader.ResourceLoadCustomization += ImcLoadHandler;
     * }
     */
  }

  private static unsafe void RestoreImcDelegate() {
    /*
     * if (--_imcManagerCount == 0) {
     *   Penumbra.ResourceLoader.ResourceLoadCustomization -= ImcLoadHandler;
     * }
     */
  }

  private FullPath CreateImcPath(Utf8GamePath path)
      => new($"|{_collection.Name}_{_collection.ChangeCounter}|{path}");

  /*
   * private static unsafe bool ImcLoadHandler(ByteString split, ByteString path, ResourceManager* resourceManager,
   *     SeFileDescriptor* fileDescriptor, int priority, bool isSync, out byte ret) {
   *    ret = 0;
   *
   *   if (fileDescriptor->ResourceHandle->FileType != ResourceType.Imc) {
   *     return false;
   *   }
   *
   *   Logger.Log.Verbose($"Using ImcLoadHandler for path {path}.");
   *   ret = Penumbra.ResourceLoader.ReadSqPackHook.Original(resourceManager, fileDescriptor, priority, isSync);
   *
   *   var lastUnderscore = split.LastIndexOf( ( byte )'_' );
   *   var name           = lastUnderscore == -1 ? split.ToString() : split.Substring( 0, lastUnderscore ).ToString();
   *   if ((Penumbra.TempMods.CollectionByName(name, out var collection)
   *       || Penumbra.CollectionManager.ByName(name, out collection))
   *   && collection.HasCache
   *   && collection.MetaCache!._imcFiles.TryGetValue(Utf8GamePath.FromSpan(path.Span, out var p) ? p : Utf8GamePath.Empty, out var file)) {
   *     Penumbra.Log.Debug($"Loaded {path} from file and replaced with IMC from collection {collection.AnonymizedName}.");
   *     file.Replace(fileDescriptor->ResourceHandle);
   *   }
   *
   *   return true;
   * }
   */
}
