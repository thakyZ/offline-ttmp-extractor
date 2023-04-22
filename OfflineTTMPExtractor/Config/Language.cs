using System.Collections.Generic;

namespace OfflineTTMPExtractor.Config;

public enum Language {
  English,
  // TODO: Add more languages.
}

public static class LanguageExtensions {
  private static Dictionary<Language, string> GetLangCodes() {
    return new Dictionary<Language, string>
    {
      { Language.English, "en" }
    };
  }

  public static string GetLocalizationCode(this Language? language) {
    return GetLangCodes()[language ?? Language.English]; // Default localization language
  }

  public static bool IsDefault(this Language? language) {
    return language is null or Language.English;
  }

  public static Language GetLangFromTwoLetterISO(this Language? language, string code) {
    foreach ((Language langCode, var name) in GetLangCodes()) {
      if (name == code) {
        return langCode;
      }
    }

    return Language.English; // Default language
  }
}
