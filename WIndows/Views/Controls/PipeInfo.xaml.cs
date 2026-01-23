using System.Windows;
using System.Windows.Controls;
using Windows.Models;
using Windows.ViewModels;

namespace Windows.Views.Controls;

/// <summary>
/// Логика взаимодействия для PipeInfo.xaml
/// </summary>
public partial class PipeInfo : UserControl
{
    public PipeInfo()
    {
        InitializeComponent();
    }

    private void DataGridCell_Selected(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource.GetType() == typeof(DataGridCell))
        {
            var grd = (DataGrid)sender;
            var vm = grd.DataContext as PipesViewModel;
            vm.SelectedItems = grd.SelectedCells.Select(c => c.Item).OfType<PipeVM>();
            grd.BeginEdit(e);
        }
    }
}
