﻿using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace RustRBLootEditor.Converters
{
    /// <summary>
    /// Converts List Count to Control.Visibility values
    /// </summary>
    public class CaseConverter : IValueConverter
    {
        public CharacterCasing Case { get; set; }

        public CaseConverter()
        {
            Case = CharacterCasing.Upper;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var str = value.ToString();
                switch (Case)
                {
                    case CharacterCasing.Lower:
                        return str.ToLower();
                    case CharacterCasing.Normal:
                        return str;
                    case CharacterCasing.Upper:
                        return str.ToUpper();
                    default:
                        return str;
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
