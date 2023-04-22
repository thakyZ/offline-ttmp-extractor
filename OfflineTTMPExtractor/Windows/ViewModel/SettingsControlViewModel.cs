using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using CheapLoc;

namespace OfflineTTMPExtractor.Windows.ViewModel;
internal class SettingsControlViewModel : INotifyPropertyChanged {
  private SettingsControl SettingsControl { get; }

  public SettingsControlViewModel(SettingsControl settingsControl) {
    this.SettingsControl = settingsControl;
    this.SetupLoc();
    ((MainWindow)App.Instance.MainWindow).DarkModeChange += this.SettingsControl_DarkModeChange;
  }
  private void SettingsControl_DarkModeChange(object? sender, bool state) {
    //ModifyTheme(state);
  }

  private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  private void SetupLoc() {
    this.GeneralTabLoc = Loc.Localize("GeneralTab", "General");
    this.SaveSettingsLoc = Loc.Localize("SaveSettings", "Save Settings");
  }

  public string GeneralTabLoc { get; private set; } = string.Empty;

  public string SaveSettingsLoc { get; private set; } = string.Empty;
}
