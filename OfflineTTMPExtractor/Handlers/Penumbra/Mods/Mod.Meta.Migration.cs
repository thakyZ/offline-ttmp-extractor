using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OfflineTTMPExtractor.Utils;

using OtterGui;

using Penumbra.Api.Enums;
using Penumbra.String.Classes;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;

public sealed partial class Mod {
  private static class Migration {
    public static bool Migrate(Mod mod, JObject json) {
      var ret = MigrateV0ToV1( mod, json ) || MigrateV1ToV2( mod ) || MigrateV2ToV3( mod );
      if (ret) {
        // Immediately save on migration.
        mod.SaveMetaFile();
      }

      return ret;
    }

    private static bool MigrateV2ToV3(Mod mod) {
      if (mod.FileVersion > 2) {
        return false;
      }

      // Remove import time.
      mod.FileVersion = 3;
      return true;
    }


    private static readonly Regex GroupRegex = new( @"group_\d{3}_", RegexOptions.Compiled );
    private static bool MigrateV1ToV2(Mod mod) {
      if (mod.FileVersion > 1) {
        return false;
      }

      if (!mod.GroupFiles.All(g => GroupRegex.IsMatch(g.Name))) {
        foreach (var (group, index) in mod.GroupFiles.WithIndex().ToArray()) {
          var newName = Regex.Replace( group.Name, "^group_", $"group_{index + 1:D3}_", RegexOptions.Compiled );
          try {
            if (newName != group.Name) {
              group.MoveTo(Path.Combine(group.DirectoryName ?? string.Empty, newName), false);
            }
          } catch (Exception e) {
            Logger.Log.Error($"Could not rename group file {group.Name} to {newName} during migration:\n{e}");
          }
        }
      }

      mod.FileVersion = 2;

      return true;
    }

    private static bool MigrateV0ToV1(Mod mod, JObject json) {
      if (mod.FileVersion > 0) {
        return false;
      }

      var swaps = json[ "FileSwaps" ]?.ToObject< Dictionary< Utf8GamePath, FullPath > >()
             ?? new Dictionary< Utf8GamePath, FullPath >();
      var groups        = json[ "Groups" ]?.ToObject< Dictionary< string, OptionGroupV0 > >() ?? new Dictionary< string, OptionGroupV0 >();
      var priority      = 1;
      var seenMetaFiles = new HashSet< FullPath >();
      foreach (var group in groups.Values) {
        ConvertGroup(mod, group, ref priority, seenMetaFiles);
      }

      foreach (var unusedFile in mod.FindUnusedFiles().Where(f => !seenMetaFiles.Contains(f))) {
        if (unusedFile.ToGamePath(mod.ModPath, out var gamePath)
        && !mod._default.FileData.TryAdd(gamePath, unusedFile)) {
          Logger.Log.Error($"Could not add {gamePath} because it already points to {mod._default.FileData[gamePath]}.");
        }
      }

      mod._default.FileSwapData.Clear();
      mod._default.FileSwapData.EnsureCapacity(swaps.Count);
      foreach (var (gamePath, swapPath) in swaps) {
        mod._default.FileSwapData.Add(gamePath, swapPath);
      }

      mod._default.IncorporateMetaChanges(mod.ModPath, true);
      foreach (var (group, index) in mod.Groups.WithIndex()) {
        IModGroup.Save(group, mod.ModPath, index);
      }

      // Delete meta files.
      foreach (var file in seenMetaFiles.Where(f => f.Exists)) {
        try {
          File.Delete(file.FullName);
        } catch (Exception e) {
          Logger.Log.Warning($"Could not delete meta file {file.FullName} during migration:\n{e}");
        }
      }

      // Delete old meta files.
      var oldMetaFile = Path.Combine( mod.ModPath.FullName, "metadata_manipulations.json" );
      if (File.Exists(oldMetaFile)) {
        try {
          File.Delete(oldMetaFile);
        } catch (Exception e) {
          Logger.Log.Warning($"Could not delete old meta file {oldMetaFile} during migration:\n{e}");
        }
      }

      mod.FileVersion = 1;
      mod.SaveDefaultMod();
      mod.SaveMetaFile();

      return true;
    }

