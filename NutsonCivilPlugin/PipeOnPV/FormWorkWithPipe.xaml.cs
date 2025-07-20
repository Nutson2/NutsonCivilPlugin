using System.Windows;

namespace NutsonCivilPlugin.PipeOnPV;

/// <summary>
/// Форма для работы с трубами на виде профиля
/// </summary>
public partial class FormWorkWithPipe : Window
{
    private readonly ViewModelPipeOnPV _vm;

    /// <summary>
    /// Инициализирует новый экземпляр класса FormWorkWithPipe
    /// </summary>
    /// <param name="vm">Модель представления для работы с трубами на виде профиля</param>
    public FormWorkWithPipe(ViewModelPipeOnPV vm)
    {
        InitializeComponent();
        _vm = vm;
        _vm.BeforeRequestUserPick += Hide;
        _vm.AfterRequestUserPick += Show;
        _vm.OnClosing += OnClosing;
        DataContext = vm;
    }

    /// <summary>
    /// Вызывается при закрытии формы
    /// </summary>
    /// <param name="e">Аргументы события</param>
    protected override void OnClosed(EventArgs e)
    {
        _vm.BeforeRequestUserPick -= Hide;
        _vm.AfterRequestUserPick -= Show;
        _vm.OnClosing -= OnClosing;

        base.OnClosed(e);
    }
}
