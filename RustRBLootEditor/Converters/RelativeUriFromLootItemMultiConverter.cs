using RustRBLootEditor.Helpers;
using RustRBLootEditor.Models;
using RustRBLootEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RustRBLootEditor.Converters
{
    public class RelativeUriFromLootItemMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length < 2) return DependencyProperty.UnsetValue;

            LootItem item = values[1] as LootItem;
			if (item != null)
            {
				MainViewModel vm = ((MainWindow)Application.Current.MainWindow).viewModel;
				string skinpath = Path.Combine(vm.SteamPath, "steamapps\\workshop\\content\\252490");

				skinpath = Path.Combine(skinpath, item.skin.ToString(), "Icon.png");

                if(File.Exists(skinpath))
				{
					return new BitmapImage(new Uri(skinpath, UriKind.Absolute));
				}
                else
				{
					string debugpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					string temppath = Path.Combine(debugpath, "Assets", "temp", $"{item.skin}.jpg");

					if (File.Exists(temppath))
					{
						return new BitmapImage(new Uri(temppath, UriKind.Absolute));
					}

					string partialpath = "";

					if (parameter != null && parameter.ToString() != null) { partialpath = parameter.ToString(); }

					string imagepath = Path.Combine(debugpath, partialpath, item.shortname.ToString());

					if (!imagepath.EndsWith(".png") && !imagepath.EndsWith(".jpg") && !imagepath.EndsWith(".jpeg"))
					{
						imagepath = imagepath + ".png";
					}

					if (File.Exists(imagepath))
					{
						return new BitmapImage(new Uri(imagepath, UriKind.RelativeOrAbsolute));
					}
					else
					{
						return new BitmapImage(new Uri("/RustRBLootEditor;component/Assets/unavailable.png", UriKind.Relative));
					}
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
