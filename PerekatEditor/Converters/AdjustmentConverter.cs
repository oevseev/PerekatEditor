using System;
using System.Globalization;
using System.Windows.Data;
using PerekatEditor.Objects;
using System.Windows;
using System.Linq;

namespace PerekatEditor.Converters
{
    class AdjustmentConverter : IValueConverter, IMultiValueConverter
    {
        public const double CACTUS_ADJUSTMENT = -1.0;
        public const double RING_SPIKE_ADJUSTMENT = 0.25;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Г-споди, Энтони, ну что за хуйня.
            Listener listener = value as Listener;
            string coord = parameter as String;

            if (listener != null)
                return CalculateAdjustment(listener.ObjectType, listener.Direction, coord);

            return 0;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue))
                return null;

            string objectType = values[0] as string;
            string direction = values[1] as string;
            string coord = parameter as string;

            return CalculateAdjustment(objectType, direction, coord);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private double CalculateAdjustment(string objectType, string direction, string coord)
        {
            bool horizontal = direction == "left" || direction == "right";

            if (objectType == "SpikeBig")
            {
                if ((coord == "Y" && direction == "top") || (coord == "X" && direction == "left"))
                    return CACTUS_ADJUSTMENT;
            }
            else if (objectType == "Spike")
            {
                if (coord == "X" && !horizontal || coord == "Y" && horizontal)
                    return RING_SPIKE_ADJUSTMENT;
            }
            else if (objectType == "Ring")
            {
                if (coord == "X" && horizontal || coord == "Y" && !horizontal)
                    return RING_SPIKE_ADJUSTMENT;
            }

            return 0;
        }
    }
}
