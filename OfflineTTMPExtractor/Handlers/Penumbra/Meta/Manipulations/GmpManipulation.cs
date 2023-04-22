using System.Runtime.InteropServices;

using Newtonsoft.Json;

using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Files;

using Penumbra.GameData.Structs;
using Penumbra.Interop.Structs;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct GmpManipulation : Penumbra.Meta.Manipulations.IMetaManipulation<GmpManipulation> {
  public GmpEntry Entry { get; private init; }
  public ushort SetId { get; private init; }

  [JsonConstructor]
  public GmpManipulation(GmpEntry entry, ushort setId) {
    Entry = entry;
    SetId = setId;
  }

  public override string ToString()
      => $"Gmp - {SetId}";

  public bool Equals(GmpManipulation other)
      => SetId == other.SetId;

  public override bool Equals(object? obj)
      => obj is GmpManipulation other && Equals(other);

  public override int GetHashCode()
      => SetId.GetHashCode();

  public int CompareTo(GmpManipulation other)
      => SetId.CompareTo(other.SetId);

  public CharacterUtility.Index FileIndex()
      => CharacterUtility.Index.Gmp;

  public bool Apply(ExpandedGmpFile file) {
    var entry = file[ SetId ];
    if (entry == Entry) {
      return false;
    }

    file[SetId] = Entry;
    return true;
  }
}
