using RustRBLootEditor.Models;
using RustRBLootEditor.ViewModels;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RustRBLootEditor.Converters
{
    public class RelativeUriFromLootItemMultiConverter : IMultiValueConverter
    {
        private static readonly System.Collections.Generic.Dictionary<string, BitmapImage> _imageCache = new System.Collections.Generic.Dictionary<string, BitmapImage>();

        public static void ClearCache(string specificPath = null)
        {
            if (specificPath != null)
            {
                _imageCache.Remove(specificPath);
            }
            else
            {
                _imageCache.Clear();
            }
        }

        private BitmapImage GetCachedImage(string path, UriKind uriKind, bool checkFileExists = true)
        {
            if (_imageCache.TryGetValue(path, out var image))
                return image;
            
            if (checkFileExists && !File.Exists(path))
            {
                _imageCache[path] = null; // cache the miss
                return null;
            }

            try
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path, uriKind);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                bmp.Freeze();
                _imageCache[path] = bmp;
                return bmp;
            }
            catch
            {
                _imageCache[path] = null;
                return null;
            }
        }

        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length < 2) return DependencyProperty.UnsetValue;

            LootItem item = values[1] as LootItem;
			if (item != null)
            {
				MainViewModel vm = ((MainWindow)Application.Current.MainWindow).viewModel;

				if(item.skin > 0)
                {
                    string skinpath = Path.Combine(vm.SteamPath, "steamapps\\workshop\\content\\252490");

                    skinpath = Path.Combine(skinpath, item.skin.ToString(), "Icon.png");

                    var skinBmp = GetCachedImage(skinpath, UriKind.Absolute);
                    if (skinBmp != null)
                    {
                        return skinBmp;
                    }
                    else
                    {
                        string temppath = Path.Combine(vm.ExePath, "Assets", "temp", $"{item.skin}.jpg");
                        var tempBmp = GetCachedImage(temppath, UriKind.Absolute);
                        if (tempBmp != null)
                        {
                            return tempBmp;
                        }
                    }
                }

                var rustItem = vm.AllItems.GetRustItem(item.shortname);
                if (rustItem != null)
                    return rustItem.ImageSource;

                string partialpath = "";

                if (parameter != null && parameter.ToString() != null) { partialpath = parameter.ToString(); }

                string imagepath = Path.Combine(vm.ExePath, partialpath, item.shortname);

                if (!imagepath.EndsWith(".png") && !imagepath.EndsWith(".jpg") && !imagepath.EndsWith(".jpeg"))
                {
                    imagepath = imagepath + ".png";
                }

                var cachedImg = GetCachedImage(imagepath, UriKind.RelativeOrAbsolute);
                if (cachedImg != null)
                {
                    return cachedImg;
                }
                else
                {
                    var fallback = GetCachedImage("/RustRBLootEditor;component/Assets/unavailable.png", UriKind.Relative, false);
                    return fallback ?? new BitmapImage(new Uri("/RustRBLootEditor;component/Assets/unavailable.png", UriKind.Relative));
                }
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string[] splitValues = ((string)value).Split(' ');
            return splitValues;
        }
    }
}
