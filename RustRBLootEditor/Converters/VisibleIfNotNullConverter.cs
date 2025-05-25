using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RustRBLootEditor.Converters
{
    public class VisibleIfNotNullConverter : IValueConverter
    {
        //Set to true if you just want to hide the control
        //else set to false if you want to collapse the control
        private bool isHidden;
        public bool IsHidden
        {
            get { return isHidden; }
            set { isHidden = value; }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
             return value != null ? Visibility.Visible : IsHidden ? Visibility.Hidden : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
