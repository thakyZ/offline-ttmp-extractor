using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using Serilog;

namespace OfflineTTMPExtractor.Utils;

internal class RichConsole {
  private RichTextBox RichTextBox { get; }
  private Paragraph Paragraph { get; } = new Paragraph();
  internal RichConsole(RichTextBox richTextBox) {
    this.RichTextBox = richTextBox;
    this.RichTextBox.Document.Blocks.Clear();
    this.RichTextBox.Document.Blocks.Add(this.Paragraph);
  }

  private void LogAny(string[] message, string type) {
    var a = new Run("[") { Foreground = MainWindow.ForegroundColor };

    var logTime = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss.fff");
    var b = new Run(logTime) { Foreground = Brushes.DimGray };

    var c = new Run("] [") { Foreground = MainWindow.ForegroundColor };

    string? typeConvert;

    SolidColorBrush? typeBrushColor;

    switch (type) {
      case "error":
        typeConvert = "ERR";
        typeBrushColor = Brushes.Red;
        break;
      case "verbose":
        typeConvert = "VER";
        typeBrushColor = Brushes.LightGray;
        break;
      case "warning":
        typeConvert = "WRN";
        typeBrushColor = Brushes.LightGoldenrodYellow;
        break;
      case "debug":
        typeConvert = "DBG";
        typeBrushColor = Brushes.Lavender;
        break;
      case "info":
      default:
        typeConvert = "INF";
        typeBrushColor = Brushes.CornflowerBlue;
        break;
    }

    var d = new Run(typeConvert) { Foreground = typeBrushColor };

    var e = new Run("] ") { Foreground = MainWindow.ForegroundColor };

    var f = new Run($"{message[0]}{Environment.NewLine}") { Foreground = MainWindow.ForegroundColor };

    var precompiledtext = new List<Run> { a, b, c, d, e, f };

    for (var index = 1; index < message.Length; index++) {
      var g = new Run(string.Concat(message[index], Environment.NewLine)) { Foreground = type == "error" ? typeBrushColor : MainWindow.ForegroundColor };
      precompiledtext.Add(g);
    }

    foreach (Run text in precompiledtext) {
      this.Paragraph.Inlines.Add(text);
    }

    switch (type) {
      case "error":
        Log.Error($"[{logTime}] [{typeConvert}] {string.Join(Environment.NewLine, message)}");
        break;
      case "verbose":
        Log.Verbose($"[{logTime}] [{typeConvert}] {string.Join(Environment.NewLine, message)}");
        break;
      case "warning":
        Log.Warning($"[{logTime}] [{typeConvert}] {string.Join(Environment.NewLine, message)}");
        break;
      default:
        Log.Information($"[{logTime}] [{typeConvert}] {string.Join(Environment.NewLine, message)}");
        break;
    }
  }

  internal void LogInformation(string message) {
    this.LogAny(new[] { message }, "info");
  }

  internal void LogError(string message) {
    this.LogAny(new[] { message }, "error");
  }

  internal void LogError(string message, Exception exception) {
    var exceptionMessage = exception.Message;
    List<string> messages = new() { message, exceptionMessage };
    if (exception.StackTrace is not null) {
      messages = (messages.Concat(exception.StackTrace.Split(Environment.NewLine)) as List<string>)!;
    }
    this.LogAny(messages.ToArray(), "error");
  }

  internal void LogVerbose(string message) {
    this.LogAny(new[] { message }, "verbose");
  }

  internal void LogWarning(string message) {
    this.LogAny(new[] { message }, "warning");
  }

  internal void LogWarning(string message, string info) {
    this.LogAny(new[] { message, info }, "warning");
  }

  internal void LogUpdateTextTheme() {
    foreach (Inline? aRun in this.Paragraph.Inlines) {
      if (aRun is null) {
        continue;
      }
      if (aRun.Foreground != Brushes.Red
        && aRun.Foreground != Brushes.DimGray
        && aRun.Foreground != Brushes.LightGray
        && aRun.Foreground != Brushes.LightGoldenrodYellow
        && aRun.Foreground != Brushes.CornflowerBlue
        && aRun.Foreground != MainWindow.ForegroundColor) {
        aRun.Foreground = MainWindow.ForegroundColor;
      }
    }
  }

  internal void LogDebug(string message) {
    this.LogAny(new[] { message }, "debug");
  }
}

internal static class Logger {
  internal static class Log {
    internal static void Information(string message) {
      MainWindow.Console?.LogInformation(message);
      Serilog.Log.Information(message);
    }

    internal static void Error(string message) {
      MainWindow.Console?.LogError(message);
      Serilog.Log.Error(message);
    }

    internal static void Error(string message, Exception exception) {
      MainWindow.Console?.LogError(message, exception);
      Serilog.Log.Error(message, exception);
    }

    internal static void Verbose(string message) {
      MainWindow.Console?.LogVerbose(message);
      Serilog.Log.Verbose(message);
    }

    internal static void Warning(string message) {
      MainWindow.Console?.LogWarning(message);
      Serilog.Log.Warning(message);
    }

    internal static void Warning(string message, string info) {
      MainWindow.Console?.LogWarning(message, info);
      Serilog.Log.Warning(string.Join(Environment.NewLine, message, info));
    }

    internal static void Debug(string message) {
      MainWindow.Console?.LogDebug(message);
      Serilog.Log.Debug(message);
    }
  }
}
