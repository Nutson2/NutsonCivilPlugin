using System.Windows;
using System.Windows.Controls;

namespace NutsonCivilPlugin.PipeOnPV.Views.Controls;

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
        // Lookup for the source to be DataGridCell
        if (e.OriginalSource.GetType() == typeof(DataGridCell))
        {
            // Starts the Edit on the row;
            DataGrid grd = (DataGrid)sender;
            grd.BeginEdit(e);
        }
    }
}
