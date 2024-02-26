using HtmlAgilityPack;
using Prism.Mvvm;
using RustRBLootEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Security.Cryptography.X509Certificates;

namespace RustRBLootEditor.Models
{
    public class RustItems : BindableBase
    {
        public RustItems()
        {
            Items = new ObservableCollection<RustItem>(); 
        }

        private ObservableCollection<RustItem> _Items;
        public ObservableCollection<RustItem> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value); }
        }

        public RustItem GetRustItem(string shortname)
        {
            return Items.FirstOrDefault(s => s.shortName == shortname);
        }

        public async Task Load(string steampath)
        {
            string debugpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string jsonpath = Path.Combine(debugpath, "Assets", "items.json");

            List<RustItem> items = new List<RustItem>();

            if (File.Exists(jsonpath))
            {
                items = await Common.LoadJsonAsync<List<RustItem>>(jsonpath);
            }

            await FetchNewItems(steampath, items);

            await LoadImages(steampath, items);

            Items = new ObservableCollection<RustItem>(items.OrderBy(x => x.displayName));
        }

        public async Task FetchNewItems(string steampath, List<RustItem> currentItems)
        {
            if (string.IsNullOrEmpty(steampath)) return;

            string itemsDirectory = Path.Combine(steampath, "steamapps\\common\\Rust\\Bundles\\items");

            if (!Directory.Exists(itemsDirectory)) return;

            List<string> itemfiles = Directory.EnumerateFiles(itemsDirectory, "*.png").ToList();

            bool NewItemsFound = false;

            foreach (var item in itemfiles.ToList())
            {
                string shortname = Path.GetFileName(item).Replace(".png", "");

                if (File.Exists(item.Replace(".png", ".json")) && !currentItems.Any(s => s.shortName == shortname))
                {
                    BundleItem bundleItem = await Common.LoadJsonAsync<BundleItem>(item.Replace(".png", ".json"));

                    if (string.IsNullOrEmpty(bundleItem.Name) || (!bundleItem.isWearable && !bundleItem.isUsable && !bundleItem.isHoldable) || bundleItem.ItemType == "Liquid") continue;

                    await ResizeAndSaveImageFromSteam(shortname, steampath);

                    currentItems.Add(new RustItem()
                    {
                        shortName = bundleItem.shortname,
                        category = bundleItem.Category,
                        displayName = bundleItem.Name
                    });

                    NewItemsFound = true;
                }
            }

            if (NewItemsFound)
            {
                string debugpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string jsonpath = Path.Combine(debugpath, "Assets", "items.json");

                Common.SaveJsonNewton(currentItems, jsonpath);
            }
        }

        public async Task ResizeAndSaveImageFromSteam(string shortname, string steampath)
        {
            string debugpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string itempath = Path.Combine(debugpath, "Assets", "RustItems", $"{shortname}.png");
            if (File.Exists(itempath)) return;

            string itemSteamPath = Path.Combine(steampath, $"steamapps\\common\\Rust\\Bundles\\items\\{shortname}.png");
            if (!File.Exists(itemSteamPath)) return;

            using FileStream fs = new FileStream(itemSteamPath, FileMode.Open);
            using System.Drawing.Image source = new Bitmap(fs);
            using System.Drawing.Image destination = new Bitmap(180, 180);

            using (var g = Graphics.FromImage(destination))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.DrawImage(source, new Rectangle(0, 0, 180, 180), new Rectangle(0, 0, (int)source.Width, (int)source.Height), GraphicsUnit.Pixel);
            }
            var stream = new MemoryStream();
            destination.Save(itempath, ImageFormat.Png);
        }

        public async Task LoadImages(string steampath, List<RustItem> currentItems)
        {
            foreach (var item in currentItems)
            {
                BitmapSource _src = null;
                string debugpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string itemImagePath = Path.Combine(debugpath, "Assets", "RustItems", $"{item.shortName}.png");

                BitmapImage noImage = new BitmapImage(new Uri("/RustRBLootEditor;component/Assets/unavailable.png", UriKind.Relative));

                if (!File.Exists(itemImagePath))
                {
                    await ResizeAndSaveImageFromSteam(item.shortName, steampath);
                }

                if (File.Exists(itemImagePath))
                {
                    item.ImageSource = await Task.Run(() =>
                    {
                        using (var fileStream = new FileStream(
                            itemImagePath, FileMode.Open, FileAccess.Read))
                        {
                            return BitmapFrame.Create(
                                fileStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        }
                    });
                }
                else
                {
                    item.ImageSource = noImage;
                }
            }
        }
    }

    public class RustItem : BindableBase
    {
        private string _displayName;
        public string displayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        private string _shortName;
        public string shortName
        {
            get { return _shortName; }
            set { SetProperty(ref _shortName, value); }
        }

        private string _category;
        public string category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }

        public ImageSource ImageSource { get; set; } 
    }
}
