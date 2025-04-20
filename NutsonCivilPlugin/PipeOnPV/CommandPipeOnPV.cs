using System;

namespace NutsonCivilPlugin.PipeOnPV;

class CommandPipeOnPV : System.Windows.Input.ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter)
    {
        try
        {
            var formWork = new FormWorkWithPipe();
            formWork.Show();
        }
        catch (System.Exception) { }
    }
}
