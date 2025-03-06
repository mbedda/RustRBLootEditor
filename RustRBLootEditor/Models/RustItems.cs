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
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

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

        public async Task Load(string steamPath)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string jsonPath = Path.Combine(appPath, "Assets", "items.json");

            List<RustItem> items = new();

            if (File.Exists(jsonPath))
                items = await Common.LoadJsonAsync<List<RustItem>>(jsonPath);

            await FetchNewItems(appPath, jsonPath, steamPath, items);

            await LoadImages(appPath, steamPath, items);

            Items = new ObservableCollection<RustItem>(items.OrderBy(x => x.displayName));
        }

        private readonly List<string> _blacklistedItems = new()
        {
            "ammo.snowballgun", "spraycandecal", "workcart", "wagon", "trike", "snowmobiletomaha", "submarinesolo", "snowmobile", "scraptransportheli.repair",
            "motorbike_sidecar", "motorbike", "mlrs", "minihelicopter.repair", "locomotive", "habrepair", "submarineduo", "blueprintbase",
            "bicycle", "attackhelicopter", "vehicle.chassis.2mod", "vehicle.chassis.3mod", "vehicle.chassis.4mod", "weaponrack.doublelight", "weaponrack.light"
        };

        private async Task FetchNewItems(string appPath, string jsonPath, string steamPath, List<RustItem> currentItems)
        {
            if (string.IsNullOrEmpty(steamPath)) return;

            string itemsDirectory = Path.Combine(steamPath, "steamapps\\common\\Rust\\Bundles\\items");

            if (!Directory.Exists(itemsDirectory)) return;

            var itemFiles = Directory.EnumerateFiles(itemsDirectory, "*.png");

            bool newItemsFound = false;

            foreach (var item in itemFiles)
            {
                string shortname = Path.GetFileNameWithoutExtension(item);
                
                if(_blacklistedItems.Contains(shortname)) continue;

                if (!currentItems.Any(s => s.shortName == shortname) && File.Exists(item.Replace(".png", ".json")))
                {
                    BundleItem bundleItem = await Common.LoadJsonAsync<BundleItem>(item.Replace(".png", ".json"));

                    if (string.IsNullOrEmpty(bundleItem.Name) || bundleItem.ItemType == "Liquid") continue;
                    
                    await ResizeAndSaveImageFromSteam(appPath, shortname, steamPath);

                    currentItems.Add(new RustItem()
                    {
                        shortName = bundleItem.shortname,
                        category = bundleItem.Category,
                        displayName = bundleItem.Name
                    });

                    newItemsFound = true;
                }
            }

            if (newItemsFound)
            {
                Common.SaveJsonNewton(currentItems, jsonPath);
            }
        }

        private async Task ResizeAndSaveImageFromSteam(string appPath, string shortname, string steamPath)
        {
            string itempath = Path.Combine(appPath, "Assets", "RustItems", $"{shortname}.png");
            if (File.Exists(itempath)) return;

            string itemSteamPath = Path.Combine(steamPath, $"steamapps\\common\\Rust\\Bundles\\items\\{shortname}.png");
            if (!File.Exists(itemSteamPath)) return;

            await using FileStream fs = new FileStream(itemSteamPath, FileMode.Open);
            using Image source = new Bitmap(fs);
            using Image destination = new Bitmap(100, 100);

            using (var g = Graphics.FromImage(destination))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.DrawImage(source, new Rectangle(0, 0, destination.Width, destination.Height), new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            }
            destination.Save(itempath, ImageFormat.Png);
        }

        private async Task LoadImages(string appPath, string steampath, List<RustItem> currentItems)
        {
            foreach (var item in currentItems)
            {
                string itemImagePath = Path.Combine(appPath, "Assets", "RustItems", $"{item.shortName}.png");

                BitmapImage? noImage = new BitmapImage(new Uri("/RustRBLootEditor;component/Assets/unavailable.png", UriKind.Relative));

                if (!File.Exists(itemImagePath))
                {
                    await ResizeAndSaveImageFromSteam(appPath, item.shortName, steampath);
                }

                if (File.Exists(itemImagePath))
                {
                    item.ImageSource = await Task.Run(() =>
                    {
                        var fileStream = new FileStream(itemImagePath, FileMode.Open, FileAccess.Read);
                        try
                        {
                            return BitmapFrame.Create(fileStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        }
                        finally
                        {
                            fileStream.Dispose();
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

        [IgnoreDataMember]
        public ImageSource? ImageSource { get; set; } 
    }
}
