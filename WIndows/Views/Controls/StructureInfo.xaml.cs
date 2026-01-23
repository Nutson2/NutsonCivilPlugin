using System.Windows;
using System.Windows.Controls;

namespace Windows.Views.Controls;

/// <summary>
/// Логика взаимодействия для StructureInfo.xaml
/// </summary>
public partial class StructureInfo : UserControl
{
    public StructureInfo()
    {
        InitializeComponent();
    }

    private void DataGridCell_Selected(object sender, RoutedEventArgs e)
    {
        // Lookup for the source to be DataGridCell
        if (e.OriginalSource.GetType() == typeof(DataGridCell))
        {
            var grd = (DataGrid)sender;
            grd.BeginEdit(e);
        }
    }
}
