using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Windows.Views.Behaviors;

public static class MultiCellEditBehavior
{
    private const DispatcherPriority background = DispatcherPriority.Background;

    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(MultiCellEditBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged)
        );

    public static bool GetIsEnabled(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsEnabledProperty);
    }

    public static void SetIsEnabled(DependencyObject obj, bool value)
    {
        obj.SetValue(IsEnabledProperty, value);
    }

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DataGrid dataGrid)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            dataGrid.CellEditEnding += DataGrid_CellEditEnding;
        }
        else
        {
            dataGrid.CellEditEnding -= DataGrid_CellEditEnding;
        }
    }

    private static void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction != DataGridEditAction.Commit
            || sender is not DataGrid dataGrid
            || dataGrid.SelectedCells.Count <= 1)
        {
            return;
        }

        var t = e.EditingElement switch
        {
            TextBox textBox => HandleTextBox(textBox, dataGrid, e),
            ComboBox comboBox => HandleComboBox(comboBox, dataGrid, e),
            _ => false
        };

    }

    private static bool HandleTextBox(TextBox textBox, DataGrid dataGrid, DataGridCellEditEndingEventArgs e)
    {
        var newValue = textBox.Text;
        var editedColumn = e.Column;

        if (editedColumn is not DataGridTextColumn textColumn)
        {
            return false;
        }

        var binding = textColumn.Binding as System.Windows.Data.Binding;
        var propertyName = binding?.Path.Path;

        if (propertyName is null || string.IsNullOrEmpty(propertyName))
        {
            return false;
        }

        var cellsToUpdate = dataGrid
            .SelectedCells.Where(cell => cell.Column == editedColumn)
            .ToList();

        if (cellsToUpdate.Count <= 1)
        {
            return false;
        }

        dataGrid.Dispatcher.BeginInvoke(
            () => UpdateCells(cellsToUpdate, propertyName, newValue), background);

        return true;
    }
    private static bool HandleComboBox(ComboBox comboBox, DataGrid dataGrid, DataGridCellEditEndingEventArgs e)
    {
        // var items = dataGrid.Row;
        return true;
    }

    private static void UpdateCells(
        List<DataGridCellInfo> cells,
        string propertyName,
        string newValue
    )
    {
        foreach (var cellInfo in cells)
        {
            var item = cellInfo.Item;
            var property = item.GetType().GetProperty(propertyName);

            if (property != null && property.CanWrite)
            {
                try
                {
                    object convertedValue;

                    // Обработка nullable типов
                    var targetType =
                        Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    if (
                        string.IsNullOrWhiteSpace(newValue)
                        && Nullable.GetUnderlyingType(property.PropertyType) != null
                    )
                    {
                        convertedValue = null;
                    }
                    else
                    {
                        convertedValue = Convert.ChangeType(newValue, targetType);
                    }

                    property.SetValue(item, convertedValue);
                }
                catch (Exception)
                {
                    // Если конвертация не удалась, пытаемся установить как строку
                    if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(item, newValue);
                    }
                }
            }
        }
    }
}
