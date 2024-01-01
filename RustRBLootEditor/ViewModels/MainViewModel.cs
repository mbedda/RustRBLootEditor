using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using RustRBLootEditor.Helpers;
using RustRBLootEditor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using static System.Net.Mime.MediaTypeNames;

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

        private List<LootItem> _SelectedBulkEditItems;
        public List<LootItem> SelectedBulkEditItems
        {
            get { return _SelectedBulkEditItems; }
            set { SetProperty(ref _SelectedBulkEditItems, value); }
        }

        private LootItem _TempBulkEditItem;
        public LootItem TempBulkEditItem
        {
            get { return _TempBulkEditItem; }
            set { SetProperty(ref _TempBulkEditItem, value); }
        }

        private BulkTargetFields _TempBulkTargetFields;
        public BulkTargetFields TempBulkTargetFields
        {
            get { return _TempBulkTargetFields; }
            set { SetProperty(ref _TempBulkTargetFields, value); }
        }

        private bool _BulkLootItemEditorOn;
        public bool BulkLootItemEditorOn
        {
            get { return _BulkLootItemEditorOn; }
            set { SetProperty(ref _BulkLootItemEditorOn, value); }
        }

        private float _MultiplierValue;
        public float MultiplierValue
        {
            get { return _MultiplierValue; }
            set { SetProperty(ref _MultiplierValue, value); }
        }

        public DelegateCommand ApplyBulkCommand { get; set; }
        public DelegateCommand CancelBulkCommand { get; set; }

        private bool _BulkLootMultiplierOn;
        public bool BulkLootMultiplierOn
        {
            get { return _BulkLootMultiplierOn; }
            set { SetProperty(ref _BulkLootMultiplierOn, value); }
        }

        public DelegateCommand ApplyMultiplierCommand { get; set; }
        public DelegateCommand CancelMultiplierCommand { get; set; }

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

        private bool loadingModal;
        public bool LoadingModal
        {
            get { return loadingModal; }
            set { SetProperty(ref loadingModal, value); }
        }

        private string loadingText;
        public string LoadingText
        {
            get { return loadingText; }
            set { SetProperty(ref loadingText, value); }
        }

        public Dictionary<ulong, string> SkinsUrls { get; set; }

        public string SteamPath { get; set; }

        public MainViewModel()
        {
            LoadingModal = true;
            if (AllItems == null)
                AllItems = new RustItems();

            LootTableFile = new LootTableFile();

            GetSteamPath();

            BgName = "rust-bg.jpg";

            //Common.DownloadImages(AllItems.Items.ToList(), "Assets\\RustItems\\");

            Status = "No file loaded...";

            ApplyBulkCommand = new DelegateCommand(ApplyBulk);
            CancelBulkCommand = new DelegateCommand(CancelBulk);

            ApplyMultiplierCommand = new DelegateCommand(ApplyMultiplier);
            CancelMultiplierCommand = new DelegateCommand(CancelMultiplier);
        }

        public async Task LoadGameItems()
        {
            ShowLoading("Loading Game Files...");

            if (AllItems != null)
                await AllItems.Load(SteamPath);

            HideLoading();
        }

        public void GetSteamPath()
        {
            SteamPath = "";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam");

            if (key != null)
            {
                SteamPath = key.GetValue("InstallPath").ToString();
                key.Close();
            }
            else
            {
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam");

                if (key != null)
                {
                    SteamPath = key.GetValue("InstallPath").ToString();
                    key.Close();
                }
            }
        }

        public async Task LoadFile(string filepath)
        {
            ShowLoading("Loading Loot File...");
            LootTableFile = new LootTableFile();

            List<LootItem> tmpLootItems = await Common.LoadJsonAsync<List<LootItem>>(filepath);

            if (tmpLootItems != null)
            {
                List<ulong> skins = new List<ulong>();

                foreach (var item in tmpLootItems)
                {
                    RustItem tmpItem = AllItems.GetRustItem(item.shortname);

                    if (item.skin > 0)
                        if (!skins.Contains(item.skin))
                            skins.Add(item.skin);

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

                await GetSteamSkins(skins);

                LootTableFile.LootItems.Clear();
                LootTableFile.LootItems.AddRange(tmpLootItems);
                LootTableFile.DoSort();
            }

            UpdateStatus();
            HideLoading();
        }

        public void Save(string filepath)
        {
            Common.SaveJsonNewton(LootTableFile.LootItems, filepath);
        }

        public void ShowLoading(string text)
        {
            LoadingModal = true;
            LoadingText = text;
        }

        public void HideLoading()
        {
            LoadingModal = false;
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
                    MessageBoxResult messageBoxResult = MessageBox.Show($"Loot table already contains the item \"{tmpitem.displayName}\". Are you sure you would like to add?", "Duplicate Notice", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.No) return;
                }

                LootTableFile.LootItems.Add(new LootItem()
                {
                    shortname = rustItem.shortName,
                    displayName = rustItem.displayName,
                    category = rustItem.category,
                    amountMin = 1,
                    amount = 1
                });
                LootTableFile.DoSort();

                UpdateStatus();
            }
        }

        public void RemoveLootTableItem(LootItem lootItem)
        {
            if (lootTableFile != null && lootTableFile.LootItems != null)
            {
                lootTableFile.LootItems.Remove(lootItem);

                UpdateStatus();
            }
        }

        ulong SelectedItemOriginalSkin = 0;

        public void ShowLootItemEditor(LootItem lootItem)
        {
            SelectedItemOriginalSkin = lootItem.skin;
            SelectedEditItem = lootItem;
            LootItemEditorOn = true;

            if (!ValidateStackSize(SelectedEditItem))
            {
                MessageBox.Show("This item amount is more than 10x its stack size, it is recommended to set a higher stack size or set it to -1.", "Stack Size Warning");
            }
        }

        public async void HideLootItemEditor()
        {
            LootItemEditorOn = false;

            if (SelectedEditItem.amountMin > SelectedEditItem.amount)
                SelectedEditItem.amountMin = SelectedEditItem.amount;

            if (SelectedEditItem.stacksize < -1)
                SelectedEditItem.stacksize = -1;

            if (SelectedEditItem.skin != SelectedItemOriginalSkin && SelectedEditItem.skin > 0)
            {
                await GetSteamSkins(new List<ulong>() { SelectedEditItem.skin });

                ulong skin = SelectedEditItem.skin;
                SelectedEditItem.skin = 0;
                SelectedEditItem.skin = skin;
            }
        }

        public void ShowBulkLootItemEditor(List<LootItem> lootItems)
        {
            SelectedBulkEditItems = lootItems;
            TempBulkEditItem = new LootItem();
            TempBulkTargetFields = new BulkTargetFields();
            BulkLootItemEditorOn = true;
        }
        public void HideBulkLootItemEditor()
        {
            BulkLootItemEditorOn = false;
        }
        private void ApplyBulk()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Are you sure you would like to overwrite values for all {SelectedBulkEditItems.Count} items?", "Bulk Edit Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (var item in SelectedBulkEditItems)
                {
                    if (TempBulkTargetFields.amount)
                        item.amount = TempBulkEditItem.amount;

                    if (TempBulkTargetFields.amountMin)
                        item.amountMin = (TempBulkEditItem.amountMin > item.amount) ? item.amount : TempBulkEditItem.amountMin;

                    if (TempBulkTargetFields.probability)
                        item.probability = TempBulkEditItem.probability;

                    if (TempBulkTargetFields.stacksize)
                        item.stacksize = (TempBulkEditItem.stacksize < -1) ? -1 : TempBulkEditItem.stacksize;
                }

                SelectedBulkEditItems = new List<LootItem>();
                TempBulkEditItem = new LootItem();
                TempBulkTargetFields = new BulkTargetFields();

                BulkLootItemEditorOn = false;
            }
        }
        private void CancelBulk()
        {
            SelectedBulkEditItems = new List<LootItem>();
            TempBulkEditItem = new LootItem();
            BulkLootItemEditorOn = false;
        }

        public void ShowBulkLootItemMultiplier(List<LootItem> lootItems)
        {
            MultiplierValue = 1;
            SelectedBulkEditItems = lootItems;
            TempBulkTargetFields = new BulkTargetFields();
            BulkLootMultiplierOn = true;
        }
        public void HideBulkLootItemMultiplier()
        {
            MultiplierValue = 1;
            BulkLootMultiplierOn = false;
        }
        private void ApplyMultiplier()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show($"Are you sure you would like to apply {MultiplierValue}x on all {SelectedBulkEditItems.Count} items?", "Bulk Multiply Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (var item in SelectedBulkEditItems)
                {
                    if (TempBulkTargetFields.amount)
                    {
                        item.amount = (int)Math.Round(item.amount * MultiplierValue);
                    }

                    if (TempBulkTargetFields.amountMin)
                    {
                        item.amountMin = (int)Math.Round(item.amountMin * MultiplierValue);

                        if (item.amountMin > item.amount)
                            item.amountMin = item.amount;
                    }

                    if (TempBulkTargetFields.stacksize)
                    {
                        item.stacksize = (int)Math.Round(item.stacksize * MultiplierValue);

                        if (item.stacksize < -1)
                            item.stacksize = -1;
                    }
                }

                SelectedBulkEditItems = new List<LootItem>();
                TempBulkTargetFields = new BulkTargetFields();

                BulkLootMultiplierOn = false;
            }
        }
        private void CancelMultiplier()
        {
            MultiplierValue = 1;
            SelectedBulkEditItems = new List<LootItem>();
            BulkLootMultiplierOn = false;
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

        public async Task<bool> GetSteamSkins(List<ulong> skinlist)
        {
            if (skinlist.Count == 0)
                return false;

            string exepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string temppath = Path.Combine(exepath, "Assets", "temp");

            foreach (var skin in skinlist.ToList())
            {
                var fileName = Path.Combine(temppath, $"{skin}.jpg");

                if (File.Exists(fileName))
                    skinlist.Remove(skin);
            }

            if (skinlist.Count == 0)
                return false;

            ShowLoading("Fetching Skins...");

            var publishedFileDetails = await SteamApi.GetPublishedFileDetailsAsync(skinlist);

            if (SkinsUrls == null)
                SkinsUrls = new Dictionary<ulong, string>();

            if (publishedFileDetails != null && publishedFileDetails.SteamResponse != null && publishedFileDetails.SteamResponse.Publishedfiledetails != null)
            {
                foreach (var file in publishedFileDetails.SteamResponse.Publishedfiledetails)
                {
                    if (file.PreviewUrl != null && !string.IsNullOrEmpty(file.PreviewUrl.AbsoluteUri) && !SkinsUrls.ContainsKey(ulong.Parse(file.Publishedfileid)))
                    {
                        SkinsUrls.Add(ulong.Parse(file.Publishedfileid), file.PreviewUrl.AbsoluteUri);
                    }
                }
            }

            return DownloadSkins();
        }

        public bool DownloadSkins()
        {
            ShowLoading("Downloading Skins...");
            bool changeOccurred = false;
            string exepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string temppath = Path.Combine(exepath, "Assets", "temp");

            Directory.CreateDirectory(temppath);

            foreach (var skinurl in SkinsUrls)
            {
                var fileName = Path.Combine(temppath, $"{skinurl.Key}.jpg");

                if (!File.Exists(fileName))
                {
                    using (WebClient client = new WebClient())
                    {
                        var address = skinurl.Value;

                        client.DownloadFile(address, fileName);
                        changeOccurred = true;
                    }
                }
            }

            HideLoading();

            return changeOccurred;
        }

        public bool ValidateProbability()
        {
            if (LootTableFile == null || LootTableFile.LootItems == null || LootTableFile.LootItems.Count == 0) return true;

            int highPropCount = LootTableFile.LootItems.Where(s => s.probability >= 0.9f && s.amount > 0).Count();

            float ratio = (float)highPropCount / (float)LootTableFile.LootItems.Count;

            return ratio >= 0.8 ? true : false;
        }

        public bool ValidateStackSize(LootItem item)
        {
            if(item == null) return true;

            float ratio = (float)item.amount / (float)item.stacksize;

            return ratio < 10.0 ? true : false;
        }
    }
}
