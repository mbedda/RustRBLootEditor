using Prism.Commands;
using Prism.Mvvm;
using RustRBLootEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustRBLootEditor.ViewModels
{
    public class RustItemsUCViewModel : BindableBase
    {
        private MainViewModel _MainViewModel;
        public MainViewModel MainViewModel
        {
            get { return _MainViewModel; }
            set { SetProperty(ref _MainViewModel, value); }
        }

        public RustItemsUCViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }

        public void ItemRightClick(RustItem item)
        {
            MainViewModel.AddLootTableItem(item);
        }
    }
}
