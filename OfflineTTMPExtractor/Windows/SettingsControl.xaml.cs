using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CheapLoc;

using MaterialDesignThemes.Wpf.Transitions;

using OfflineTTMPExtractor.Windows.ViewModel;

namespace OfflineTTMPExtractor.Windows;
/// <summary>
/// Interaction logic for SettingsControl.xaml
/// </summary>
public partial class SettingsControl : UserControl {
  public event EventHandler SettingsDismissed;
  public event EventHandler CloseMainWindowGracefully;

  private SettingsControlViewModel ViewModel => DataContext as SettingsControlViewModel;

  public SettingsControl() {
    InitializeComponent();

    DataContext = new SettingsControlViewModel();

    ReloadSettings();
  }

  public void ReloadSettings() {
  }

  private void AcceptButton_Click(object sender, RoutedEventArgs e) {

    SettingsDismissed?.Invoke(this, null);

    Transitioner.MoveNextCommand.Execute(null, null);
  }
}
