using System.Windows.Input;

namespace Test;

public abstract class CivilCommand : ICommand
{
    private object? _parameter;

    public event EventHandler? CanExecuteChanged;
    public abstract void Execute();

    public virtual bool CanExecute(object parameter) => true;

    public void Execute(object parameter)
    {
        _parameter = parameter;
        Execute();
    }
}
