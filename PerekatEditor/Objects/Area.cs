using System;
using System.Collections.Generic;

namespace PerekatEditor.Objects
{
    class Area : LevelObject
    {
        protected Area() {}

        /// <summary>
        /// Ширина зоны
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Высота зоны
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Герметичность зоны
        /// </summary>
        public bool IsHermetic { get; set; }

        /// <summary>
        /// Создает зону с заданными параметрами.
        /// </summary>
        /// <param name="type">Тип зоны</param>
        /// <param name="x">Координата X левого верхнего угла</param>
        /// <param name="y">Координата Y левого верхнего угла</param>
        /// <param name="width">Ширина зоны</param>
        /// <param name="height">Высота зоны</param>
        /// <param name="isHermetic">Герметичность зоны</param>
        public Area(string type, double x, double y, double width, double height, bool isHermetic)
            : base(type, x, y)
        {
            Width = width;
            Height = height;
            IsHermetic = isHermetic;
        }

        /// <summary>
        /// Создает зону, заданную десериализованным объектом
        /// </summary>
        /// <param name="rawArea">Десериализованный объект</param>
        /// <returns>Объект зоны</returns>
        public static Area FromRaw(RawLevelData.RawArea rawArea)
        {
            string areaType = rawArea.type;
            if (areaType == "water") areaType = "Water";

            return new Area(areaType, rawArea.position.x, rawArea.position.y,
                rawArea.width, rawArea.height, rawArea.hermetic);
        }

        /// <summary>
        /// Создает сериализуемый объект на основе объекта зоны
        /// </summary>
        /// <param name="area">Объект зоны</param>
        /// <returns>Сериализуемый объект</returns>
        public static RawLevelData.RawArea ToRaw(Area area)
        {
            return new RawLevelData.RawArea
            {
                type = area.ObjectType.ToLowerInvariant(),
                position = new RawLevelData.Coords { x = area.X, y = area.Y },
                width = area.Width,
                height = area.Height,
                hermetic = area.IsHermetic
            };
        }

        public override IList<EditablePropertyEntry> GetEditableProperties()
        {
            return new List<EditablePropertyEntry>
            {
                new EditablePropertyEntry { Name = "X", Type = EditablePropertyType.Numeric },
                new EditablePropertyEntry { Name = "Y", Type = EditablePropertyType.Numeric },
                new EditablePropertyEntry { Name = "Width", Type = EditablePropertyType.Numeric },
                new EditablePropertyEntry { Name = "Height", Type = EditablePropertyType.Numeric },
                new EditablePropertyEntry { Name = "IsHermetic", Type = EditablePropertyType.Boolean }
            };
        }
    }
}
