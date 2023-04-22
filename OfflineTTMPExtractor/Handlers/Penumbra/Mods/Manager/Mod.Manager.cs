using System.Collections;
using System.Collections.Generic;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;

public sealed partial class Mod {
  public sealed partial class Manager : IReadOnlyList<Mod> {

    private readonly List<Mod> _mods = new();

    public int Count
        => _mods.Count;

    public Mod this[int index] => this._mods[index];

    public IEnumerator<Mod> GetEnumerator()
        => _mods.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public Manager(string modDirectory) {
      ModDirectoryChanged += OnModDirectoryChange;
      SetBaseDirectory(modDirectory, true);
      UpdateExportDirectory(App.Instance.Configuration.ExportDirectory, false);
    }
  }
}
