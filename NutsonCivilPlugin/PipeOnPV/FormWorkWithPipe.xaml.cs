using System.Windows;

namespace NutsonCivilPlugin.PipeOnPV;

public partial class FormWorkWithPipe : Window
{
    private readonly ViewModelPipeOnPV _vm;

    public FormWorkWithPipe(ViewModelPipeOnPV vm)
    {
        InitializeComponent();
        _vm = vm;
        _vm.BeforeRequestUserPick += Hide;
        _vm.AfterRequestUserPick += Show;
        _vm.OnClosing += OnClosing;
        DataContext = vm;
    }

    protected override void OnClosed(EventArgs e)
    {
        _vm.BeforeRequestUserPick -= Hide;
        _vm.AfterRequestUserPick -= Show;
        _vm.OnClosing -= OnClosing;

        base.OnClosed(e);
    }
}
