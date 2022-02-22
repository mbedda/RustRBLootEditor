using Prism.Mvvm;
using RustRBLootEditor.Helpers;
using RustRBLootEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private string activity;
        public string Activity
        {
            get { return activity; }
            set { SetProperty(ref activity, value); }
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

            Activity = "No file loaded...";
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

        public void UpdateActivity()
        {
            if (LootTableFile == null || LootTableFile.LootItems == null)
            {
                Activity = "No file loaded...";
            }
            else
            {
                Activity = "Loot table file imported.. " + LootTableFile.LootItems.Count() + " items";
            }
        }

        public void LoadFile(string filepath)
        {
            LootTableFile = new LootTableFile();

            LootTableFile.LootItems = Common.LoadJson<List<LootItem>>(filepath);

            if (LootTableFile.LootItems != null)
            {
                foreach (var item in LootTableFile.LootItems)
                {
                    RustItem tmpItem = AllItems.GetItemDetailsByShortname(item.shortname);

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

                //LootTableFile.LootItems = tmpLootItems;
            }

            //LootTableItemsCollectionView.Refresh();
            //UpdateLootTableItemsCollectionView();
            UpdateActivity();
        }

        public void Save(string filepath)
        {
            //if (DataChanged)
            //{
            Common.SaveJson(LootTableFile.LootItems, filepath);
            //}
        }

        public void AddLootTableItem(RustItem rustItem)
        {
            if(lootTableFile!=null && lootTableFile.LootItems != null)
            {
                LootTableFile.LootItems.Add(new LootItem()
                {
                    shortname = rustItem.shortName,
                    displayName = rustItem.displayName,
                    category = rustItem.category
                });

                //LootTableItemsCollectionView.Refresh();
                //UpdateLootTableItemsCollectionView();
                UpdateActivity();
            }
        }

        public void RemoveLootTableItem(LootItem lootItem)
        {
            if (lootTableFile != null && lootTableFile.LootItems != null)
            {
                lootTableFile.LootItems.Remove(lootItem);

                //LootTableItemsCollectionView.Refresh();
                ////UpdateLootTableItemsCollectionView();
                //UpdateActivity();
            }
        }
    }
}
