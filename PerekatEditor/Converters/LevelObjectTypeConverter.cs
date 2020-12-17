using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PerekatEditor.Converters
{
    class LevelObjectTypeConverter : IValueConverter, IMultiValueConverter
    {
        public const string OBJ_RESOURCE_PREFIX = "Object_";
        public static readonly List<string> ENTITY_LIST = new List<string> { "Spider" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string objectType = value as String;

            BitmapImage objectResource = null;
            try
            {
                objectResource = GetObjectResource(objectType);
            }
            catch (InvalidOperationException e)
            {
                if (parameter as string != "IgnoreExceptions")
                    throw e;
            }

            return objectResource;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Contains(DependencyProperty.UnsetValue))
                return null;

            string objectType = values[0] as string;
            double angle = System.Convert.ToDouble(values[1]);

            BitmapImage objectResource = Convert(objectType, targetType, parameter, culture) as BitmapImage;
            if (objectResource == null)
                return null;

            // Энтити не вращаются, direction задает лишь направление их движения
            if (GetAllEntities().Contains(objectType))
                angle = 0.0;

            RotateTransform rotateTransform = new RotateTransform(angle, 0.5, 0.5);
            TransformedBitmap bitmap = new TransformedBitmap(objectResource, rotateTransform);

            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получает ресурс объекта заданного типа.
        /// </summary>
        /// <param name="objectType">Тип объекта</param>
        /// <returns></returns>
        public static BitmapImage GetObjectResource(string objectType)
        {
            if (objectType != null)
            {
                try
                {
                    return Application.Current.FindResource(OBJ_RESOURCE_PREFIX + objectType) as BitmapImage;
                }
                catch (ResourceReferenceKeyNotFoundException)
                {
                    string message = string.Format("Attempting to load bitmap for non-existent object \"{0}\"", objectType);
                    throw new InvalidOperationException(message);
                }
            }
            return null;
        }

        /// <summary>
        /// Возвращает список названий всех объектов, трактуемых как слушатели, без учета объектов,
        /// явно трактуемых как энтити
        /// </summary>
        /// <returns>Список слушателей</returns>
        public static IEnumerable<string> GetAllListeners()
        {
            var resourceDictionaries = Application.Current.Resources.MergedDictionaries;
            var resourceKeys = resourceDictionaries.SelectMany(x => x.Keys.Cast<string>()).OrderBy(x => x);

            var objectKeys = from x in resourceKeys
                             where x.StartsWith(OBJ_RESOURCE_PREFIX)
                             select x.Substring(OBJ_RESOURCE_PREFIX.Length);

            return objectKeys.Except(GetAllEntities());
        }

        /// <summary>
        /// Возвращает список названий всех объектов, явно трактуемых как энтити
        /// </summary>
        /// <returns>Список энтити</returns>
        public static IEnumerable<string> GetAllEntities()
        {
            return ENTITY_LIST;
        }
    }
}
