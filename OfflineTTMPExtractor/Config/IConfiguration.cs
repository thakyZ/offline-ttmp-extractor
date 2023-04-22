using System.Text.Json.Serialization;

namespace OfflineTTMPExtractor.Config;
public interface IConfiguration {
  /// <summary>
  /// Gets or sets configuration version.
  /// </summary>
  [JsonPropertyName("version")]
  int Version { get; set; }
}
