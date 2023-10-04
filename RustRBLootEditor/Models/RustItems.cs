using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
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

        public async Task Load()
        {
            string debugpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string jsonpath = Path.Combine(debugpath, "Assets", "items.json");

            if (File.Exists(jsonpath))
            {
                List<RustItem> items = await Common.LoadJsonAsync<List<RustItem>>(jsonpath);

                Items = new ObservableCollection<RustItem>(items.OrderBy(x => x.displayName));
            }

            //Items = new ObservableCollection<RustItem>(Common.LoadJsonResource<List<RustItem>>("RustRBLootEditor.Assets.items.json"));
            //SyncShortnames();
        }

        public void SyncShortnames()
        {
            foreach(var item in _defaults)
            {
                if(Items.FirstOrDefault(s=>s.shortName == item.Key) == null)
                {
                    Items.Add(new RustItem() { category = "Unknown", shortName = item.Key, displayName = item.Key, image = new Uri("https://rustlabs.com/img/items180/" + item.Key + ".png") });
                }
            }

            for (int i = Items.Count-1; i >= 0; i--)
            {
                if (!_defaults.ContainsKey(Items[i].shortName))
                {
                    Items.RemoveAt(i);
                }
            }
        }

        private readonly Dictionary<string, int> _defaults = new Dictionary<string, int>
        {
        };
    }

    public class RustItem : BindableBase
    {
        private string _id;
        public string id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private Uri _image;
        public Uri image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

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

        BitmapSource _src = null;
        static readonly BitmapImage noImage = new BitmapImage(new Uri("/RustRBLootEditor;component/Assets/unavailable.png", UriKind.Relative));
        public ImageSource Source
        {
            get
            {
                if (_src is null && File.Exists($"F:\\Games\\steamapps\\common\\Rust\\Bundles\\items\\{shortName}.png"))
                {
                    using FileStream fs = new FileStream($"F:\\Games\\steamapps\\common\\Rust\\Bundles\\items\\{shortName}.png", FileMode.Open);
                    using Image source = new Bitmap(fs);
                    using Image destination = new Bitmap(64, 64);

                    using (var g = Graphics.FromImage(destination))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                        g.DrawImage(source, new Rectangle(0, 0, 64, 64), new Rectangle(0, 0, (int)source.Width, (int)source.Height), GraphicsUnit.Pixel);
                    }
                    var stream = new MemoryStream();
                    destination.Save(stream, ImageFormat.Png);

                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.EndInit();

                    var i = image.Clone();
                    i.Freeze();

                    _src = i;
                }
                else if (_src is null)
                    _src = noImage;
                return _src;
            }
        }
    }
}
