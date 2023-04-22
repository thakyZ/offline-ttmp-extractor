param(
  [Parameter(Mandatory = $True, Position = 0)]
  [string]
  $Path
)

$NerdFontFile = "https://raw.githubusercontent.com/ryanoasis/nerd-fonts/master/patched-fonts/CascadiaCode/Regular/complete/Caskaydia%20Cove%20Nerd%20Font%20Complete%20Mono%20Windows%20Compatible%20Regular.otf";

$NerdFontDownload = "$($Path)Resources\Fonts\NerdFontCaskaydiaMono.otf";

if (-not (Test-Path -LiteralPath $NerdFontDownload -PathType Leaf)) {
  try {
    Invoke-WebRequest -Uri $NerdFontFile -OutFile $NerdFontDownload;
  } catch {
    Write-Error -Message "Failed to download nerd font...\n $($_)";
    Exit 1;
  }
}

Exit 0;