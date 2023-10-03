using RustRBLootEditor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RustRBLootEditor.Converters
{
    public class ProbabilityCheckConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<LootItem> lootItems = value as ObservableCollection<LootItem>;
            if (lootItems != null)
            {
                string result = $"{lootItems.Where(s => s.amount > 0 && s.probability == 1).Count()} items guaranteed to spawn.";

                var mightspawn = lootItems.Where(s => s.amount > 0 && s.probability > 0 && s.probability < 1);

                if (mightspawn.Any())
                {
                    result += $" {mightspawn.Count()} items might spawn based on chance.";
                }

                return result;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