    private static void ConvertGroup(Mod mod, OptionGroupV0 group, ref int priority, HashSet<FullPath> seenMetaFiles) {
      if (group.Options.Count == 0) {
        return;
      }

      switch (group.SelectionType) {
        case GroupType.Multi:

          var optionPriority = 0;
          var newMultiGroup = new MultiModGroup()
                    {
            Name        = group.GroupName,
            Priority    = priority++,
            Description = string.Empty,
          };
          mod._groups.Add(newMultiGroup);
          foreach (var option in group.Options) {
            newMultiGroup.PrioritizedOptions.Add((SubModFromOption(mod, option, seenMetaFiles), optionPriority++));
          }

          break;
        case GroupType.Single:
          if (group.Options.Count == 1) {
            AddFilesToSubMod(mod._default, mod.ModPath, group.Options[0], seenMetaFiles);
            return;
          }

          var newSingleGroup = new SingleModGroup()
                    {
            Name        = group.GroupName,
            Priority    = priority++,
            Description = string.Empty,
          };
          mod._groups.Add(newSingleGroup);
          foreach (var option in group.Options) {
            newSingleGroup.OptionData.Add(SubModFromOption(mod, option, seenMetaFiles));
          }

          break;
      }
    }

    private static void AddFilesToSubMod(SubMod mod, DirectoryInfo basePath, OptionV0 option, HashSet<FullPath> seenMetaFiles) {
      foreach (var (relPath, gamePaths) in option.OptionFiles) {
        var fullPath = new FullPath( basePath, relPath );
        foreach (var gamePath in gamePaths) {
          mod.FileData.TryAdd(gamePath, fullPath);
        }

        if (fullPath.Extension is ".meta" or ".rgsp") {
          seenMetaFiles.Add(fullPath);
        }
      }
    }

    private static SubMod SubModFromOption(Mod mod, OptionV0 option, HashSet<FullPath> seenMetaFiles) {
      var subMod = new SubMod( mod ) { Name = option.OptionName };
      AddFilesToSubMod(subMod, mod.ModPath, option, seenMetaFiles);
      subMod.IncorporateMetaChanges(mod.ModPath, false);
      return subMod;
    }

    private struct OptionV0 {
      public string OptionName = string.Empty;
      public string OptionDesc = string.Empty;

      [JsonProperty( ItemConverterType = typeof( SingleOrArrayConverter< Utf8GamePath > ) )]
      public Dictionary< Utf8RelPath, HashSet< Utf8GamePath > > OptionFiles = new();

      public OptionV0() { }
    }

    private struct OptionGroupV0 {
      public string GroupName = string.Empty;

      [JsonConverter( typeof( Newtonsoft.Json.Converters.StringEnumConverter ) )]
      public GroupType SelectionType = GroupType.Single;

      public List< OptionV0 > Options = new();

      public OptionGroupV0() { }
    }

    // Not used anymore, but required for migration.
    private class SingleOrArrayConverter<T> : JsonConverter {
      public override bool CanConvert(Type objectType)
          => objectType == typeof(HashSet<T>);

      public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
        var token = JToken.Load( reader );

        if (token.Type == JTokenType.Array) {
          return token.ToObject<HashSet<T>>() ?? new HashSet<T>();
        }

        var tmp = token.ToObject< T >();
        return tmp != null
            ? new HashSet<T> { tmp }
            : new HashSet<T>();
      }

      public override bool CanWrite
          => true;

      public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        writer.WriteStartArray();
        if (value != null) {
          var v = ( HashSet< T > )value;
          foreach (var val in v) {
            serializer.Serialize(writer, val?.ToString());
          }
        }

        writer.WriteEndArray();
      }
    }
  }
}
