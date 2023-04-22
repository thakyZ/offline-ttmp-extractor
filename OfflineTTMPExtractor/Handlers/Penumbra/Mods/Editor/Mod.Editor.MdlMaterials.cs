using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using OfflineTTMPExtractor.Utils;

using OtterGui;

using Penumbra.GameData.Files;
using Penumbra.String.Classes;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;

public partial class Mod {
  public partial class Editor {
    private static readonly Regex MaterialRegex = new(@"/mt_c(?'RaceCode'\d{4})b0001_(?'Suffix'.*?)\.mtrl", RegexOptions.Compiled);
    private readonly        List< ModelMaterialInfo > _modelFiles = new();

    // Find all model files in the mod that contain skin materials.
    public void ScanModels() {
      _modelFiles.Clear();
      foreach (var file in _mdlFiles.Where(f => f.File.Extension == ".mdl")) {
        try {
          var bytes   = File.ReadAllBytes( file.File.FullName );
          var mdlFile = new MdlFile( bytes );
          var materials = mdlFile.Materials.WithIndex().Where( p => MaterialRegex.IsMatch( p.Item1 ) )
                       .Select( p => p.Item2 ).ToArray();
          if (materials.Length > 0) {
            _modelFiles.Add(new ModelMaterialInfo(file.File, mdlFile, materials));
          }
        } catch (Exception e) {
          Logger.Log.Error($"Unexpected error scanning {_mod.Name}'s {file.File.FullName} for materials:\n{e}");
        }
      }
    }

    // A class that collects information about skin materials in a model file and handle changes on them.
    public class ModelMaterialInfo {
      public readonly  FullPath             Path;
      public readonly  MdlFile              File;
      private readonly string[]             _currentMaterials;
      private readonly IReadOnlyList< int > _materialIndices;

      private IEnumerable<string> DefaultMaterials
          => _materialIndices.Select(i => File.Materials[i]);

      public ModelMaterialInfo(FullPath path, MdlFile file, IReadOnlyList<int> indices) {
        Path = path;
        File = file;
        _materialIndices = indices;
        _currentMaterials = DefaultMaterials.ToArray();
      }
    }
  }
}
