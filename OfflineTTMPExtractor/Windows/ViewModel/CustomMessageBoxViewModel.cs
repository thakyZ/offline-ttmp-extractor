using System.Windows.Input;

using CheapLoc;

namespace OfflineTTMPExtractor.Windows.ViewModel;

internal class CustomMessageBoxViewModel {
  public ICommand CopyMessageTextCommand { get; set; }

  public CustomMessageBoxViewModel() {
    this.SetupLoc();
  }

  private void SetupLoc() {
    this.JoinDiscordLoc = Loc.Localize("JoinDiscord", "Join Discord");
    this.OpenIntegrityReportLoc = Loc.Localize("OpenIntegrityReport", "Open Integrity Report");
    this.OpenFaqLoc = Loc.Localize("OpenFaq", "Open FAQ");
    this.ReportErrorLoc = Loc.Localize("ReportError", "Report error");
    this.OkLoc = Loc.Localize("OK", "OK");
    this.YesWithShortcutLoc = Loc.Localize("Yes", "Yes");
    this.NoWithShortcutLoc = Loc.Localize("No", "No");
    this.CancelWithShortcutLoc = Loc.Localize("Cancel", "Cancel");
    this.CopyWithShortcutLoc = Loc.Localize("Copy", "Copy");
  }

  public string JoinDiscordLoc { get; private set; } = string.Empty;
  public string OpenIntegrityReportLoc { get; private set; } = string.Empty;
  public string OpenFaqLoc { get; private set; } = string.Empty;
  public string ReportErrorLoc { get; private set; } = string.Empty;
  public string OkLoc { get; private set; } = string.Empty;
  public string YesWithShortcutLoc { get; private set; } = string.Empty;
  public string NoWithShortcutLoc { get; private set; } = string.Empty;
  public string CancelWithShortcutLoc { get; private set; } = string.Empty;
  public string CopyWithShortcutLoc { get; private set; } = string.Empty;
}
