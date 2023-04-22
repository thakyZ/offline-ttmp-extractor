using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using MaterialDesignThemes.Wpf;

using OfflineTTMPExtractor.Config;
using OfflineTTMPExtractor.Utils;
using OfflineTTMPExtractor.Windows;
using OfflineTTMPExtractor.Windows.ViewModel;

namespace OfflineTTMPExtractor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
  internal static RichConsole Console { get; set; } = null!;

  internal static Brush ForegroundColor { get; set; } = null!;

  internal bool DarkModeState {
    get => App.Instance.Configuration.IsDarkMode;
    set {
      if (App.Instance.Configuration.IsDarkMode != value) {
        App.Instance.Configuration.IsDarkMode = value;
        this.OnDarkModeChange(value);
      }
    }
  }

  internal ProgressDialog GetProgressDialog() => this.ProgressDialog;

  public event EventHandler<bool>? DarkModeChange;

  protected virtual void OnDarkModeChange(bool state) {
    DarkModeChange?.Invoke(this, state);
  }

  public MainWindow() {
    this.InitializeComponent();

    this.DataContext = new MainWindowViewModel(this);
    Closing += this.MainWindow_OnWindowClosing;
    ContentRendered += this.MainWindow_OnWindowContentRendered;

    var paletteHelper = new PaletteHelper();
    if (paletteHelper.GetThemeManager() is { } themeManager) {
      themeManager.ThemeChanged += (_, e) => {
        this.DarkModeState = e.NewTheme?.GetBaseTheme() == BaseTheme.Dark;
        ForegroundColor = this.Foreground;
        Console.LogUpdateTextTheme();
      };
    }
    /*var theme = paletteHelper.GetTheme();

    MenuDarkModeButton.IsChecked = theme.GetBaseTheme() == BaseTheme.Dark;

    if (paletteHelper.GetThemeManager() is { } themeManager)
    {
        themeManager.ThemeChanged += (_, e)
            => DarkModeToggleButton.IsChecked = e.NewTheme?.GetBaseTheme() == BaseTheme.Dark;
    }*/
  }

  private void MainWindow_OnWindowContentRendered(object? sender, EventArgs e) {
    this.InitalizeConsoleBox();
    if (CheckWindowsVersion) {
      // TODO: Check Windows Version Later (May not need to do?)
    }
  }

  public void InitalizeConsoleBox() {
    ForegroundColor = this.Foreground;
    Console = new RichConsole(this.ConsoleBox);
  }

  public static bool CheckWindowsVersion {
    get {
      if (!App.CheckWindowsVersion) {
        Console.LogError("Windows versions < 8 are not supported!");
        return false;
      }
      Console.LogInformation("Windows version is valid.");
      return true;
    }
  }

  private void MenuDarkModeButton_Click(object? sender, RoutedEventArgs e) {
    this.DarkModeState ^= true;
  }

  private void CloseButton_Click(object? sender, RoutedEventArgs e) {
    //if (true) return;
    this.Close();
  }

  private void SettingsButton_Click(object? sender, RoutedEventArgs e) {
    // TODO: Settings Function
  }

  private void ExtractButton_Click(object? sender, RoutedEventArgs e) {
    // TODO: Extract Function
  }

  public void MainWindow_OnWindowClosing(object? sender, CancelEventArgs args) {
    ConfigurationBase.Save(App.Instance.Configuration);
  }

  private void MainWindow_OnClosed(object sender, EventArgs e) {
    Application.Current.Shutdown();
  }

  private void SettingsControl_OnSettingsDismissed(object sender, EventArgs e) {
    // TODO: Settings Dismissed Function
  }

  internal Task<object?> ShowProgressDialogAsync() {
    return DialogHost.Show(this.ProgressDialog, "ProgressDialog",
      (this.ProgressDialog.DataContext as ProgressDialogViewModel)!.ProgressDialog_ExtendedOpenedEventHandler,
      (this.ProgressDialog.DataContext as ProgressDialogViewModel)!.ProgressDialog_ExtendedClosingEventHandler,
      (this.ProgressDialog.DataContext as ProgressDialogViewModel)!.ProgressDialog_ExtendedClosedEventHandler);
  }

  private void SettingsControl_OnCloseMainWindowGracefully(object sender, EventArgs e) {
    Close();
  }
}
