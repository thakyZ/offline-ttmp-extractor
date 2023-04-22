using System;

using Newtonsoft.Json;

namespace OfflineTTMPExtractor.Config;

[Serializable]
public class Configuration : IConfiguration {
  [JsonProperty("version")]
  public int Version { get; set; }

  [JsonProperty("is_dark_mode")]
  public bool IsDarkMode { get; set; } = false;

  [JsonProperty("last_import_dir")]
  public string LastImportDirectory { get; set; } = "";

  [JsonProperty("last_export_dir")]
  public string LastExportDirectory { get; set; } = "";

  [JsonIgnore]
  public string ExportDirectory { get => this.LastExportDirectory; set => this.LastExportDirectory = value; }

  [JsonProperty("penumbra_dir")]
  public string PenumbraDirectory { get; set; } = "";

  [JsonIgnore]
  public string ModDirectory { get => this.PenumbraDirectory; set => this.PenumbraDirectory = value; }

  [JsonIgnore]
  public bool KeepDefaultMetaChanges => true;

  [JsonProperty(Required = Required.AllowNull)]
  public Language? Language { get; set; }

  [JsonProperty("modpack_dir")]
  public string ModpackDirectory { get; set; } = "";

  [JsonProperty("auto_deduplicate_on_import")]
  public bool AutoDeduplicateOnImport { get; set; }

  public Configuration() : base() { }

  public void Save() {
    // To be implemented.
  }
}
