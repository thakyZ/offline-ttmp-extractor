using System.Collections.Generic;
using System.IO;

namespace OfflineTTMPExtractor.Utils.Extensions;

public static class BuiltinExtensions {
  public static FileInfo ToFileInfo(this string value) {
    return new FileInfo(value);
  }

  public static FileInfo[] ToFileInfoArray(this string[] value) {
    var output = new List<FileInfo>();

    foreach (var path in value) {
      output.Add(path.ToFileInfo());
    }

    return output.ToArray();
  }
}
