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
                viewModel.BulkItemsRightClick(AllItemsListbox.SelectedItems.Cast<RustItem>().ToList());
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

        private void showDlcCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            if (t != null)
            {
                t.Stop();
                t.Start();
            }
        }

        private void T_Elapsed(object? sender, ElapsedEventArgs e)
        {
            t.Stop();
            Dispatcher.Invoke(() =>
            {
                var filterText = filtertxt.Text.Trim();
                bool showDlc = showDlcCheckbox.IsChecked ?? true;
                bool dlcOnly = filterText.Equals("dlc", StringComparison.OrdinalIgnoreCase);

                CollectionViewSource.GetDefaultView(AllItemsListbox.ItemsSource).Filter = (o) =>
                {
                    RustItem item = (RustItem)o;

                    if (!showDlc && item.isDLC == true) return false;

                    if (dlcOnly) return item.isDLC == true;

                    if (String.IsNullOrEmpty(filterText)) return true;

                    return (item.displayName?.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                           (item.shortName?.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                           (item.category?.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);
                };
            });
        }

        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.ItemLeftClick((sender as Grid).DataContext as RustItem);
        }
    }
}
