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
        public DelegateCommand<string> UpdateBulkCommand { get; set; }

        public LootTableItemsUCViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
            UpdateBulkCommand = new DelegateCommand<string>(UpdateBulk);
        }

        private void UpdateBulk(string group)
        {
            MainViewModel.ShowBulkLootItemEditor(group);
        }

        public void ItemRightClick(LootItem item)
        {
            MainViewModel.RemoveLootTableItem(item);
        }

        public void ItemLeftClick(LootItem lootItem)
        {
            MainViewModel.ShowLootItemEditor(lootItem);
        }
    }
}
