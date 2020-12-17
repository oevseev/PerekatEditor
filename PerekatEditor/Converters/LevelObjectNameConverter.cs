using System;
using System.Globalization;
using System.Windows.Data;
using PerekatEditor.Objects;

namespace PerekatEditor.Converters
{
    class LevelObjectNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Area)
            {
                Area area = value as Area;
                string description = string.Format("{0}: ({1}, {2})-({3}, {4})", area.ObjectType,
                    area.X, area.Y, area.X + area.Width, area.Y + area.Height);
                return description;
            }
            else if (value is LevelObject)
            {
                LevelObject levelObject = value as LevelObject;
                string description = string.Format("{0}: ({1}, {2})", levelObject.ObjectType, levelObject.X, levelObject.Y);
                return description;
            }

            return "Unknown object";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
