using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace OfflineTTMPExtractor.Resources;

[MarkupExtensionReturnType(typeof(FontFamily))]
internal class NerdFontExtension : MarkupExtension {
  private static readonly Lazy<FontFamily> _nerfont
            = new(() =>
                new FontFamily(new ("pack://application:,,,/OfflineTTMPExtractor;component/Resources/Fonts/NerdFontCaskaydiaMono.otf"), "#CaskaydiaCove NFM"));

  public override object ProvideValue(IServiceProvider serviceProvider) {
    return _nerfont.Value;
  }
}
