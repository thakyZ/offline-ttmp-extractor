using System.Diagnostics.CodeAnalysis;
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Save function not yet implemented.",
                           Scope = "member", Target = "~M:OfflineTTMPExtractor.Config.Configuration.Save")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "KeepDefaultMetaChanges will be able to be changed later.",
                           Scope = "member", Target = "~P:OfflineTTMPExtractor.Config.Configuration.KeepDefaultMetaChanges")]
