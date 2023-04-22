using System;
using System.Runtime.InteropServices;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Files;

using Penumbra.GameData.Enums;
using Penumbra.Interop.Structs;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct RspManipulation : Penumbra.Meta.Manipulations.IMetaManipulation<RspManipulation> {
  public float Entry { get; private init; }

  [JsonConverter(typeof(StringEnumConverter))]
  public SubRace SubRace { get; private init; }

  [JsonConverter(typeof(StringEnumConverter))]
  public RspAttribute Attribute { get; private init; }

  [JsonConstructor]
  public RspManipulation(SubRace subRace, RspAttribute attribute, float entry) {
    Entry = entry;
    SubRace = subRace;
    Attribute = attribute;
  }

  public override string ToString()
      => $"Rsp - {/*SubRace.ToName()*/"Unknown"} - {Attribute.ToFullString()}";

  public bool Equals(RspManipulation other)
      => SubRace == other.SubRace
       && Attribute == other.Attribute;

  public override bool Equals(object? obj)
      => obj is RspManipulation other && Equals(other);

  public override int GetHashCode()
      => HashCode.Combine((int)SubRace, (int)Attribute);

  public int CompareTo(RspManipulation other) {
    var s = SubRace.CompareTo( other.SubRace );
    return s != 0 ? s : Attribute.CompareTo(other.Attribute);
  }

  public CharacterUtility.Index FileIndex()
      => CharacterUtility.Index.HumanCmp;

  public bool Apply(CmpFile file) {
    var value = file[ SubRace, Attribute ];
    if (value == Entry) {
      return false;
    }

    file[SubRace, Attribute] = Entry;
    return true;
  }
}
