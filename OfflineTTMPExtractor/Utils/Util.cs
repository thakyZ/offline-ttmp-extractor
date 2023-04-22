using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Serilog;

namespace OfflineTTMPExtractor.Utils;

public static class Util {
  public static string? GetFromResources(string resourceName) {
    try {
      var asm = Assembly.GetExecutingAssembly();
      using Stream stream = asm.GetManifestResourceStream(resourceName)!;
      using var reader = new StreamReader(stream);
      return reader.ReadToEnd();
    } catch (Exception exception) {
      Log.Error(exception, $"[{DateTime.Now:yyyy-MM-dd_HH:mm:ss.fff}] [ERR] Failed to read resource at {resourceName}");
    }

    return null;
  }

  public static string? GetFromResourcePath(string path) {
    try {
      return File.ReadAllText(path);
    } catch (Exception exception) {
      Log.Error(exception, $"[{DateTime.Now:yyyy-MM-dd_HH:mm:ss.fff}] [ERR] Failed to read file at {path}");
    }

    return null;
  }

  internal static async Task<T> RetryAsync<T>(Func<T> func, int retryCount) {
    while (true) {
      try {
        T result = await Task.Run(func);
        return result;
      } catch when (retryCount-- > 0) { }
    }
  }
}
