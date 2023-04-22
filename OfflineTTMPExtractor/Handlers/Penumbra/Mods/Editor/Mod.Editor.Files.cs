using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using OfflineTTMPExtractor.Utils;

using Penumbra.String.Classes;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;

public partial class Mod {
  public partial class Editor {
    public class FileRegistry : IEquatable<FileRegistry> {
      public readonly List<(ISubMod, Utf8GamePath)> SubModUsage = new();
      public FullPath File { get; private init; }
      public Utf8RelPath RelPath { get; private init; }
      public long FileSize { get; private init; }
      public int CurrentUsage;

      public static bool FromFile(Mod mod, FileInfo file, [NotNullWhen(true)] out FileRegistry? registry) {
        var fullPath = new FullPath(file.FullName);
        if (!fullPath.ToRelPath(mod.ModPath, out var relPath)) {
          registry = null;
          return false;
        }

        registry = new FileRegistry {
          File = fullPath,
          RelPath = relPath,
          FileSize = file.Length,
          CurrentUsage = 0,
        };
        return true;
      }

      public bool Equals(FileRegistry? other) {
        if (other is null) {
          return false;
        }

        return ReferenceEquals(this, other) || File.Equals(other.File);
      }

      public override bool Equals(object? obj) {
        if (obj is null) {
          return false;
        }

        if (ReferenceEquals(this, obj)) {
          return true;
        }

        return obj.GetType() == GetType() && Equals((FileRegistry)obj);
      }

      public override int GetHashCode()
          => File.GetHashCode();
    }

    // All files in subdirectories of the mod directory.
    public IReadOnlyList<FileRegistry> AvailableFiles
        => _availableFiles;

    public bool FileChanges { get; private set; }
    private List<FileRegistry> _availableFiles = null!;
    private List<FileRegistry> _mtrlFiles = null!;
    private List<FileRegistry> _mdlFiles = null!;
    private List<FileRegistry> _texFiles = null!;
    private List<FileRegistry> _shpkFiles = null!;
    private readonly HashSet<Utf8GamePath> _usedPaths = new();

    // All paths that are used in
    private readonly SortedSet<FullPath> _missingFiles = new();

    private bool CheckAgainstMissing(FullPath file, Utf8GamePath key, bool removeUsed) {
      if (!_missingFiles.Contains(file)) {
        return true;
      }

      if (removeUsed) {
        _usedPaths.Remove(key);
      }

      Logger.Log.Debug($"[RemoveMissingPaths] Removing {key} -> {file} from {_mod.Name}.");
      return false;
    }

    // Fetch all files inside subdirectories of the main mod directory.
    // Then check which options use them and how often.
    private void UpdateFiles() {
      _availableFiles = _mod.ModPath.EnumerateDirectories()
         .SelectMany(d => d.EnumerateFiles("*.*", SearchOption.AllDirectories)
             .Select(f => FileRegistry.FromFile(_mod, f, out var r) ? r : null)
             .OfType<FileRegistry>())
         .ToList();
      _usedPaths.Clear();
      _mtrlFiles = _availableFiles.Where(f => f.File.FullName.EndsWith(".mtrl", StringComparison.OrdinalIgnoreCase)).ToList();
      _mdlFiles = _availableFiles.Where(f => f.File.FullName.EndsWith(".mdl", StringComparison.OrdinalIgnoreCase)).ToList();
      _texFiles = _availableFiles.Where(f => f.File.FullName.EndsWith(".tex", StringComparison.OrdinalIgnoreCase)).ToList();
      _shpkFiles = _availableFiles.Where(f => f.File.FullName.EndsWith(".shpk", StringComparison.OrdinalIgnoreCase)).ToList();
      FileChanges = false;
      foreach (var subMod in _mod.AllSubMods) {
        foreach (var (gamePath, file) in subMod.Files) {
          if (!file.Exists) {
            _missingFiles.Add(file);
            if (subMod == _subMod) {
              _usedPaths.Add(gamePath);
            }
          } else {
            var registry = _availableFiles.Find(x => x.File.Equals(file));
            if (registry != null) {
              if (subMod == _subMod) {
                ++registry.CurrentUsage;
                _usedPaths.Add(gamePath);
              }

              registry.SubModUsage.Add((subMod, gamePath));
            }
          }
        }
      }
    }
  }
}
