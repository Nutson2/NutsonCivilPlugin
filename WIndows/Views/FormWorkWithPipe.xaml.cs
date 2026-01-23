using System.Windows;
using NutsonCivilPlugin.PipeOnPV.ViewModels;

namespace Windows.Views;

public partial class FormWorkWithPipe : Window
{
    public FormWorkWithPipe(ViewModelPipeOnPV vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
