using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PerekatEditor.Converters
{
    class BlockConverter : IValueConverter
    {
        public const int MAX_ID = 32;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string blockName = null;
            if (value is int)
                blockName = "Even" + GetBlockName((int)value);
            else
                blockName = value as String;

            if (blockName != null)
            {
                try
                {
                    return Application.Current.FindResource("Block_" + blockName) as BitmapImage;
                }
                catch (ResourceReferenceKeyNotFoundException)
                {
                    // Возможно, стоит ввести какую-нибудь текстуру, которая будет подгружаться при попытке
                    // подгрузить несуществующий блок, но так ли это нужно?
                    string message = string.Format("Attempting to load bitmap for non-existent block \"{0}\"", blockName);
                    throw new InvalidOperationException(message);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получает название блока с заданным ID
        /// </summary>
        /// <param name="blockId">ID блока</param>
        /// <returns>Строка с названием блока</returns>
        public static string GetBlockName(int blockId)
        {
            switch (blockId)
            {
                // Обычные блоки
                case 1: return "BrickClearDefault";
                case 2: return "HalfBrickTopleft";
                case 3: return "HalfBrickTopright";
                case 4: return "HalfBrickBottomright";
                case 5: return "HalfBrickBottomleft";
                
                // Прыгучие блоки
                case 21: return "JumpBrickBoth";
                case 22: return "HalfBrickJumpTopleft";
                case 23: return "HalfBrickJumpTopright";
                case 24: return "HalfBrickJumpBottomright";
                case 25: return "HalfBrickJumpBottomleft";

                default: return null;
            }
        }
    }
}
