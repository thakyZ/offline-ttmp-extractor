using System;
using System.IO;

using OfflineTTMPExtractor.Utils;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;

public sealed partial class Mod {
  public sealed partial class Manager {
    public DirectoryInfo BasePath { get; private set; } = null!;
    private DirectoryInfo? _exportDirectory;

    public bool Valid { get; private set; }
    public event Action< string, bool > ModDirectoryChanged;

    // Set the mod base directory.
    // If its not the first time, check if it is the same directory as before.
    // Also checks if the directory is available and tries to create it if it is not.
    private void SetBaseDirectory(string newPath, bool firstTime) {
      if (!firstTime && string.Equals(newPath, App.Instance.Configuration.ModDirectory, StringComparison.OrdinalIgnoreCase)) {
        return;
      }

      if (newPath.Length == 0) {
        Valid = false;
        BasePath = new DirectoryInfo(".");
        if (App.Instance.Configuration.ModDirectory != BasePath.FullName) {
          ModDirectoryChanged.Invoke(string.Empty, false);
        }
      } else {
        var newDir = new DirectoryInfo( newPath );
        if (!newDir.Exists) {
          try {
            Directory.CreateDirectory(newDir.FullName);
            newDir.Refresh();
          } catch (Exception e) {
            Logger.Log.Error($"Could not create specified mod directory {newDir.FullName}:\n{e}");
          }
        }

        BasePath = newDir;
        Valid = Directory.Exists(newDir.FullName);
        if (App.Instance.Configuration.ModDirectory != BasePath.FullName) {
          ModDirectoryChanged.Invoke(BasePath.FullName, Valid);
        }
      }
    }

    private static void OnModDirectoryChange(string newPath, bool _) {
      Logger.Log.Information($"Set new mod base directory from {App.Instance.Configuration.ModDirectory} to {newPath}.");
      App.Instance.Configuration.ModDirectory = newPath;
      App.Instance.Configuration.Save();
    }

    public void UpdateExportDirectory(string newDirectory, bool change) {
      if (newDirectory.Length == 0) {
        if (_exportDirectory == null) {
          return;
        }

        _exportDirectory = null;
        App.Instance.Configuration.ExportDirectory = string.Empty;
        App.Instance.Configuration.Save();
        return;
      }

      var dir = new DirectoryInfo( newDirectory );
      if (dir.FullName.Equals(_exportDirectory?.FullName, StringComparison.OrdinalIgnoreCase)) {
        return;
      }

      if (!dir.Exists) {
        try {
          Directory.CreateDirectory(dir.FullName);
        } catch (Exception e) {
          Logger.Log.Error($"Could not create Export Directory:\n{e}");
          return;
        }
      }

      /*
       * if (change) {
       *   foreach (var mod in _mods) {
       *     new ModBackup(mod).Move(dir.FullName);
       *   }
       * }
       */

      _exportDirectory = dir;

      if (change) {
        App.Instance.Configuration.ExportDirectory = dir.FullName;
        App.Instance.Configuration.Save();
      }
    }
  }
}
