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
using RustRBLootEditor.Helpers;
using System.Diagnostics;
using System.Timers;

namespace RustRBLootEditor.UserControls
{
    /// <summary>
    /// Interaction logic for RustItemsUC.xaml
    /// </summary>
    public partial class RustItemsUC : UserControl
    {
        RustItemsUCViewModel viewModel;

        public RustItemsUC()
        {
            InitializeComponent();
            t = new Timer(200);
            t.Elapsed += T_Elapsed;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = new RustItemsUCViewModel(MainViewModel);
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
            DependencyProperty.Register("MainViewModel", typeof(MainViewModel), typeof(RustItemsUC), new PropertyMetadata(default(MainViewModel)));


        #endregion

        private void Grid_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (AllItemsListbox.SelectedItems.Count < 2)
            {
                Common.GetChildOfType<RustLoadingUC>((sender as Grid)).BeginStoryboard((sender as Grid).DataContext);
                //viewModel.ItemRightClick((sender as Grid).DataContext as RustItem);
            }
            else
            {
                for (int i = 0; i < AllItemsListbox.SelectedItems.Count; i++)
                {
                    viewModel.ItemRightClick(AllItemsListbox.SelectedItems[i] as RustItem);
                }
            }
        }

        private void rustLoadingUC_Animation_Completed(object sender, EventArgs e)
        {
            RustItem rustItem = (sender as RustItem);

            viewModel.ItemRightClick(rustItem);
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
                CollectionViewSource.GetDefaultView(AllItemsListbox.ItemsSource).Filter = (o) =>
                {
                    if (String.IsNullOrEmpty(filtertxt.Text)) return true;
                    RustItem item = (RustItem)o;
                    return (item.displayName.ToLower() + item.shortName.ToLower()).Contains(filtertxt.Text.Trim().ToLower());
                };
            });
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.ItemLeftClick((sender as Grid).DataContext as RustItem);
        }
    }
}
