using System;
using System.Globalization;
using System.Windows.Data;

namespace PerekatEditor.Converters
{
    class DirectionConverter : IValueConverter
    {
        public const double ANGLE_TOP = 0.0;
        public const double ANGLE_RIGHT = 90.0;
        public const double ANGLE_BOTTOM = 180.0;
        public const double ANGLE_LEFT = 270.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string direction = value as string;
            switch (direction)
            {
                case "top": return ANGLE_TOP;
                case "right": return ANGLE_RIGHT;
                case "bottom": return ANGLE_BOTTOM;
                case "left": return ANGLE_LEFT;
                default: return ANGLE_TOP;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
