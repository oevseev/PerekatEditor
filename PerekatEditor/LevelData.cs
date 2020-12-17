using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using Newtonsoft.Json;
using PerekatEditor.Objects;

namespace PerekatEditor
{
    sealed class LevelData
    {
        /// <summary>
        /// Флаг успешной инициализации данных
        /// </summary>
        public bool Ready { get; private set; }

        /// <summary>
        /// Ширина текущего уровня
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Высота текущего уровня
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Координаты точки спауна
        /// </summary>
        public Point SpawnPosition { get; set; }

        /// <summary>
        /// Спаунится ли колобок надутым
        /// </summary>
        public bool SpawnBig { get; set; }

        /// <summary>
        /// Список блоков уровня
        /// </summary>
        public ObservableCollection<Block> Blocks { get; private set; }

        /// <summary>
        /// Список зон уровня
        /// </summary>
        public ObservableCollection<Area> Areas { get; private set; }

        /// <summary>
        /// Список слушателей уровня
        /// </summary>
        public ObservableCollection<Listener> Listeners { get; private set; }

        /// <summary>
        /// Список энтити уровня
        /// </summary>
        public ObservableCollection<Entity> Entities { get; private set; }

        public LevelData()
        {
            Ready = false;

            Blocks = new ObservableCollection<Block>();
            Areas = new ObservableCollection<Area>();
            Listeners = new ObservableCollection<Listener>();
            Entities = new ObservableCollection<Entity>();
        }

        public LevelData(int width, int height) : this()
        {
            Width = width;
            Height = height;

            for (int i = 0; i < width * height; i++)
                Blocks.Add(Block.Construct(0, i % width));
        }

        /// <summary>
        /// Создает новый экземпляр LevelData
        /// </summary>
        /// <param name="jsonData">Данные уровня в формате JSON</param>
        public LevelData(string jsonData) : this()
        {
            RawLevelData rawLevelData = null;
            try
            {
                rawLevelData = JsonConvert.DeserializeObject<RawLevelData>(jsonData);
            }
            catch (JsonException) { return; }

            try
            {
                Width = rawLevelData.size.width;
                Height = rawLevelData.size.height;

                SpawnPosition = new Point(rawLevelData.spawnInfo.position.x, rawLevelData.spawnInfo.position.y);
                SpawnBig = (rawLevelData.spawnInfo.type == "big");

                Blocks = new ObservableCollection<Block>(ConstructBlockList(rawLevelData.blocks));
                if (rawLevelData.areas != null)
                    Areas = new ObservableCollection<Area>(rawLevelData.areas.Select(
                        rawArea => Area.FromRaw(rawArea)));
                if (rawLevelData.listeners != null)
                    Listeners = new ObservableCollection<Listener>(rawLevelData.listeners.Select(
                        rawListener => Listener.FromRaw(rawListener)));
                if (rawLevelData.entities != null)
                    Entities = new ObservableCollection<Entity>(rawLevelData.entities.Select(
                        rawEntity => Entity.FromRaw(rawEntity)));
            }
            catch (NullReferenceException) { return; }

            // На случай, если какой-то 3.14дор вздумает редактировать уровень руками, без этого шикарного редактора,
            // и выставит ширину, не соответствующую массиву блоков.
            // Или если Энтони набухается и проебет несколько блоков при конвертации.
            if (Blocks.Count == Width * Height)
                Ready = true;
        }

