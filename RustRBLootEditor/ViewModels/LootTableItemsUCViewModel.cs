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
    public class LootTableItemsUCViewModel : BindableBase
    {
        private MainViewModel _MainViewModel;
        public MainViewModel MainViewModel
        {
            get { return _MainViewModel; }
            set { SetProperty(ref _MainViewModel, value); }
        }

        public LootTableItemsUCViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }

        public void ItemRightClick(LootItem item)
        {
            MainViewModel.RemoveLootTableItem(item);
        }
    }
}
