using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NutsonCivilPlugin.PipeOnPV
{
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
            this.DataContext = PipeOnPV;

            Pipes.ItemsSource = PipeOnPV.modelPipes;
            Structures.ItemsSource = PipeOnPV.modelStructures;
        }

        private void BtSelectPV_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            PipeOnPV.GetNetworkPartsFromPV();

            this.Show();
        }

        private void FamilyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmbxFamilyType = (ComboBox)sender;
            ListBox listBox;

            if (cmbxFamilyType.Name== "pipeFamilyType")
            {
                listBox = Pipes;
            }
            else  
            {
                listBox = Structures;
            }

            Model el = (Model)((ListBoxItem)listBox.ContainerFromElement(cmbxFamilyType)).Content;
            if (!listBox.SelectedItems.Contains(el)) { listBox.SelectedItems.Add(el); }

            foreach (Model modelItem in listBox.SelectedItems)
            {
                modelItem.PartFamily = (string)cmbxFamilyType.SelectedItem;
                var cmbxPartSize=((ComboBox)((StackPanel)cmbxFamilyType.Parent).Children[5]).ItemsSource;
                modelItem.PartSize = modelItem.ListPartSizes[0];
            }

        }

        private void PartSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmbxPartSize = (ComboBox)sender;
            ListBox listBox;

            if (cmbxPartSize.Name == "pipePartSize")
            {
                listBox = Pipes;
            }
            else
            {
                listBox = Structures;
            }

            Model el = (Model)((ListBoxItem)listBox.ContainerFromElement(cmbxPartSize)).Content;
            if (!listBox.SelectedItems.Contains(el)) { listBox.SelectedItems.Add(el); }

            PipeOnPV.SetPartFamily(listBox.SelectedItems, el.PartFamily, (string)cmbxPartSize.SelectedItem);


        }

    }
}
