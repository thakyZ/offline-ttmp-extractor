using System;
//using ImGuiScene;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;

public sealed partial class Mod {
  public partial class Manager {


    private void ChangeTag(Index idx, int tagIdx, string newTag, bool local) {
      /*
       *   var mod   = this[ idx ];
       *   var which = local ? mod.LocalTags : mod.ModTags;
       *   if (tagIdx < 0 || tagIdx > which.Count) {
       *     return;
       *   }
       *
       *   ModDataChangeType flags = 0;
       *   if (tagIdx == which.Count) {
       *     flags = mod.UpdateTags(local ? null : which.Append(newTag), local ? which.Append(newTag) : null);
       *   } else {
       *     var tmp = which.ToArray();
       *     tmp[tagIdx] = newTag;
       *     flags = mod.UpdateTags(local ? null : tmp, local ? tmp : null);
       *   }
       *
       *   if (flags.HasFlag(ModDataChangeType.ModTags)) {
       *     mod.SaveMeta();
       *   }
       *
       *
       *   if (flags.HasFlag(ModDataChangeType.LocalTags)) {
       *     mod.SaveLocalData();
       *   }
       *
       *   if (flags != 0) {
       *     ModDataChanged?.Invoke(flags, mod, null);
       *   }
       */
    }

    /*
     * public void ChangeLocalTag(Index idx, int tagIdx, string newTag)
     *     => ChangeTag(idx, tagIdx, newTag, true);
     */
  }
}
