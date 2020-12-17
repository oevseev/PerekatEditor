using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace PerekatEditor.Converters
{
    class CoordConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue))
                return 0.0;

            // Мне очень стыдно писать этот код и очень больно его читать.
            // Но, так как UserControl.Resources почему-то не имеет доступа к DataContext элемента управления,
            // приходится делать финт ушами и делать двойной байндинг.
            double position = System.Convert.ToDouble(values[0]);
            double blockSize = System.Convert.ToDouble(values[1]);

            // Если передан дополнительный аргумент (поправка к позиции), то смещаем имеющуюся
            // координату на данную поправку.
            double adjustment = 0;
            if (values.Length > 2)
                adjustment = System.Convert.ToDouble(values[2]);

            return (position + adjustment) * blockSize;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
