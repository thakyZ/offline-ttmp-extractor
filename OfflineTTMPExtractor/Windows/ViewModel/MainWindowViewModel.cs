using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

using CheapLoc;

using MaterialDesignThemes.Wpf;

namespace OfflineTTMPExtractor.Windows.ViewModel;

internal class MainWindowViewModel : INotifyPropertyChanged {
  private MainWindow MainWindow { get; }

  public MainWindowViewModel(MainWindow mainWindow) {
    this.MainWindow = mainWindow;
    this.SetupLoc();
    this.MainWindow.DarkModeChange += this.MainWindow_DarkModeChange;
    //MainWindow_DarkModeChange(null, App.Instance.Configuration.IsDarkMode);
  }

  public static readonly PackIcon LightModeIcon = new() { Kind = PackIconKind.WeatherSunny };
  public static readonly PackIcon DarkModeIcon = new() { Kind = PackIconKind.WeatherNight };

  public event PropertyChangedEventHandler? PropertyChanged;

  private void MainWindow_DarkModeChange(object? sender, bool state) {
    this.DarkModeStateIcon = state ? DarkModeIcon : LightModeIcon;
    this.DarkModeStateTooltip = state ? this.DarkModeTooltipLoc : this.LightModeTooltipLoc;
    ModifyTheme(state);
  }
  private static void ModifyTheme(bool isDarkTheme) {
    var paletteHelper = new PaletteHelper();
    ITheme theme = paletteHelper.GetTheme();

    theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
    paletteHelper.SetTheme(theme);
  }

  private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  private void SetupLoc() {
    this.InputFileTitleLoc = Loc.Localize("InputFileTitle", "TTMP File");
    this.OutputFolderTitleLoc = Loc.Localize("OutputFolderTitle", "Output Folder");
    this.DarkModeTooltipLoc = Loc.Localize("DarkModeTooltip", "Switch to light mode");
    this.LightModeTooltipLoc = Loc.Localize("LightModeTooltip", "Switch to dark mode");
    this.CloseButtonLoc = Loc.Localize("CloseButton", "Close");
    this.ExtractButtonLoc = Loc.Localize("ExtractButton", "Extract");
    this.SettingsTooltipLoc = Loc.Localize("SettingsTooltip", "Settings");
  }

  private string _inputFileStatusText = string.Empty;

  public string InputFileStatusText {
    get => this._inputFileStatusText;
    private set {
      if (this._inputFileStatusText != value) {
        this._inputFileStatusText = value;
        this.NotifyPropertyChanged(nameof(this.OutputFolderStatusText));
      }
    }
  }

  private string _outputFolderStatusText = string.Empty;

  public string OutputFolderStatusText {
    get => this._outputFolderStatusText;
    private set {
      if (this._outputFolderStatusText != value) {
        this._outputFolderStatusText = value;
        this.NotifyPropertyChanged(nameof(this.OutputFolderStatusText));
      }
    }
  }

  public string InputFileTitleLoc { get; private set; } = string.Empty;

  public string OutputFolderTitleLoc { get; private set; } = string.Empty;

  public string DarkModeTooltipLoc { get; private set; } = string.Empty;

  public string LightModeTooltipLoc { get; private set; } = string.Empty;

  public string CloseButtonLoc { get; private set; } = string.Empty;

  public string ExtractButtonLoc { get; private set; } = string.Empty;

  public string SettingsTooltipLoc { get; private set; } = string.Empty;

  //public static readonly DependencyProperty HelperTextStyleProperty
  //        = DependencyProperty.RegisterAttached("HelperTextStyle", typeof(Style), typeof(HintAssist),
  //            new PropertyMetadata(null));

  // TODO: Change later to get the color of MaterialDesignDarkSeparatorBackground & MaterialDesignSeparatorBackground
  public Brush _borderBrushState = Brushes.Gray;

  public Brush BorderBrushState { get => this._borderBrushState; set => _ = this.SetProperty(ref this._borderBrushState, value); }

  private PackIcon _darkModeStateIcon = LightModeIcon;
  public PackIcon DarkModeStateIcon {
    get => this._darkModeStateIcon;
    private set {
      if (this._darkModeStateIcon != value) {
        this._darkModeStateIcon = value;
        this.NotifyPropertyChanged(nameof(this.DarkModeStateIcon));
      }
    }
  }

  private string _darkModeStateTooltip = string.Empty;
  public string DarkModeStateTooltip {
    get => this._darkModeStateTooltip;
    private set {
      if (this._darkModeStateTooltip != value) {
        this._darkModeStateTooltip = value;
        this.NotifyPropertyChanged(nameof(this.DarkModeStateTooltip));
      }
    }
  }

  protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null) {
    if (!Equals(field, newValue)) {
      field = newValue;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      return true;
    }

    return false;
  }
}
