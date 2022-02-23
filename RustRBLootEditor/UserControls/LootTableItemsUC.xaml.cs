using RustRBLootEditor.Models;
using RustRBLootEditor.UI;
using RustRBLootEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            viewModel.ItemRightClick(lootItem);
        }

        private void filtertxt_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ICollectionView itemsViewOriginal = (CollectionView)CollectionViewSource.GetDefaultView(LootTableItemsListbox.ItemsSource);

            if(itemsViewOriginal != null)
            {
                itemsViewOriginal.Filter = ((o) =>
                {
                    if (String.IsNullOrEmpty(filtertxt.Text)) return true;
                    else
                    {
                        if (((LootItem)o).displayName.ToLower().Contains(filtertxt.Text.Trim().ToLower())) return true;
                        else return false;
                    }
                });
            }
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.ItemLeftClick((sender as Grid).DataContext as LootItem);
        }
    }
}
