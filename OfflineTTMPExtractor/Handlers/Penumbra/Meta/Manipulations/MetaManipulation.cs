using System;
using System.Runtime.InteropServices;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Penumbra.String.Functions;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;

/*
 * public interface IMetaManipulation {
 *   public CharacterUtility.Index FileIndex();
 * }
 *
 * public interface IMetaManipulation<T>
 *     : IMetaManipulation, IComparable<T>, IEquatable<T> where T : struct { }
 */

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
public readonly struct MetaManipulation : IEquatable<MetaManipulation>, IComparable<MetaManipulation> {

  public enum Type : byte {
    Unknown = 0,
    Imc     = 1,
    Eqdp    = 2,
    Eqp     = 3,
    Est     = 4,
    Gmp     = 5,
    Rsp     = 6,
  }

  [FieldOffset( 0 )]
  [JsonIgnore]
  public readonly EqpManipulation Eqp = default;

  [FieldOffset( 0 )]
  [JsonIgnore]
  public readonly GmpManipulation Gmp = default;

  [FieldOffset( 0 )]
  [JsonIgnore]
  public readonly EqdpManipulation Eqdp = default;

  [FieldOffset( 0 )]
  [JsonIgnore]
  public readonly EstManipulation Est = default;

  [FieldOffset( 0 )]
  [JsonIgnore]
  public readonly RspManipulation Rsp = default;

  [FieldOffset( 0 )]
  [JsonIgnore]
  public readonly ImcManipulation Imc = default;

  [FieldOffset( 15 )]
  [JsonConverter( typeof( StringEnumConverter ) )]
  [JsonProperty( "Type" )]
  public readonly Type ManipulationType;

  public MetaManipulation(EqpManipulation eqp) {
    Eqp = eqp;
    ManipulationType = Type.Eqp;
  }

  public MetaManipulation(GmpManipulation gmp) {
    Gmp = gmp;
    ManipulationType = Type.Gmp;
  }

  public MetaManipulation(EqdpManipulation eqdp) {
    Eqdp = eqdp;
    ManipulationType = Type.Eqdp;
  }

  public MetaManipulation(EstManipulation est) {
    Est = est;
    ManipulationType = Type.Est;
  }

  public MetaManipulation(RspManipulation rsp) {
    Rsp = rsp;
    ManipulationType = Type.Rsp;
  }

  public MetaManipulation(ImcManipulation imc) {
    Imc = imc;
    ManipulationType = Type.Imc;
  }

  public static implicit operator MetaManipulation(EqpManipulation eqp)
      => new(eqp);

  public static implicit operator MetaManipulation(GmpManipulation gmp)
      => new(gmp);

  public static implicit operator MetaManipulation(EqdpManipulation eqdp)
      => new(eqdp);

  public static implicit operator MetaManipulation(EstManipulation est)
      => new(est);

  public static implicit operator MetaManipulation(RspManipulation rsp)
      => new(rsp);

  public static implicit operator MetaManipulation(ImcManipulation imc)
      => new(imc);

  public bool Equals(MetaManipulation other) {
    if (ManipulationType != other.ManipulationType) {
      return false;
    }

    return ManipulationType switch {
      Type.Eqp => Eqp.Equals(other.Eqp),
      Type.Gmp => Gmp.Equals(other.Gmp),
      Type.Eqdp => Eqdp.Equals(other.Eqdp),
      Type.Est => Est.Equals(other.Est),
      Type.Rsp => Rsp.Equals(other.Rsp),
      Type.Imc => Imc.Equals(other.Imc),
      _ => false,
    };
  }

  public override bool Equals(object? obj)
      => obj is MetaManipulation other && Equals(other);

  public override int GetHashCode()
      => ManipulationType switch {
        Type.Eqp => Eqp.GetHashCode(),
        Type.Gmp => Gmp.GetHashCode(),
        Type.Eqdp => Eqdp.GetHashCode(),
        Type.Est => Est.GetHashCode(),
        Type.Rsp => Rsp.GetHashCode(),
        Type.Imc => Imc.GetHashCode(),
        _ => 0,
      };

  public unsafe int CompareTo(MetaManipulation other) {
    fixed (MetaManipulation* lhs = &this) {
      return MemoryUtility.MemCmpUnchecked(lhs, &other, sizeof(MetaManipulation));
    }
  }

  public override string ToString()
      => ManipulationType switch {
        Type.Eqp => Eqp.ToString(),
        Type.Gmp => Gmp.ToString(),
        Type.Eqdp => Eqdp.ToString(),
        Type.Est => Est.ToString(),
        Type.Rsp => Rsp.ToString(),
        Type.Imc => Imc.ToString(),
        _ => "Invalid",
      };

  public string EntryToString()
      => ManipulationType switch {
        Type.Imc => $"{Imc.Entry.DecalId}-{Imc.Entry.MaterialId}-{Imc.Entry.VfxId}-{Imc.Entry.SoundId}-{Imc.Entry.MaterialAnimationId}-{Imc.Entry.AttributeMask}",
        Type.Eqdp => $"{(ushort)Eqdp.Entry:X}",
        Type.Eqp => $"{(ulong)Eqp.Entry:X}",
        Type.Est => $"{Est.Entry}",
        Type.Gmp => $"{Gmp.Entry.Value}",
        Type.Rsp => $"{Rsp.Entry}",
        _ => string.Empty,
      };
}
