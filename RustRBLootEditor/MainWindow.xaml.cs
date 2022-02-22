using Microsoft.Win32;
using RustRBLootEditor.Models;
using RustRBLootEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RustRBLootEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel viewModel;

        public MainWindow()
        {
            viewModel = new MainViewModel();

            InitializeComponent();

            DataContext = viewModel;
        }

        private void ImportFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FilePathTB.Text = openFileDlg.FileName;
                viewModel.Filename = "(" + System.IO.Path.GetFileName(openFileDlg.FileName.Trim()) + ")";
                viewModel.LoadFile(FilePathTB.Text);
            }
        }

        private void ExportFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON file (*.json)|*.json";

            if (saveFileDialog.ShowDialog() == true)
                viewModel.Save(saveFileDialog.FileName);
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {

        }

        private void AllItemsGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            RustItem rustitem = (sender as Grid).DataContext as RustItem;

            viewModel.AddLootTableItem(rustitem);
        }

        private void LootTableGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            LootItem lootitem = (sender as Grid).DataContext as LootItem;

            viewModel.RemoveLootTableItem(lootitem);
        }
    }
}
