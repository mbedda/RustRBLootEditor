﻿using Prism.Commands;
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
            MainViewModel.ShowBulkLootItemEditor(MainViewModel.LootTableFile.LootItems.Where(s => s.category == group).ToList());
        }

        public void RemoveLootTableItem(LootItem item)
        {
            MainViewModel.RemoveLootTableItem(item);
        }

        public void ShowLootItemEditor(LootItem lootItem)
        {
            MainViewModel.ShowLootItemEditor(lootItem);
        }

        public void ShowBulkLootItemEditor(List<LootItem> lootItems)
        {
            MainViewModel.ShowBulkLootItemEditor(lootItems);
        }

        public void ShowBulkLootItemMultiplier(List<LootItem> lootItems)
        {
            MainViewModel.ShowBulkLootItemMultiplier(lootItems);
        }
    }
}
