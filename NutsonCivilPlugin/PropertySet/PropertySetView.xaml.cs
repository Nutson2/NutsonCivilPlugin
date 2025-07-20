using System.Windows;

namespace NutsonCivilPlugin.PropertySet;

/// <summary>
/// Представление для работы с наборами свойств
/// </summary>
public partial class PropertySetView : Window
{
    private readonly PropertySetsManagerViewModel propertySetsManager;

    /// <summary>
    /// Инициализирует новый экземпляр класса PropertySetView
    /// </summary>
    public PropertySetView()
    {
        InitializeComponent();
        propertySetsManager = new PropertySetsManagerViewModel();
        DataContext = propertySetsManager;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (listParamSetName.SelectedItem == null)
        {
            return;
        }

        propertySetsManager.DeleteSelectedProperty(
            listParamSetName.SelectedItems.Cast<string>().ToList()
        );

        Close();
    }
}
