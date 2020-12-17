using System;
using System.Globalization;
using System.Windows.Data;

namespace PerekatEditor.Converters
{
    class AreaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string areaType = value as string;

            if (areaType == "Water")
                return "LightBlue";

            // Любая неизвестная зона будет (пока) отображаться пурпурным цветом.
            return "Magenta";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
