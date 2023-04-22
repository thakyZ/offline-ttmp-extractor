using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using OfflineTTMPExtractor.Utils;
using OfflineTTMPExtractor.Utils.Extensions;

using Penumbra.Import;

namespace OfflineTTMPExtractor.Handlers.IO;

internal readonly struct IOTaskExport {
  public FileInfo[] ImportFileInfo { get; init; }
  public FileInfo ExportFileInfo { get; init; }

  internal IOTaskExport(FileInfo[] importFileInfo, FileInfo exportFileInfo) {
    this.ImportFileInfo = importFileInfo;
    this.ExportFileInfo = exportFileInfo;
  }
}

internal partial class Import : IDisposable {
  /*
   * private void AddImportModButton(Vector2 size) {
   *   var button = ImGuiUtil.DrawDisabledButton(FontAwesomeIcon.FileImport.ToIconString(), size,
   *   "Import one or multiple mods from Tex Tools Mod Pack Files or Penumbra Mod Pack Files.", !Penumbra.ModManager.Valid, true);
   *   ConfigWindow.OpenTutorial(ConfigWindow.BasicTutorialSteps.ModImport);
   *   if(!button) {
   *     return;
   *   }
   *
   *   var modPath = _hasSetFolder && !Penumbra.Config.AlwaysOpenDefaultImport ? null
   *     : Penumbra.Config.DefaultModImportPath.Length > 0                     ? Penumbra.Config.DefaultModImportPath
   *     : Penumbra.Config.ModDirectory.Length         > 0                     ? Penumbra.Config.ModDirectory : null;
   *   _hasSetFolder = true;
   *
   *   _fileManager.OpenFileDialog("Import Mod Pack", "Mod Packs{.ttmp,.ttmp2,.pmp},TexTools Mod Packs{.ttmp,.ttmp2},Penumbra Mod Packs{.pmp},Archives{.zip,.7z,.rar}", (s, f) => {
   *     if(s) {
   *       _import = new TexToolsImporter(Penumbra.ModManager.BasePath, f.Count, f.Select(file => new FileInfo(file)), AddNewMod);
   *       ImGui.OpenPopup("Import Status");
   *     }
   *   }, 0, modPath);
   * }
   *
   * // Mods need to be added thread-safely outside of iteration.
   * private readonly ConcurrentQueue< DirectoryInfo > _modsToAdd = new();
   *
   * // Clean up invalid directory if necessary.
   * // Add successfully extracted mods.
   * private void AddNewMod(FileInfo file, DirectoryInfo? dir, Exception? error) {
   *   if (error != null) {
   *     if (dir != null && Directory.Exists(dir.FullName)) {
   *       try {
   *         Directory.Delete(dir.FullName, true);
   *       } catch (Exception e) {
   *         Penumbra.Log.Error($"Error cleaning up failed mod extraction of {file.FullName} to {dir.FullName}:\n{e}");
   *       }
   *     }
   *
   *     if (error is not OperationCanceledException) {
   *       Penumbra.Log.Error($"Error extracting {file.FullName}, mod skipped:\n{error}");
   *     }
   *   } else if (dir != null) {
   *     _modsToAdd.Enqueue(dir);
   *   }
   * }
   */

  /// <summary>
  /// The instanced <see cref="TexToolsImporter"/> class.
  /// </summary>
  private TexToolsImporter? _import;

  /// <summary>
  /// The readonly dictionary of valid types of import files.
  /// </summary>
  private static readonly ReadOnlyDictionary<string, ReadOnlyCollection<string>> importFileTypes = new(new Dictionary<string, ReadOnlyCollection<string>>() {
    { "All FFXIV Modpacks", new(new List<string>() { "*.ttmp2", "*.ttmp", "*.pmp", "*.zip", "*.rar", "*.7z" }) },
    { "TexTools", new(new List<string>() { "*.ttmp2", "*.ttmp" }) },
    { "Penumrba", new(new List<string>() { "*.pmp" }) },
    { "Archive", new(new List<string>() { "*.zip", "*.rar", "*.7z" }) }
  });

  /// <summary>
  /// The readonly dictionary of valid types of exportport files.
  /// </summary>
  private static readonly ReadOnlyDictionary<string, ReadOnlyCollection<string>> exportFileTypes = new(new Dictionary<string, ReadOnlyCollection<string>>() {
    { "Penumrba Folder", new ReadOnlyCollection<string>(new List<string>() { "folder" }) },
    { "Penumrba", new ReadOnlyCollection<string>(new List<string>() { "*.pmp" }) }
  });

