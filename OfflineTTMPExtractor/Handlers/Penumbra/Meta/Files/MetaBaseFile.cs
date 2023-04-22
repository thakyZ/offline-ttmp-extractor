using System;

//using Dalamud.Memory;

using CharacterUtility = Penumbra.Interop.CharacterUtility;

namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Meta.Files;

public unsafe class MetaBaseFile : IDisposable {
  public byte* Data { get; private set; }
  public int Length { get; private set; }
  public CharacterUtility.InternalIndex Index { get; }

  public MetaBaseFile(Penumbra.Interop.Structs.CharacterUtility.Index idx)
      => Index = CharacterUtility.ReverseIndices[(int)idx];

  protected (IntPtr Data, int Length) DefaultData
      => ((nint)0, 0); //App.CharacterUtility.DefaultResource(Index);

  // Reset to default values.
  public virtual void Reset() { }

  // Obtain memory.
  protected void AllocateData(int length) {
    Length = length;
    //Data = (byte*)App.MetaFileManager.AllocateFileMemory(length);
    if (length > 0) {
      GC.AddMemoryPressure(length);
    }
  }

  // Free memory.
  /*
   * protected void ReleaseUnmanagedResources() {
   *   var ptr = ( IntPtr )Data;
   *   MemoryHelper.GameFree(ref ptr, (ulong)Length);
   *   if (Length > 0) {
   *     GC.RemoveMemoryPressure(Length);
   *   }
   *
   *   Length = 0;
   *   Data = null;
   * }
   */

  // Resize memory while retaining data.
  protected void ResizeResources(int newLength) {
    if (newLength == Length) {
      return;
    }

    //var data = ( byte* )App.MetaFileManager.AllocateFileMemory( ( ulong )newLength );
    if (newLength > Length) {
      //MemoryUtility.MemCpyUnchecked(data, Data, Length);
      //MemoryUtility.MemSet(data + Length, 0, newLength - Length);
    } else {
      //MemoryUtility.MemCpyUnchecked(data, Data, newLength);
    }

    //ReleaseUnmanagedResources();
    GC.AddMemoryPressure(newLength);
    //Data = data;
    Length = newLength;
  }

  // Manually free memory.
  public void Dispose() {
    //ReleaseUnmanagedResources();
    GC.SuppressFinalize(this);
  }

  ~MetaBaseFile() {
    //ReleaseUnmanagedResources();
  }
}
