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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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


            if (listBox.SelectedItems.Count>0)
            {
                PipeOnPV.SetPartFamily(listBox.SelectedItems,(string)cmbxFamilyType.SelectedItem,String.Empty);
            }
            else
            {
                Model el = (Model)((ListBoxItem)listBox.ContainerFromElement(cmbxFamilyType)).Content;
                PipeOnPV.SetPartFamily(new List<Model> { el }, (string)cmbxFamilyType.SelectedItem, String.Empty);
            }


            this.Hide();
            this.Show();

        }

        private void partSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

            ListBoxItem lbxItem = (ListBoxItem)listBox.ContainerFromElement(cmbxPartSize);
            Model el = (Model)lbxItem.Content;

            if (listBox.SelectedItems.Count>0)
            {
                PipeOnPV.SetPartFamily(listBox.SelectedItems, el.PartFamily, (string)cmbxPartSize.SelectedItem);
            }
            else
            {
                PipeOnPV.SetPartFamily(new List<Model> { el }, el.PartFamily, (string)cmbxPartSize.SelectedItem);
            }

            this.Hide();
            this.Show();

        }
    }
}