        public string Serialize()
        {
            var rawLevelData = new RawLevelData();

            rawLevelData.size = new RawLevelData.Size
            {
                width = Width,
                height = Height
            };

            rawLevelData.spawnInfo = new RawLevelData.RawSpawnInfo
            {
                position = new RawLevelData.Coords { x = SpawnPosition.X, y = SpawnPosition.Y },
                type = SpawnBig ? "big" : "small"
            };

            rawLevelData.areas = Areas.Select(area => Area.ToRaw(area)).ToList();
            rawLevelData.listeners = Listeners.Select(listener => Listener.ToRaw(listener)).ToList();
            rawLevelData.entities = Entities.Select(entity => Entity.ToRaw(entity)).ToList();
            rawLevelData.blocks = new List<IList<int>>();

            for (int i = 0; i < Height; i++)
            {
                var blockList = new List<int>();
                for (int j = 0; j < Width; j++)
                    blockList.Add(Blocks[i * Width + j].BlockId);
                rawLevelData.blocks.Add(blockList);
            }

            return JsonConvert.SerializeObject(rawLevelData, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        /// <summary>
        /// Устанавливает блок на заданной позиции.
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="blockId">ID блока</param>
        public void PlaceBlockAt(int x, int y, int blockId)
        {
            Blocks[y * Width + x] = Block.Construct(blockId, y);
        }

        /// <summary>
        /// Устанавливает слушателя на текущей позиции.
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="objectType">Тип слушателя</param>
        /// <param name="direction">Направление</param>
        public void PlaceListenerAt(double x, double y, string objectType, string direction)
        {
            PlaceListener(new Listener(objectType, x, y, direction));
        }

        public void PlaceListener(Listener listener)
        {
            if (listener is Entity)
                Entities.Add(listener as Entity);
            else
                Listeners.Add(listener);
        }

        public void RemoveObject(LevelObject levelObject)
        {
            if (levelObject is Area)
                Areas.Remove(levelObject as Area);
            else if (levelObject is Entity)
                Entities.Remove(levelObject as Entity);
            else if (levelObject is Listener)
                Listeners.Remove(levelObject as Listener);
        }

        public void CreateArea(double x1, double y1, double x2, double y2)
        {
            double ux = Math.Min(x1, x2), uy = Math.Min(y1, y2);
            double lx = Math.Max(x1, x2), ly = Math.Max(y1, y2);

            // Зоны не должны быть пустыми
            if (lx - ux == 0 || ly - uy == 0)
                return;

            Area area = new Area("Water", ux, uy, lx - ux, ly - uy, true);
            Areas.Add(area);
        }

        /// <summary>
        /// Изменяет размер уровня до заданных ширины и высоты.
        /// </summary>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        public void Resize(int width, int height)
        {
            List<Block> newBlocks = new List<Block>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Block block = null;
                    if (x < Width && y < Height)
                        block = Blocks[y * Width + x];
                    newBlocks.Add(block);
                }
            }

            Width = width;
            Height = height;

            Blocks = new ObservableCollection<Block>(newBlocks);

            // Возможно, стоит сделать более серьезные проверки, но мне люто хочется спать.
            // Сейчас выпиливаются объекты, левый верхний угол которых лежит за пределами уровня, но учитывая
            // различные поправки к координатам, этот способ далеко не идеален.
            if (Areas != null)
                Areas = new ObservableCollection<Area>(
                    Areas.ToList().FindAll(area => area.X + area.Width < width && area.Y + area.Height < height));
            if (Listeners != null)
                Listeners = new ObservableCollection<Listener>(
                    Listeners.ToList().FindAll(listener => listener.X < width && listener.Y < height));
            if (Entities != null)
                Entities = new ObservableCollection<Entity>(
                    Entities.ToList().FindAll(entity => entity.X < width && entity.Y < height));

            OnLevelResize();
        }

        private IList<Block> ConstructBlockList(IList<IList<int>> rawBlocks)
        {
            var blockList = new List<Block>();
            foreach (IList<int> row in rawBlocks)
            {
                var iterItems = row.Select((Value, Index) => new { Value, Index });
                foreach (var it in iterItems)
                {
                    Block tile = Block.Construct(it.Value, it.Index);
                    blockList.Add(tile);
                }
            }
            return blockList;
        }

        public EventHandler<EventArgs> LevelResize;
        private void OnLevelResize()
        {
            if (LevelResize != null)
                LevelResize(this, null);
        }
    }
}
