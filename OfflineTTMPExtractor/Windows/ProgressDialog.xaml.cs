using System.Windows;
using System.Windows.Controls;

using OfflineTTMPExtractor.Windows.ViewModel;

namespace OfflineTTMPExtractor.Windows;
/// <summary>
/// Interaction logic for ProgressDialog.xaml
/// </summary>
public partial class ProgressDialog : UserControl {
  public ProgressDialog() {
    this.InitializeComponent();

    this.DataContext = new ProgressDialogViewModel(this);
  }

  private void CloseDialogButton_Click(object sender, RoutedEventArgs e) {
    // TODO: Extract Function
  }

  private void OpenDialogButton_Click(object sender, RoutedEventArgs e) {
    // TODO: Extract Function
  }

  private void CancelDialogButton_Click(object sender, RoutedEventArgs e) {
    // TODO: Extract Function
  }
}
