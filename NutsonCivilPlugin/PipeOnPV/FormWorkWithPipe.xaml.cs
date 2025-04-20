using System.Windows;
using System.Windows.Controls;

namespace NutsonCivilPlugin.PipeOnPV;

/// <summary>
/// Логика взаимодействия для FormWorkWithPipe.xaml
/// </summary>
public partial class FormWorkWithPipe : Window
{
    readonly ViewModelPipeOnPV PipeOnPV;

    public FormWorkWithPipe()
    {
        InitializeComponent();
        PipeOnPV = new ViewModelPipeOnPV();
        DataContext = PipeOnPV;

        Pipes.ItemsSource = PipeOnPV.modelPipes;
        Structures.ItemsSource = PipeOnPV.modelStructures;
    }

    private void BtSelectPV_Click(object sender, RoutedEventArgs e)
    {
        Hide();
        try
        {
            PipeOnPV.GetNetworkPartsFromPV();
        }
        catch (Exception ex)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(
                "Ошибочка вышла" + ex.Message + "\n" + ex.StackTrace
            );
        }

        Show();
    }

    private void FamilyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmbxFamilyType = (ComboBox)sender;
        var listBox = cmbxFamilyType.Name == "pipeFamilyType" ? Pipes : Structures;

        var el = (Model)((ListBoxItem)listBox.ContainerFromElement(cmbxFamilyType)).Content;
        if (!listBox.SelectedItems.Contains(el))
        {
            listBox.SelectedItems.Add(el);
        }

        foreach (Model modelItem in listBox.SelectedItems)
        {
            modelItem.PartFamily = (string)cmbxFamilyType.SelectedItem;
            modelItem.PartSize = modelItem.ListPartSize[0];
        }
    }

    private void PartSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmbxPartSize = (ComboBox)sender;
        var listBox = cmbxPartSize.Name == "pipePartSize" ? Pipes : Structures;

        var el = (Model)((ListBoxItem)listBox.ContainerFromElement(cmbxPartSize)).Content;
        if (!listBox.SelectedItems.Contains(el))
        {
            listBox.SelectedItems.Add(el);
        }

        PipeOnPV.SetPartFamily(
            listBox.SelectedItems,
            el.PartFamily,
            (string)cmbxPartSize.SelectedItem
        );
    }
}
