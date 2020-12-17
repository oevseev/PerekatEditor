using System;
using System.Collections.Generic;

namespace PerekatEditor.Objects
{
    class LevelObject
    {
        protected LevelObject() {}

        /// <summary>
        /// Внутриигровой тип объекта
        /// </summary>
        public string ObjectType { get; private set; }

        /// <summary>
        /// Координата X объекта
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Координата Y объекта
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Создает объект с заданным внутриигровым типом.
        /// </summary>
        /// <param name="type">Тип объекта</param>
        /// <param name="x">Координата X объекта</param>
        /// <param name="y">Координата Y объекта</param>
        public LevelObject(string type, double x, double y)
        {
            ObjectType = type;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Возвращает список, содержащий сведения о всех свойствах объекта, доступных
        /// для редактирования
        /// </summary>
        /// <returns>Список данных свойств</returns>
        public virtual IList<EditablePropertyEntry> GetEditableProperties()
        {
            return new List<EditablePropertyEntry>
            {
                new EditablePropertyEntry { Name = "X", Type = EditablePropertyType.Numeric },
                new EditablePropertyEntry { Name = "Y", Type = EditablePropertyType.Numeric }
            };
        }
    }

    enum EditablePropertyType { Numeric, Selectable, Boolean }

    struct EditablePropertyEntry
    {
        public string Name;
        public EditablePropertyType Type;
    }
}
