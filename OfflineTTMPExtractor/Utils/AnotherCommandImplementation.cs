using System;
using System.Windows.Input;

namespace OfflineTTMPExtractor.Utils;
internal class AnotherCommandImplementation : ICommand {
  private readonly Action<object?> _execute;
  private readonly Func<object?, bool> _canExecute;

  public AnotherCommandImplementation(Action<object?> execute) : this(execute, null) { }

  public AnotherCommandImplementation(Action<object?> execute, Func<object?, bool>? canExecute) {
    if (execute is null) {
      throw new ArgumentNullException(nameof(execute));
    }

    this._execute = execute;
    this._canExecute = canExecute ?? (_ => true);
  }

  public bool CanExecute(object? parameter) => this._canExecute(parameter);

  public void Execute(object? parameter) => this._execute(parameter);

  public event EventHandler? CanExecuteChanged {
    add => CommandManager.RequerySuggested += value;
    remove => CommandManager.RequerySuggested -= value;
  }

  public void Refresh() => CommandManager.InvalidateRequerySuggested();
}
