using System;
using System.Runtime.InteropServices;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Files;

using Penumbra.GameData.Enums;
using Penumbra.GameData.Structs;
using Penumbra.Interop.Structs;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct EqdpManipulation : Penumbra.Meta.Manipulations.IMetaManipulation<EqdpManipulation> {
  public EqdpEntry Entry { get; private init; }

  [JsonConverter(typeof(StringEnumConverter))]
  public Gender Gender { get; private init; }

  [JsonConverter(typeof(StringEnumConverter))]
  public ModelRace Race { get; private init; }

  public ushort SetId { get; private init; }

  [JsonConverter(typeof(StringEnumConverter))]
  public EquipSlot Slot { get; private init; }

  [JsonConstructor]
  public EqdpManipulation(EqdpEntry entry, EquipSlot slot, Gender gender, ModelRace race, ushort setId) {
    Gender = gender;
    Race = race;
    SetId = setId;
    Slot = slot;
    Entry = Eqdp.Mask(Slot) & entry;
  }

  public override string ToString()
      => $"Eqdp - {SetId} - {Slot} - {/*Race.ToName()*/ "Unknown"} - {/*Gender.ToName()*/ "Unknown"}";

  public bool Equals(EqdpManipulation other)
      => Gender == other.Gender
       && Race == other.Race
       && SetId == other.SetId
       && Slot == other.Slot;

  public override bool Equals(object? obj)
      => obj is EqdpManipulation other && Equals(other);

  public override int GetHashCode()
      => HashCode.Combine((int)Gender, (int)Race, SetId, (int)Slot);

  public int CompareTo(EqdpManipulation other) {
    var r = Race.CompareTo( other.Race );
    if (r != 0) {
      return r;
    }

    var g = Gender.CompareTo( other.Gender );
    if (g != 0) {
      return g;
    }

    var set = SetId.CompareTo( other.SetId );
    return set != 0 ? set : Slot.CompareTo(other.Slot);
  }

  public CharacterUtility.Index FileIndex()
      => CharacterUtility.EqdpIdx(Names.CombinedRace(Gender, Race), Slot.IsAccessory());

  public bool Apply(ExpandedEqdpFile file) {
    var entry = file[ SetId ];
    var mask  = Eqdp.Mask( Slot );
    if ((entry & mask) == Entry) {
      return false;
    }

    file[SetId] = (entry & ~mask) | Entry;
    return true;
  }
}
