using RustRBLootEditor.Models;
using RustRBLootEditor.UI;
using RustRBLootEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RustRBLootEditor.UserControls
{
    /// <summary>
    /// Interaction logic for LootTableItemsUC.xaml
    /// </summary>
    public partial class LootTableItemsUC : UserControl
    {
        LootTableItemsUCViewModel viewModel;

        public LootTableItemsUC()
        {
            InitializeComponent();
            t = new Timer(200);
            t.Elapsed += T_Elapsed;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = new LootTableItemsUCViewModel(MainViewModel);
            DataContext = viewModel;
        }

        #region Dependency Properties


        public MainViewModel MainViewModel
        {
            get { return (MainViewModel)GetValue(MainViewModelProperty); }
            set { SetValue(MainViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Project.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(LootTableItemsUC), new PropertyMetadata(default(MainViewModel)));


        #endregion

        private void Grid_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((sender as Grid).Children[1] as RustLoadingUC).BeginStoryboard((sender as Grid).DataContext);
        }

        private void rustLoadingUC_Animation_Completed(object sender, EventArgs e)
        {
            LootItem lootItem = (sender as LootItem);

            viewModel.RemoveLootTableItem(lootItem);
        }

        Timer t = null;
        private void filtertxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            t.Stop();
            t.Start();
        }

        private void T_Elapsed(object? sender, ElapsedEventArgs e)
        {
            t.Stop();

            Dispatcher.Invoke(() =>
            {
                CollectionViewSource.GetDefaultView(LootTableItemsListbox.ItemsSource).Filter = (o) =>
                {
                    if (String.IsNullOrEmpty(filtertxt.Text)) return true;
                    LootItem item = (LootItem)o;
                    return (item.displayName.ToLower() + item.shortname.ToLower()).Contains(filtertxt.Text.Trim().ToLower());
                };
            });
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.ShowLootItemEditor((sender as Grid).DataContext as LootItem);
        }

        private void LootItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
			{
				LootItem item = (LootItem)((Grid)sender).DataContext;

                viewModel.ShowLootItemEditor(item);
			}
        }

        private void EditLootItem_Click(object sender, RoutedEventArgs e)
        {
            if (LootTableItemsListbox.SelectedItems.Count < 2)
            {
                LootItem item = (LootItem)((MenuItem)sender).CommandParameter;
                viewModel.ShowLootItemEditor(item);
            }
            else
            {
                viewModel.ShowBulkLootItemEditor(LootTableItemsListbox.SelectedItems.Cast<LootItem>().ToList() as List<LootItem>);
            }
        }

        private void MultiplyLootItem_Click(object sender, RoutedEventArgs e)
        {
            if (LootTableItemsListbox.SelectedItems.Count > 0)
            {
                viewModel.ShowBulkLootItemMultiplier(LootTableItemsListbox.SelectedItems.Cast<LootItem>().ToList() as List<LootItem>);
            }
        }

        private void DeleteLootItem_Click(object sender, RoutedEventArgs e)
        {
            if(LootTableItemsListbox.SelectedItems.Count < 2)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you would like to delete selected item?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    LootItem item = (LootItem)((MenuItem)sender).CommandParameter;

                    viewModel.RemoveLootTableItem(item);
                }
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you would like to delete selected items?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    for (int i = LootTableItemsListbox.SelectedItems.Count - 1; i >= 0; i--)
                    {
                        LootItem item = (LootItem)LootTableItemsListbox.SelectedItems[i];

                        viewModel.RemoveLootTableItem(item);
                    }
                }
            }
        }

        private void LootTableItemsListbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (LootTableItemsListbox.SelectedItems.Count > 0)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you would like to delete selected items?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        for (int i = LootTableItemsListbox.SelectedItems.Count - 1; i >= 0; i--)
                        {
                            LootItem item = (LootItem)LootTableItemsListbox.SelectedItems[i];

                            viewModel.RemoveLootTableItem(item);
                        }
                    }
                }
            }
        }
    }
}
