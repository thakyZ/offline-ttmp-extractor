using System.Collections.Generic;

using OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Manipulations;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Mods;

public partial class Mod {
  public partial class Editor {
    public struct Manipulations {
      private readonly HashSet<ImcManipulation>  _imc  = new();
      private readonly HashSet<EqpManipulation>  _eqp  = new();
      private readonly HashSet<EqdpManipulation> _eqdp = new();
      private readonly HashSet<GmpManipulation>  _gmp  = new();
      private readonly HashSet<EstManipulation>  _est  = new();
      private readonly HashSet<RspManipulation>  _rsp  = new();

      public bool Changes { get; private set; } = false;

      public Manipulations() { }

      public void Clear() {
        _imc.Clear();
        _eqp.Clear();
        _eqdp.Clear();
        _gmp.Clear();
        _est.Clear();
        _rsp.Clear();
        Changes = true;
      }

      public void Split(IEnumerable<MetaManipulation> manips) {
        Clear();
        foreach (var manip in manips) {
          switch (manip.ManipulationType) {
            case MetaManipulation.Type.Imc:
              _imc.Add(manip.Imc);
              break;
            case MetaManipulation.Type.Eqdp:
              _eqdp.Add(manip.Eqdp);
              break;
            case MetaManipulation.Type.Eqp:
              _eqp.Add(manip.Eqp);
              break;
            case MetaManipulation.Type.Est:
              _est.Add(manip.Est);
              break;
            case MetaManipulation.Type.Gmp:
              _gmp.Add(manip.Gmp);
              break;
            case MetaManipulation.Type.Rsp:
              _rsp.Add(manip.Rsp);
              break;
          }
        }

        Changes = false;
      }
    }

    public Manipulations Meta = new();

    public void RevertManipulations()
        => Meta.Split(_subMod.Manipulations);
  }
}
