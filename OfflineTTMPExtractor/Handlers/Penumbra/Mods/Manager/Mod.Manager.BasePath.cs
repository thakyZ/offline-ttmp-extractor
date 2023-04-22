using System.IO;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;

public partial class Mod {
  public partial class Manager {
    public delegate void ModPathChangeDelegate(ModPathChangeType type, Mod mod, DirectoryInfo? oldDirectory,
        DirectoryInfo? newDirectory);


    // Add new mods to NewMods and remove deleted mods from NewMods.
    private void OnModPathChange(ModPathChangeType type, Mod mod, DirectoryInfo? oldDirectory,
        DirectoryInfo? newDirectory) {
      /*
       * switch (type) {
       *   case ModPathChangeType.Added:
       *     NewMods.Add(mod);
       *     break;
       *   case ModPathChangeType.Deleted:
       *     NewMods.Remove(mod);
       *     break;
       *   case ModPathChangeType.Moved:
       *     if (oldDirectory != null && newDirectory != null) {
       *       MoveDataFile(oldDirectory, newDirectory);
       *     }
       *
       *     break;
       * }
       */
    }
  }
}
