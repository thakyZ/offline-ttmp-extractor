namespace OfflineTTMPExtractor.Handlers.ForkPenumbra.Import;

public enum ImporterState {
  None,
  WritingPackToDisk,
  ExtractingModFiles,
  DeduplicatingFiles,
  Done,
}
