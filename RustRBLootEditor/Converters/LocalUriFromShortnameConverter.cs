using RustRBLootEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RustRBLootEditor.Converters
{
    public class LocalUriFromShortnameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            else
            {
                if (Common.ResourceExists("Assets/RustItems/" + value.ToString() + ".png"))
                {
                    return new Uri("/RustRBLootEditor;component/Assets/RustItems/" + value.ToString() + ".png", UriKind.Relative);
                }
                else
                {
                    return new Uri("/RustRBLootEditor;component/Assets/unavailable.png", UriKind.Relative);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
