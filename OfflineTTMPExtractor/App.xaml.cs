using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

using CheapLoc;

using OfflineTTMPExtractor.Config;
using OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;
using OfflineTTMPExtractor.Utils;

using Penumbra.GameData;

using Serilog;

namespace OfflineTTMPExtractor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
  internal static bool CheckWindowsVersion => !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || Environment.OSVersion.Version.Major > 8;

  internal static App Instance { get; private set; } = null!;
  public static Mod.Manager ModManager { get; private set; } = null!;
  //public static CharacterUtility CharacterUtility { get; private set; } = null!;
  public static IGamePathParser GamePathParser { get; private set; } = null!;
  //public static MetaFileManager MetaFileManager { get; private set; } = null!;

  internal Configuration Configuration { get; set; } = null!;

  internal MainWindow GetMainWindow() => (MainWindow)this.MainWindow;

  public App() {
    Instance = this;

    if (CheckWindowsVersion) {
      Log.Logger = new LoggerConfiguration()
      .WriteTo.Async(a =>
         a.File(System.IO.Path.Combine(App.GetConfigPath(), "output.log")))
#if DEBUG
        .WriteTo.Debug()
        .MinimumLevel.Verbose()
#else
        .MinimumLevel.Information()
#endif
        .CreateLogger();

      this.Configuration = ConfigurationBase.Load();

      // Was delayed loading at start time.
      ModManager = new Mod.Manager(this.Configuration.ModDirectory);
      //CharacterUtility = new CharacterUtility();
      GamePathParser = GameData.GetGamePathParser();
      //MetaFileManager = new MetaFileManager();

      Log.Information("Trying to set up Loc for language code {0}", this.Configuration.Language.GetLocalizationCode());
      if (!this.Configuration.Language.IsDefault()) {
        Loc.Setup(Util.GetFromResources(Util.GetFromResources($"OfflineTTMPExtractor.Resources.Loc.loc_{this.Configuration.Language.GetLocalizationCode()}.json")!));
      } else {
        try {
          Loc.Setup(Util.GetFromResources("OfflineTTMPExtractor.Resources.Loc.Localizable.json"));
        } catch {
          Loc.SetupWithFallbacks();
        }
      }
    }

    this.Exit += this.App_Exit;
  }

  private void App_Exit(object sender, ExitEventArgs e) {
    // TODO: Implement dispose.
  }

  internal static string GetConfigPath() {
    var folersToCreate = new string[] { "NekoBoiNick", "OfflineTTMPExtractor" };
    var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    try {
      if (!Directory.Exists(System.IO.Path.Join(appDataFolder, folersToCreate[0]))) {
        _ = Directory.CreateDirectory(System.IO.Path.Join(appDataFolder, folersToCreate[0]));
      }

      if (!Directory.Exists(System.IO.Path.Join(appDataFolder, folersToCreate[0], folersToCreate[1]))) {
        _ = Directory.CreateDirectory(System.IO.Path.Join(appDataFolder, folersToCreate[0], folersToCreate[1]));
      }
    } catch (IOException ioException) {
      Logger.Log.Error("Failed to create missing configuration directories.", ioException);
    }

    return System.IO.Path.Join(appDataFolder, folersToCreate[0], folersToCreate[1]);
  }
}
