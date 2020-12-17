using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PerekatEditor.Converters
{
    class SpawnSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue))
                return 0.0;

            bool isBig = System.Convert.ToBoolean(values[0]);
            double blockSize = System.Convert.ToDouble(values[1]);

            return blockSize * (isBig ? 1.35 : 1.0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
