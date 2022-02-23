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

        private CollectionView allItemsCollectionView;
        public CollectionView AllItemsCollectionView
        {
            get { return allItemsCollectionView; }
            set { SetProperty(ref allItemsCollectionView, value); }
        }

        private CollectionView lootTableItemsCollectionView;
        public CollectionView LootTableItemsCollectionView
        {
            get { return lootTableItemsCollectionView; }
            set { SetProperty(ref lootTableItemsCollectionView, value); }
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

        public MainViewModel()
        {
            UpdateAllItemsCollectionView();
            UpdateLootTableItemsCollectionView();

            if (AllItems == null)
                AllItems = new RustItems();

            Status = "No file loaded...";
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

            //LootTableItemsCollectionView.Refresh();
            //UpdateLootTableItemsCollectionView();
            UpdateStatus();
        }

        public void Save(string filepath)
        {
            //if (DataChanged)
            //{
            Common.SaveJson(LootTableFile.LootItems, filepath);
            //}
        }

        public void UpdateAllItemsCollectionView()
        {
            if (AllItems == null)
                AllItems = new RustItems();

            AllItemsCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(AllItems.Items);
            AllItemsCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("category"));
        }

        public void UpdateLootTableItemsCollectionView()
        {
            if (LootTableFile == null)
                LootTableFile = new LootTableFile();

            LootTableItemsCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(LootTableFile.LootItems);
            LootTableItemsCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("category"));
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
            if(lootTableFile!=null && lootTableFile.LootItems != null)
            {
                var tmpitem = lootTableFile.LootItems.FirstOrDefault(s => s.shortname == rustItem.shortName);

                if (tmpitem == null)
                {
                    LootTableFile.LootItems.Add(new LootItem()
                    {
                        shortname = rustItem.shortName,
                        displayName = rustItem.displayName,
                        category = rustItem.category
                    });

                    UpdateStatus();
                }
                else
                {
                    MessageBox.Show("Loot table already contains this item.", "Duplicate Item");
                }
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
    }
}
