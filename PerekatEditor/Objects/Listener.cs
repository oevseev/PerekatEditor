using System;
using System.Collections.Generic;
using System.Text;

namespace PerekatEditor.Objects
{
    class Listener : LevelObject
    {
        protected Listener() {}

        /// <summary>
        /// Направление слушателя
        /// </summary>
        public string Direction { get; set; }
        
        /// <summary>
        /// Создает объект-слушатель, имеющий заданные тип, координаты и направление
        /// </summary>
        /// <param name="type">Тип слушателя</param>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="direction">Направление слушателя</param>
        public Listener(string type, double x, double y, string direction)
            : base(type, x, y)
        {
            if (type.StartsWith("Ring"))
            {
                if (direction == "left") direction = "right";
                if (direction == "bottom") direction = "top";
            }
            else if (type == "Checkpoint" || type == "Leavesite" || type == "Life" || type == "Speed")
            {
                direction = null;
            }

            Direction = direction;
        }

        /// <summary>
        /// Создает слушателя, заданного десериализованным объектом
        /// </summary>
        /// <param name="rawListener">Десериализованный объект</param>
        /// <returns>Объект слушателя</returns>
        public static Listener FromRaw(RawLevelData.RawListener rawListener)
        {
            StringBuilder listenerType = new StringBuilder();
            if (!string.IsNullOrEmpty(rawListener.type))
            {
                // Капитализация первой буквы (но что будет, если появятся дефисы?)
                string prettyType = char.ToUpper(rawListener.type[0]) + rawListener.type.Substring(1);
                listenerType.Append(prettyType);
            }
            // Энтони, ну чтоб тебя с твоим форматом
            if (rawListener.size == "big")
                listenerType.Append("Big");

            return new Listener(listenerType.ToString(), rawListener.position.x, rawListener.position.y,
                rawListener.direction);
        }

        /// <summary>
        /// Создает сериализуемый объект на основе объекта слушателя
        /// </summary>
        /// <param name="listener">Объект слушателя</param>
        /// <returns>Сериализуемый объект</returns>
        public static RawLevelData.RawListener ToRaw(Listener listener)
        {
            string objectType = listener.ObjectType;
            string size = null;

            if (objectType.EndsWith("Big"))
            {
                size = "big";
                objectType = objectType.Replace("Big", "");
            }
            else if (objectType == "Ring" || objectType == "Spike")
            {
                size = "small";
            }

            return new RawLevelData.RawListener
            {
                type = objectType.ToLowerInvariant(),
                position = new RawLevelData.Coords { x = listener.X, y = listener.Y },
                direction = listener.Direction,
                size = size
            };
        }

        public override IList<EditablePropertyEntry> GetEditableProperties()
        {
            var list = new List<EditablePropertyEntry>
            {
                new EditablePropertyEntry { Name = "X", Type = EditablePropertyType.Numeric },
                new EditablePropertyEntry { Name = "Y", Type = EditablePropertyType.Numeric },
            };

            if (Direction != null)
            {
                list.Add(new EditablePropertyEntry { Name = "Direction", Type = EditablePropertyType.Selectable });
            }

            return list;
        }
    }
}