  /// <summary>
  /// Gets the <see cref="readonly"/> of file types to import or export.
  /// </summary>
  /// <param name="import">Provide <see cref="true"/> if the type you want is import otherwise will give export.</param>
  /// <returns>Returns a <see cref="string"/> to be passed to <see cref="Microsoft.Win32.FileDialog.Filter"/></returns>
  private static string GetFileTypes(bool import) {
    ReadOnlyDictionary<string, ReadOnlyCollection<string>> fileTypes = import ? importFileTypes : exportFileTypes;
    var output = "";

    foreach ((var key, ReadOnlyCollection<string> value) in fileTypes) {
      output += $"|{key}|";
      foreach (var fileType in value) {
        output += $"{fileType};";
      }

      output = PathSepratorRegex().Replace(output, "");
      output += "\n";
    }
    return output;
  }

  /// <summary>
  /// Starts a task to ask for the FFXIV modpack file(s) to import.
  /// </summary>
  /// <returns>An awaitable <see cref="Task"/> of <see cref="FileInfo"/> in an <see cref="Array"/>.</returns>
  /// <exception cref="TaskCanceledException">
  /// <para>If canceled it will return an exception with the message of <c>0x000000</c> as a string.</para>
  /// <para>If failed (somehow) it will return an exception with the message of <c>0x000001</c> as a string.</para>
  /// </exception>
  private async Task<FileInfo[]> AskImportPathAsync() {
    var dialog = new Microsoft.Win32.OpenFileDialog {
      InitialDirectory = string.IsNullOrEmpty(App.Instance.Configuration.LastImportDirectory)
      ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) : App.Instance.Configuration.LastImportDirectory,
      DefaultExt = ".ttmp2", // Default file extension
      Filter = GetFileTypes(true), // Filter files by extension
      CheckPathExists = true,
      CheckFileExists = true,
      Multiselect = true,
      Title = "Import FFXIV Modpack File"
    };
    var dialogStatus = dialog.ShowDialog(App.Instance.MainWindow);
    if (dialogStatus is null) {
      Logger.Log.Error("The Import Dialog Status came back as null. Retrying...");
      await Task.Delay(1000);
    } else {
      return dialogStatus == false ? throw new TaskCanceledException($"{0x000000}") : dialog.FileNames.ToFileInfoArray();
    }

