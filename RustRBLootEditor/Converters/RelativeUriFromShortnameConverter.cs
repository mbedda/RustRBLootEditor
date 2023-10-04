//using RustRBLootEditor.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Data;
//using System.Windows.Media.Imaging;
//using System.Windows.Media;
//using System.Drawing;
//using System.Drawing.Imaging;

//namespace RustRBLootEditor.Converters
//{
//    public class RelativeUriFromShortnameConverter : IValueConverter
//    {
//        static readonly Dictionary<string, BitmapSource> images = new Dictionary<string, BitmapSource>();
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value != null)
//            {
//                if (!images.TryGetValue(value.ToString(), out var img) && File.Exists($"F:\\Games\\steamapps\\common\\Rust\\Bundles\\items\\{value}.png"))
//                {
//                    using FileStream fs = new FileStream($"F:\\Games\\steamapps\\common\\Rust\\Bundles\\items\\{value}.png", FileMode.Open);
//                    using Image source = new Bitmap(fs);
//                    using Image destination = new Bitmap(64, 64);

//                    using (var g = Graphics.FromImage(destination))
//                    {
//                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
//                        g.DrawImage(source, new Rectangle(0, 0, 64, 64), new Rectangle(0, 0, (int)source.Width, (int)source.Height), GraphicsUnit.Pixel);
//                    }
//                    var stream = new MemoryStream();
//                    destination.Save(stream, ImageFormat.Png);

//                    var image = new BitmapImage();
//                    image.BeginInit();
//                    image.StreamSource = stream;
//                    image.EndInit();

//                    var i = image.Clone();
//                    i.Freeze();

//                    images[value.ToString()] = img = i;
//                }

//                if (img != null) return img;
//                return DependencyProperty.UnsetValue;
//            }
//            else
//            {
//                return DependencyProperty.UnsetValue;
//            }
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
