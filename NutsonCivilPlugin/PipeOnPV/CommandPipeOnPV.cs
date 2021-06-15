using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Windows;

namespace NutsonCivilPlugin.PipeOnPV
{
    class CommandPipeOnPV : System.Windows.Input.ICommand
    {
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            FormWorkWithPipe formWork = new FormWorkWithPipe();
            formWork.Show();

        }
    }

}