    throw new TaskCanceledException($"{0x000001}");
  }

  /// <summary>
  /// Starts a task to ask for the location to export the FFXIV modpack file(s).
  /// </summary>
  /// <returns>An awaitable <see cref="Task"/> of <see cref="FileInfo"/>.</returns>
  /// <exception cref="TaskCanceledException">
  /// <para>If canceled it will return an exception with the message of <c>0x000000</c> as a string.</para>
  /// <para>If failed (somehow) it will return an exception with the message of <c>0x000001</c> as a string.</para>
  /// </exception>
  private async Task<FileInfo> AskExportPathAsync() {
    var dialog = new Microsoft.Win32.SaveFileDialog {
      InitialDirectory = string.IsNullOrEmpty(App.Instance.Configuration.PenumbraDirectory)
      ? string.IsNullOrEmpty(App.Instance.Configuration.LastExportDirectory)
        ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) : App.Instance.Configuration.LastExportDirectory
      : App.Instance.Configuration.PenumbraDirectory,
      DefaultExt = "folder", // Default file extension
      Filter = GetFileTypes(false), // Filter files by extension
      CheckPathExists = true,
      CheckFileExists = true,
      Title = "Export FFXIV Modpack"
    };
    var dialogStatus = dialog.ShowDialog(App.Instance.MainWindow);
    if (dialogStatus is null) {
      Logger.Log.Error("The Export Dialog Status came back as null. Retrying...");
      await Task.Delay(1000);
    } else {
      return dialogStatus == false ? throw new TaskCanceledException($"{0x000000}") : new FileInfo(dialog.FileName);
    }

    throw new TaskCanceledException($"{0x000001}");
  }

  /// <summary>
  /// Does multiple tasks to ask for the paths of files if not supplied.
  /// </summary>
  /// <param name="importPath">An <see cref="Array"/> of <see cref="string"/>s of files to import.</param>
  /// <param name="exportPath">An <see cref="string"/> of the directory to export the modpack to.</param>
  /// <param name="import"><see cref="NotImplementedException"/></param>
  /// <returns>The information if asked or not asked as a <see cref="IOTaskExport"/> stuct.</returns>
  /// <exception cref="TaskCanceledException">If any dialog was closed it will cancel the import/export.</exception>
  internal async Task<IOTaskExport> DoIOTaskAsync(string[] importPath, string exportPath = "", bool import = true) {
    FileInfo[] importFileInfo;
    FileInfo exportFileInfo;

    if (importPath.Length == 0) {
      try {
        importFileInfo = await await Util.RetryAsync(this.AskImportPathAsync, 4);
      } catch (Exception exception) {
        if (exception.Message == $"{0x000000}") {
          Logger.Log.Verbose("The Import Dialog was closed");
          throw new TaskCanceledException();
        } else {
          Logger.Log.Verbose("Retry failed...");
          throw new TaskCanceledException();
        }
      }
    } else {
      importFileInfo = importPath.ToFileInfoArray();
    }

    if (string.IsNullOrEmpty(exportPath)) {
      try {
        exportFileInfo = await await Util.RetryAsync(this.AskExportPathAsync, 4);
      } catch (Exception exception) {
        if (exception.Message == $"{0x000000}") {
          Logger.Log.Verbose("The Export Dialog was closed");
          throw new TaskCanceledException();
        } else {
          Logger.Log.Verbose("Retry failed...");
          throw new TaskCanceledException();
        }
      }
    } else {
      exportFileInfo = new FileInfo(exportPath);
    }

    return new IOTaskExport(importFileInfo, exportFileInfo);
  }

  /// <summary>
  /// <para>Mods need to be added thread-safely outside of iteration.</para>
  /// <para>Barrowed from <seealso href="https://github.com/xivdev/Penumbra/blob/master/Penumbra/UI/Classes/ModFileSystemSelector.cs"/></para>
  /// </summary>
  private readonly ConcurrentQueue<DirectoryInfo> _modsToAdd = new();

  /// <summary>
  /// <para>Asynchronous task to extract mod files.</para>
  /// <para>
  /// The line starting with <c>var _import = new TexToolsImporter</c> and after until said line's EOL is barrowed from:
  /// <seealso href="https://github.com/xivdev/Penumbra/blob/master/Penumbra/UI/Classes/ModFileSystemSelector.cs"/></para>
  /// </summary>
  /// <param name="importPath"></param>
  /// <param name="exportPath"></param>
  internal async Task ExtractModAsync(string[] importPath, string exportPath) {
    IOTaskExport results = await this.DoIOTaskAsync(importPath, exportPath);

    var _import = new TexToolsImporter(App.ModManager.BasePath, results.ImportFileInfo.Length, results.ImportFileInfo, this.AddNewMod);
    // TODO: Start Draw Progress Task Import Status
    var result = await (App.Instance.MainWindow as MainWindow)!.ShowProgressDialogAsync();
  }

  /// <summary>
  /// <para>Asynchronous task to extract mod files.</para>
  /// <para>Barrowed from <seealso href="https://github.com/xivdev/Penumbra/blob/master/Penumbra/UI/Classes/ModFileSystemSelector.cs"/></para>
  /// </summary>
  /// <param name="file"></param>
  /// <param name="dir"></param>
  /// <param name="error"></param>
  private void AddNewMod(FileInfo file, DirectoryInfo? dir, Exception? error) {
    if (error != null) {
      if (dir != null && Directory.Exists(dir.FullName)) {
        try {
          Directory.Delete(dir.FullName, true);
        } catch (Exception e) {
          Logger.Log.Error($"Error cleaning up failed mod extraction of {file.FullName} to {dir.FullName}:\n{e}");
        }
      }

      if (error is not OperationCanceledException) {
        Logger.Log.Error($"Error extracting {file.FullName}, mod skipped:\n{error}");
      }
    } else if (dir != null) {
      this._modsToAdd.Enqueue(dir);
    }
  }

  /// <summary>
  /// Don't dispose if already disposed.
  /// </summary>
  private bool _isDisposed;

  /// <summary>
  /// Protected Dispose function so no override.
  /// </summary>
  /// <param name="disposing"></param>
  protected virtual void Dispose(bool disposing) {
    if (disposing && !this._isDisposed) {
      this._import?.Dispose();
      this._import = null;

      this._isDisposed = true;
    }
  }

  /// <summary>
  /// Public safe dispose function.
  /// </summary>
  public void Dispose() {
    this.Dispose(true);
    GC.SuppressFinalize(this);
  }

  [GeneratedRegex(";$")]
  private static partial Regex PathSepratorRegex();
}
