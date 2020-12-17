using System;
using System.Collections.Generic;

namespace PerekatEditor.Objects
{
    class Entity : Listener
    {
        protected Entity() {}

        /// <summary>
        /// Создает энтити с заданными параметрами
        /// </summary>
        /// <param name="objectType">Тип энтити</param>
        /// <param name="x">Координата X левого верхнего угла</param>
        /// <param name="y">Координата Y левого верхнего угла</param>
        /// <param name="direction">Начальное аправление движения</param>
        public Entity(string objectType, double x, double y, string direction) : base(objectType, x, y, direction)
        {
            X1 = X2 = x + 1.0;
            Y1 = Y2 = y + 1.0;
        }

        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }

        /// <summary>
        /// Создает энтити, заданный десериализованным объектом
        /// </summary>
        /// <param name="rawEntity">Десериализованный объект</param>
        /// <returns>Объект энтити</returns>
        public static Entity FromRaw(RawLevelData.RawEntity rawEntity)
        {
            string entityType = rawEntity.type;
            if (entityType == "spider") entityType = "Spider";

            Entity entity = new Entity(entityType, rawEntity.position.x, rawEntity.position.y, rawEntity.direction);
 
            // Поправка специфична только для пауков, но пока не планируется введение новых энтити
            entity.X1 = rawEntity.first.x + 1.0;
            entity.Y1 = rawEntity.first.y + 1.0;
            entity.X2 = rawEntity.second.x + 1.0;
            entity.Y2 = rawEntity.second.y + 1.0;

            return entity;
        }

        /// <summary>
        /// Создает сериализуемый объект на основе объекта энтити
        /// </summary>
        /// <param name="entity">Объект энтити</param>
        /// <returns>Сериализуемый объект</returns>
        public static RawLevelData.RawEntity ToRaw(Entity entity)
        {
            return new RawLevelData.RawEntity
            {
                type = entity.ObjectType.ToLowerInvariant(),
                position = new RawLevelData.Coords { x = entity.X, y = entity.Y },
                direction = entity.Direction,
                first = new RawLevelData.Coords { x = entity.X1 - 1.0, y = entity.Y1 - 1.0 },
                second = new RawLevelData.Coords { x = entity.X2 - 1.0, y = entity.Y2 - 1.0 }
            };
        }

        public override IList<EditablePropertyEntry> GetEditableProperties()
        {
            return new List<EditablePropertyEntry>
            {
                new EditablePropertyEntry { Name = "X", Type = EditablePropertyType.Numeric },
                new EditablePropertyEntry { Name = "Y", Type = EditablePropertyType.Numeric },

                // Listener-specific
                new EditablePropertyEntry { Name = "Direction", Type = EditablePropertyType.Selectable },
                
                // Entity-specific
                new EditablePropertyEntry { Name = "X1", Type = EditablePropertyType.Numeric },
                new EditablePropertyEntry { Name = "Y1", Type = EditablePropertyType.Numeric },
                new EditablePropertyEntry { Name = "X2", Type = EditablePropertyType.Numeric },
                new EditablePropertyEntry { Name = "Y2", Type = EditablePropertyType.Numeric }
            };
        }
    }
}
