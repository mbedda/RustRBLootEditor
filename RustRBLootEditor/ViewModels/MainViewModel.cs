using Prism.Commands;
using Prism.Mvvm;
using RustRBLootEditor.Helpers;
using RustRBLootEditor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RustRBLootEditor.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private RustItems allItems;
        public RustItems AllItems
        {
            get { return allItems; }
            set { SetProperty(ref allItems, value); }
        }

        private LootTableFile lootTableFile;
        public LootTableFile LootTableFile
        {
            get { return lootTableFile; }
            set { SetProperty(ref lootTableFile, value); }
        }

        private LootItem _SelectedEditItem;
        public LootItem SelectedEditItem
        {
            get { return _SelectedEditItem; }
            set { SetProperty(ref _SelectedEditItem, value); }
        }

        private bool _LootItemEditorOn;
        public bool LootItemEditorOn
        {
            get { return _LootItemEditorOn; }
            set { SetProperty(ref _LootItemEditorOn, value); }
        }

        private string _SelectedEditGroup;
        public string SelectedEditGroup
        {
            get { return _SelectedEditGroup; }
            set { SetProperty(ref _SelectedEditGroup, value); }
        }

        private LootItem _TempBulkEditItem;
        public LootItem TempBulkEditItem
        {
            get { return _TempBulkEditItem; }
            set { SetProperty(ref _TempBulkEditItem, value); }
        }

        private bool _BulkLootItemEditorOn;
        public bool BulkLootItemEditorOn
        {
            get { return _BulkLootItemEditorOn; }
            set { SetProperty(ref _BulkLootItemEditorOn, value); }
        }

        public DelegateCommand ApplyBulkCommand { get; set; }
        public DelegateCommand CancelBulkCommand { get; set; }

        private RustItem _SelectedEditGameItem;
        public RustItem SelectedEditGameItem
        {
            get { return _SelectedEditGameItem; }
            set { SetProperty(ref _SelectedEditGameItem, value); }
        }

        private bool _GameItemEditorOn;
        public bool GameItemEditorOn
        {
            get { return _GameItemEditorOn; }
            set { SetProperty(ref _GameItemEditorOn, value); }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private string filename;
        public string Filename
        {
            get { return filename; }
            set { SetProperty(ref filename, value); }
        }

        private string bgName;
        public string BgName
        {
            get { return bgName; }
            set { SetProperty(ref bgName, value); }
        }

        public MainViewModel()
        {
            if (AllItems == null)
                AllItems = new RustItems();

            LootTableFile = new LootTableFile();

            BgName = "rust-bg.jpg";

            //Common.DownloadImages(AllItems.Items.ToList(), "Assets\\RustItems\\");

            Status = "No file loaded...";

            ApplyBulkCommand = new DelegateCommand(ApplyBulk);
            CancelBulkCommand = new DelegateCommand(CancelBulk);
        }

        public void LoadFile(string filepath)
        {
            LootTableFile = new LootTableFile();

            List<LootItem> tmpLootItems = Common.LoadJson<List<LootItem>>(filepath);

            if (LootTableFile.LootItems != null)
            {
                foreach (var item in tmpLootItems)
                {
                    RustItem tmpItem = AllItems.GetRustItem(item.shortname);

                    if (tmpItem != null)
                    {
                        item.category = tmpItem.category;
                        item.displayName = tmpItem.displayName;
                    }
                    else
                    {
                        item.category = "Misc";
                        item.displayName = item.shortname;
                    }
                }

                LootTableFile.LootItems = new ObservableCollection<LootItem>(tmpLootItems);
            }

            UpdateStatus();
        }

        public void Save(string filepath)
        {
            Common.SaveJsonNewton(LootTableFile.LootItems, filepath);
        }

        public void UpdateStatus()
        {
            if (LootTableFile == null || LootTableFile.LootItems == null)
            {
                Status = "No file loaded...";
            }
            else
            {
                Status = "Loot table file imported.. " + LootTableFile.LootItems.Count() + " items";
            }
        }

        public void AddLootTableItem(RustItem rustItem)
        {
            if (lootTableFile != null && lootTableFile.LootItems != null)
            {
                var tmpitem = lootTableFile.LootItems.FirstOrDefault(s => s.shortname == rustItem.shortName);

                if (tmpitem != null)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Loot table already contains this item. Are you sure you would like to add?", "Duplicate Notice", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.No) return;
                }

                LootTableFile.LootItems.Add(new LootItem()
                {
                    shortname = rustItem.shortName,
                    displayName = rustItem.displayName,
                    category = rustItem.category
                });

                UpdateStatus();
            }
        }

        public void RemoveLootTableItem(LootItem lootItem)
        {
            if (lootTableFile != null && lootTableFile.LootItems != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you would like to delete this item from the Loot Table?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    lootTableFile.LootItems.Remove(lootItem);
                }
            }
        }

        public void ShowLootItemEditor(LootItem lootItem)
        {
            SelectedEditItem = lootItem;
            LootItemEditorOn = true;
        }
        public void HideLootItemEditor()
        {
            LootItemEditorOn = false;
        }
        public void ShowBulkLootItemEditor(string group)
        {
            SelectedEditGroup = group;
            TempBulkEditItem = new LootItem() { category = group };
            BulkLootItemEditorOn = true;
        }
        public void HideBulkLootItemEditor()
        {
            BulkLootItemEditorOn = false;
        }
        private void ApplyBulk()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you would like to overwrite values for all items in " + TempBulkEditItem.category + "?", "Bulk Edit Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                var groupItems = lootTableFile.LootItems.Where(s => s.category == TempBulkEditItem.category);

                foreach (var item in groupItems)
                {
                    item.amount = TempBulkEditItem.amount;
                    item.amountMin = TempBulkEditItem.amountMin;
                    item.probability = TempBulkEditItem.probability;
                    item.stacksize = TempBulkEditItem.stacksize;
                }

                TempBulkEditItem = new LootItem();

                BulkLootItemEditorOn = false;
            }
        }
        private void CancelBulk()
        {
            TempBulkEditItem = new LootItem();
            BulkLootItemEditorOn = false;
        }
        public void ShowGameItemEditor(RustItem item)
        {
            SelectedEditGameItem = item;
            GameItemEditorOn = true;
        }
        public void HideGameItemEditor()
        {
            GameItemEditorOn = false;
        }
    }
}
