using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PerekatEditor.Converters
{
    class LevelObjectSizeConverter : IMultiValueConverter
    {
        public const double BLOCK_PIXEL_SIZE = 200.0;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue))
                return 0.0;

            string objectType = values[0] as string;
            string direction = values[1] as string;
            double blockSize = System.Convert.ToDouble(values[2]);

            // Небольшой костыль для Leavesite, который в силу своей конструкции самую малость больше,
            // чем пространство, которое он должен занимать (2x2 блока)
            if (objectType == "Leavesite")
                return 2 * blockSize;

            // Убейте меня.
            // Но пока скорость важнее качества. (Удивительно, но на производительности это не сказывается!)
            BitmapImage image = LevelObjectTypeConverter.GetObjectResource(objectType);

            if (image == null)
                return 0.0;

            bool horizontal = false;
            if (direction == "left" || direction == "right")
                horizontal = true;

            return (horizontal ? image.PixelHeight : image.PixelWidth) / BLOCK_PIXEL_SIZE * blockSize;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
