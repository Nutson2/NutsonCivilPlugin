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

            
            if (Pipes.SelectedItems.Count>0)
            {
                PipeOnPV.SetPartFamily(Pipes.SelectedItems,(string)cmbxFamilyType.SelectedItem,String.Empty);
            }
            else
            {
                List<Model> list = new List<Model>();
                ListBoxItem a = (ListBoxItem)Pipes.ContainerFromElement(cmbxFamilyType);
                list.Add((Model)a.Content);
                PipeOnPV.SetPartFamily(list, (string)cmbxFamilyType.SelectedItem, String.Empty);
            }


            this.Hide();
            this.Show();

        }

        private void pipePartSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmbxPartSize = (ComboBox)sender;

            ListBoxItem lbxItem = (ListBoxItem)Pipes.ContainerFromElement(cmbxPartSize);

            Model el = (Model)lbxItem.Content;

            if (Pipes.SelectedItems.Count>0)
            {
                PipeOnPV.SetPartFamily(Pipes.SelectedItems, el.PartFamily, (string)cmbxPartSize.SelectedItem);

            }
            else
            {
                List<Model> list = new List<Model>();
                list.Add(el);

                PipeOnPV.SetPartFamily(list, el.PartFamily, (string)cmbxPartSize.SelectedItem);
            }

            this.Hide();
            this.Show();

        }
    }
}
