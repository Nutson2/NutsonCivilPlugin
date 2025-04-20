using System.Linq;
using System.Windows;

namespace NutsonCivilPlugin.PropertySet;

public partial class PropertySetView : Window
{
    PropertySetsManagerViewModel propertySetsManager;

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
