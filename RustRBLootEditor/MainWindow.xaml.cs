using Microsoft.Win32;
using RustRBLootEditor.Helpers;
using RustRBLootEditor.ViewModels;
using System;
using System.Reflection;
using System.Windows;

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

            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            Title = Title + " - v" + version.Major + "." + version.Minor + "." + version.Build.ToString();
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

        private void LootItemEdit_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            viewModel.HideLootItemEditor();
        }

        private void GameItemEdit_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            viewModel.HideGameItemEditor();
        }

        private void SaveGameItems_Click(object sender, RoutedEventArgs e)
        {
            Common.SaveJson(viewModel.AllItems.Items, "Assets\\items.json");
        }
    }
}
