using Microsoft.Win32;
using RustRBLootEditor.Helpers;
using RustRBLootEditor.Models;
using RustRBLootEditor.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

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

        public void LoadBG()
        {
            string debugpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string imagepath = Path.Combine(debugpath, "Assets", viewModel.BgName);
            MemoryStream ms = new MemoryStream();
            BitmapImage bi = new BitmapImage();

            byte[] arrbytFileContent = File.ReadAllBytes(imagepath);
            ms.Write(arrbytFileContent, 0, arrbytFileContent.Length);
            ms.Position = 0;
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            MainGridBrush.ImageSource = bi;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBG();
            viewModel.LoadGameItems();
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
            Button button = sender as Button;
            ContextMenu contextMenu = button.ContextMenu;
            contextMenu.PlacementTarget = button;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }


        private void ExportFileEN_Click(object sender, RoutedEventArgs e)
        {
            ExportLootTable("EN");
        }

        private void ExportFileRU_Click(object sender, RoutedEventArgs e)
        {
            ExportLootTable("RU");
        }

        private void ExportLootTable(string lang = "EN")
        {
            if (!viewModel.ValidateDLCsFree())
                MessageBox.Show("This loot table has paid content that might be against Facepunch's TOS (https://facepunch.com/legal/servers).", "DLC Warning");

            if (!viewModel.ValidateProbability())
                MessageBox.Show("This loot table has more than 80% of its items with probability less than 0.9 or amount of 0, this may cause raid bases to not spawn enough items.", "Probability Warning");

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON file (*.json)|*.json";

            if (saveFileDialog.ShowDialog() == true)
            {
                viewModel.Filename = "(" + System.IO.Path.GetFileName(saveFileDialog.FileName.Trim()) + ")";
                viewModel.Save(saveFileDialog.FileName, lang);
            }
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
            Common.SaveJsonNewton(viewModel.AllItems.Items, "Assets\\items.json");
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                Border rectangle = sender as Border;
                ContextMenu contextMenu = rectangle.ContextMenu;
                //contextMenu.PlacementTarget = rectangle;
                //contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
                contextMenu.IsOpen = true;
            }
        }

        private void ChangeBGMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Filter = "Images Files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg" +
            "|PNG Portable Network Graphics (*.png)|*.png" +
            "|JPEG File Interchange Format (*.jpg *.jpeg)|*.jpg;*.jpeg";

            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                //FilePathTB.Text = openFileDlg.FileName;
                //viewModel.Filename = "(" + System.IO.Path.GetFileName(openFileDlg.FileName.Trim()) + ")";
                string debugpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string imagepath = Path.Combine(debugpath, "Assets", viewModel.BgName);

                File.Copy(openFileDlg.FileName, imagepath, true);
                LoadBG();
            }
        }

        private void RBMainWindow_Initialized(object sender, EventArgs e)
        {
            if (Width > SystemParameters.MaximizedPrimaryScreenWidth || Height > SystemParameters.MaximizedPrimaryScreenHeight)
            {
                Width = SystemParameters.MaximizedPrimaryScreenWidth;
                Height = SystemParameters.MaximizedPrimaryScreenHeight;
                WindowState = WindowState.Maximized;
            }
        }
    }
}
