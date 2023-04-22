using System;
using System.IO;

using Newtonsoft.Json;

using OfflineTTMPExtractor.Utils;

using Serilog;

namespace OfflineTTMPExtractor.Config;

public static class ConfigurationBase {
  private static string ConfigPath => Path.Join(App.GetConfigPath(), "config.json");

  private static FileInfo GetConfigFile => new(ConfigPath);

  public static void Save(IConfiguration config) {
    File.WriteAllText(GetConfigFile.FullName, SerializeConfig(config));
  }

  public static Configuration Load() {
    FileInfo path = GetConfigFile;

    Configuration? deserialized = null;
    try {
      if (path.Exists) {
        deserialized = DeserializeConfig(File.ReadAllText(path.FullName));
      }
    } catch (Exception exception) {
      if (MainWindow.Console == null) {
        Log.Error(exception, $"[{DateTime.Now:yyyy-MM-dd_HH:mm:ss.fff}] [ERR] Failed to deserialize configuration");
      } else {
        Logger.Log.Error("Failed to deserialize configuration", exception);
      }
    }

    deserialized ??= new Configuration();
    return deserialized;
  }

  internal static string SerializeConfig(object? config) {
    return JsonConvert.SerializeObject(config, Formatting.Indented, new JsonSerializerSettings {
      TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
      TypeNameHandling = TypeNameHandling.Objects,
    });
  }

  internal static Configuration? DeserializeConfig(string data) {
    return JsonConvert.DeserializeObject<Configuration>(
        data,
        new JsonSerializerSettings {
          TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
          TypeNameHandling = TypeNameHandling.Objects,
        });
  }
}
