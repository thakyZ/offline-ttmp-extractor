using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using CheapLoc;

using MaterialDesignThemes.Wpf;

using OfflineTTMPExtractor.Utils;

namespace OfflineTTMPExtractor.Windows.ViewModel;
internal class ProgressDialogViewModel : INotifyPropertyChanged {
  private ProgressDialog ProgressDialog { get; init; }

  public ProgressDialogViewModel(ProgressDialog progressDialog) {
    this.ProgressDialog = progressDialog;
    this.CancelButtonCommand = new AnotherCommandImplementation(this.CancelProgressDialog);
    this.CloseButtonCommand = new AnotherCommandImplementation(this.CloseProgressDialog);
    this.OpenDialogCommand = new AnotherCommandImplementation(this.OpenProgressDialog);
    this.SetupLoc();
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  private void SetupLoc() {
    this.CloseButtonLoc = Loc.Localize("CloseButtonTitle", "Close");
    this.CancelButtonLoc = Loc.Localize("CancelButtonTitle", "Cancel");
  }
  public ICommand CancelButtonCommand { get; }
  public ICommand CloseButtonCommand { get; }
  public ICommand OpenDialogCommand { get; }

  private string _messageExtractingOption = string.Empty;

  public string Message_ExtractingOption {
    get => this._messageExtractingOption;
    private set {
      if (this._messageExtractingOption != value) {
        this._messageExtractingOption = value;
        this.NotifyPropertyChanged(nameof(this.Message_ExtractingOption));
      }
    }
  }

  private int _progressExtractingOption;

  public int Progress_ExtractingOption {
    get => this._progressExtractingOption;
    private set {
      if (this._progressExtractingOption != value) {
        this._progressExtractingOption = value;
        this.NotifyPropertyChanged(nameof(this.Progress_ExtractingOption));
      }
    }
  }

  private string _messageImporterState = string.Empty;

  public string Message_ImporterState {
    get => this._messageImporterState;
    private set {
      if (this._messageImporterState != value) {
        this._messageImporterState = value;
        this.NotifyPropertyChanged(nameof(this.Message_ImporterState));
      }
    }
  }

  private int _progressImporterState;

  public int Progress_ImporterState {
    get => this._progressImporterState;
    private set {
      if (this._progressImporterState != value) {
        this._progressImporterState = value;
        this.NotifyPropertyChanged(nameof(this.Progress_ImporterState));
      }
    }
  }

  private string _messageExtractingFile = string.Empty;

  public string Message_ExtractingFile {
    get => this._messageExtractingFile;
    private set {
      if (this._messageExtractingFile != value) {
        this._messageExtractingFile = value;
        this.NotifyPropertyChanged(nameof(this.Message_ExtractingFile));
      }
    }
  }

  private int _progressExtractingFile;

  public int Progress_ExtractingFile {
    get => this._progressExtractingFile;
    private set {
      if (this._progressExtractingFile != value) {
        this._progressExtractingFile = value;
        this.NotifyPropertyChanged(nameof(this.Progress_ExtractingFile));
      }
    }
  }

  public string CloseButtonLoc { get; private set; } = string.Empty;

  public string CancelButtonLoc { get; private set; } = string.Empty;

  private void CloseProgressDialog(object? sender) {
    // TODO: Extract Function
  }

  private void CancelProgressDialog(object? sender) {
    // TODO: Extract Function
  }

  private void OpenProgressDialog(object? sender) {
    // TODO: Extract Function
  }

  public void ProgressDialog_ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs e) {
    // TODO: Extract Function
  }

  public void ProgressDialog_ExtendedClosingEventHandler(object sender, DialogClosingEventArgs e) {
    // TODO: Extract Function
  }

  public void ProgressDialog_ExtendedClosedEventHandler(object sender, DialogClosedEventArgs e) {
    // TODO: Extract Function
  }
}
