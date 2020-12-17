using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using PerekatEditor.Converters;
using PerekatEditor.Objects;

namespace PerekatEditor
{
    sealed class MainDataContext : INotifyPropertyChanged
    {
        public const int DEFAULT_BLOCK_SIZE = 32;

        public const int DEFAULT_BLOCK_ID = 1;
        public const string DEFAULT_LISTENER_TYPE = "Ring";
        public const string DEFAULT_LISTENER_DIRECTION = "top";

        private int blockSize = DEFAULT_BLOCK_SIZE;

        private int selectedBlockId = DEFAULT_BLOCK_ID;
        private string selectedListenerType = DEFAULT_LISTENER_TYPE;
        private string selectedListenerDirection = DEFAULT_LISTENER_DIRECTION;

        private Listener currentSelectionObject = null;

        private Point currentPosition = new Point(0, 0);
        private EditingMode currentEditingMode = EditingMode.BlockPlacement;

        /// <summary>
        /// Ссылка на данные текущего редактируемого уровня
        /// </summary>
        private LevelData levelData { get; set; }

        /// <summary>
        /// Открыт ли файл
        /// </summary>
        public bool IsFileOpened
        {
            get
            {
                return levelData != null;
            }
        }

        /// <summary>
        /// Текущий размер блока в пикселях
        /// </summary>
        public int BlockSize
        {
            get { return blockSize; }
            set
            {
                blockSize = value;
                NotifyPropertyChanged("BlockSize");
                NotifyPropertyChanged("GridWidth");
                NotifyPropertyChanged("GridHeight");
            }
        }
        
        /// <summary>
        /// Текущий выбранный блок
        /// </summary>
        public int SelectedBlockId
        {
            get { return selectedBlockId; }
            set
            {
                selectedBlockId = value;
                NotifyPropertyChanged("SelectedBlockId");
            }
        }

        /// <summary>
        /// Текущий выбранный тип слушателя
        /// </summary>
        public string SelectedListenerType
        {
            get { return selectedListenerType; }
            set
            {
                selectedListenerType = value;
                NotifyPropertyChanged("SelectedListenerType");
                NotifyPropertyChanged("CurrentListenerObject");
            }
        }

        /// <summary>
        /// Текущее выбранное направление слушателя
        /// </summary>
        public string SelectedListenerDirection
        {
            get { return selectedListenerDirection; }
            set
            {
                selectedListenerDirection = value;
                NotifyPropertyChanged("SelectedListenerDirection");
                NotifyPropertyChanged("CurrentListenerObject");
            }
        }

        public Listener CurrentListenerObject
        {
            get
            {
                if (LevelObjectTypeConverter.GetAllEntities().Contains(SelectedListenerType))
                    return new Entity(SelectedListenerType, CurrentPosition.X, CurrentPosition.Y,
                        SelectedListenerDirection);

                // Forgive me, IT gods, for I have sinned.
                // EDIT: А не, все нормально.
                return new Listener(SelectedListenerType, CurrentPosition.X, CurrentPosition.Y,
                    SelectedListenerDirection);
            }
        }

        public Listener CurrentSelectionObject
        {
            get { return currentSelectionObject; }
            set
            {
                currentSelectionObject = value;
                NotifyPropertyChanged("CurrentSelectionObject");
                NotifyPropertyChanged("SelectionVisibility");
            }
        }

        /// <summary>
        /// Текущий режим редактирования
        /// </summary>
        public EditingMode CurrentEditingMode
        {
            get { return currentEditingMode; }
            set
            {
                currentEditingMode = value;
                NotifyPropertyChanged("SelectionVisibility");
                NotifyPropertyChanged("BlockPlacementVisibility");
                NotifyPropertyChanged("ListenerPlacementVisibility");
            }
        }

        public Visibility SelectionVisibility
        {
            get
            {
                if (levelData != null && (currentEditingMode == EditingMode.Selection || CurrentSelectionObject != null))
                    return Visibility.Visible;
                else return Visibility.Hidden;
            }
        }

        /// <summary>
        /// Видимость слоя установки блока
        /// </summary>
        public Visibility BlockPlacementVisibility
        {
            get
            {
                if (levelData != null && currentEditingMode == EditingMode.BlockPlacement)
                    return Visibility.Visible;
                else return Visibility.Hidden;
            }
        }

        /// <summary>
        /// Видимость слоя установки объекта
        /// </summary>
        public Visibility ListenerPlacementVisibility
        {
            get
            {
                if (levelData != null && currentEditingMode == EditingMode.ListenerPlacement)
                    return Visibility.Visible;
                else return Visibility.Hidden;
            }
        }

        /// <summary>
        /// Видимость точки спауна
        /// </summary>
        public Visibility SpawnPositionVisibility
        {
            get
            {
                if (levelData != null)
                    return Visibility.Visible;
                else return Visibility.Hidden;
            }
        }

        /// <summary>
        /// Текущая позиция курсора
        /// </summary>
        public Point CurrentPosition
        {
            get { return currentPosition; }
            set
            {
                currentPosition = value;
                NotifyPropertyChanged("CurrentPosition");
            }
        }

        /// <summary>
        /// Ширина уровня
        /// </summary>
        public int LevelWidth
        {
            get
            {
                if (levelData != null)
                    return levelData.Width;
                else return 0;
            }
            set
            {
                // I'm going to hell for it.
                levelData.Resize(value, LevelHeight);
            }
        }

        /// <summary>
        /// Высота уровня
        /// </summary>
        public int LevelHeight
        {
            get
            {
                if (levelData != null)
                    return levelData.Height;
                else return 0;
            }
            set {
                // Или нет?
                levelData.Resize(LevelWidth, value);
            }
        }

        /// <summary>
        /// Координаты точки спауна
        /// </summary>
        public Point SpawnPosition
        {
            get
            {
                if (levelData != null)
                    return levelData.SpawnPosition;
                else return new Point(0, 0);
            }
            set
            {
                levelData.SpawnPosition = value;
                NotifyPropertyChanged("SpawnPosition");
            }
        }

        public bool SpawnBig
        {
            get
            {
                if (levelData != null)
                    return levelData.SpawnBig;
                else return false;
            }
            set
            {
                levelData.SpawnBig = value;
                NotifyPropertyChanged("SpawnBig");
            }
        }

        /// <summary>
        /// Суммарная ширина сетки в пикселях
        /// </summary>
        public int GridWidth
        {
            get { return BlockSize * LevelWidth; }
        }

        /// <summary>
        /// Суммарная высота сетки в пикселях
        /// </summary>
        public int GridHeight
        {
            get { return BlockSize * LevelHeight; }
        }

        /// <summary>
        /// Список блоков
        /// </summary>
        public ObservableCollection<Block> Blocks
        {
            get
            {
                if (levelData != null)
                    return levelData.Blocks;
                else return null;
            }
        }

        /// <summary>
        /// Список зон
        /// </summary>
        public IEnumerable<Area> Areas
        {
            get
            {
                if (levelData != null)
                    return levelData.Areas.ToList();
                else return null;
            }
        }

        /// <summary>
        /// Список объектов-слушателей
        /// </summary>
        public IEnumerable<Listener> Listeners
        {
            get
            {
                if (levelData != null)
                    return levelData.Listeners.ToList();
                else return null;
            }
        }

        public IEnumerable<Entity> Entities
        {
            get
            {
                if (levelData != null)
                    return levelData.Entities.ToList();
                else return null;
            }
        }

        public IEnumerable<Listener> LevelObjects
        {
            get
            {
                if (levelData != null)
                {
                    return new List<Listener>()
                        .Concat(levelData.Listeners)
                        .Concat(levelData.Entities);
                }
                else return null;
            }
        }

        private MainDataContext() { }

        public MainDataContext(LevelData levelData)
        {
            this.levelData = levelData;
        }

        /// <summary>
        /// Устанавливает блок на заданной позиции.
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="blockId">ID блока</param>
        public void PlaceBlockAt(int x, int y, int blockId)
        {
            levelData.PlaceBlockAt(x, y, blockId);
        }

        /// <summary>
        /// Устанавливает слушателя на текущей позиции.
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="objectType">Тип слушателя</param>
        /// <param name="direction">Направление</param>
        public void PlaceListenerAt(double x, double y, string objectType, string objectDirection)
        {
            levelData.PlaceListenerAt(x, y, objectType, objectDirection);
            UpdateObjects();
        }

        /// <summary>
        /// Устанавливает текущий выбранный блок на текущей позиции.
        /// </summary>
        public void PlaceBlockAtCurrentPosition()
        {
            PlaceBlockAt((int)CurrentPosition.X, (int)CurrentPosition.Y, SelectedBlockId);
        }

        /// <summary>
        /// Удаляет блок на текущей позиции.
        /// </summary>
        public void RemoveBlockAtCurrentPosition()
        {
            PlaceBlockAt((int)CurrentPosition.X, (int)CurrentPosition.Y, 0);
        }

        /// <summary>
        /// Устанавливает текущий выбранный объект на текущей позиции.
        /// </summary>
        public void PlaceListenerAtCurrentPosition()
        {
            levelData.PlaceListener(CurrentListenerObject);
            UpdateObjects();
        }

        /// <summary>
        /// Создает зону.
        /// </summary>
        /// <param name="x1">Координата X левого верхнего угла</param>
        /// <param name="y1">Координата Y левого верхнего угла</param>
        /// <param name="x2">Координата X правого нижнего угла</param>
        /// <param name="y2">Координата Y правого нижнего угла</param>
        public void CreateArea(double x1, double y1, double x2, double y2)
        {
            levelData.CreateArea(x1, y1, x2, y2);
            UpdateObjects();
        }

        /// <summary>
        /// Переключает направление слушателя.
        /// </summary>
        public void NextDirection()
        {
            if (SelectedListenerDirection == "top")
                SelectedListenerDirection = "left";
            else if (SelectedListenerDirection == "left")
                SelectedListenerDirection = "bottom";
            else if (SelectedListenerDirection == "bottom")
                SelectedListenerDirection = "right";
            else if (SelectedListenerDirection == "right")
                SelectedListenerDirection = "top";
        }

        /// <summary>
        /// Изменяет размер уровня до заданных ширины и высоты.
        /// </summary>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        public void ResizeLevel(int width, int height)
        {
            levelData.Resize(width, height);
        }

        public void SelectObjectAtCurrentPosition()
        {
            LevelObject levelObject = GetObjectAtCurrentPosition(false);
            if (levelObject is Listener)
                CurrentSelectionObject = levelObject as Listener;
        }

        public LevelObject GetObjectAtCurrentPosition(bool searchAreas = true)
        {
            foreach (Listener listener in LevelObjects)
            {
                BitmapImage resource = LevelObjectTypeConverter.GetObjectResource(listener.ObjectType);
                bool isHorizontal = listener.Direction == "left" || listener.Direction == "right";

                double objectWidth = (isHorizontal ? resource.PixelHeight : resource.PixelWidth) /
                    LevelObjectSizeConverter.BLOCK_PIXEL_SIZE;
                double objectHeight = (isHorizontal ? resource.PixelWidth : resource.PixelHeight) /
                    LevelObjectSizeConverter.BLOCK_PIXEL_SIZE;

                if (currentPosition.X >= listener.X && currentPosition.X <= listener.X + objectWidth &&
                    currentPosition.Y >= listener.Y && currentPosition.Y <= listener.Y + objectHeight)
                {
                    return listener;
                }
            }

            if (searchAreas)
            {
                foreach (Area area in levelData.Areas)
                {
                    if (currentPosition.X >= area.X && currentPosition.X <= area.X + area.Width &&
                        currentPosition.Y >= area.Y && currentPosition.Y <= area.Y + area.Height)
                    {
                        return area;
                    }
                }
            }

            return null;
        }

        public void SelectObjectWithIndex(int index)
        {
            Listener currentObject = LevelObjects?.ElementAtOrDefault(index);
            CurrentSelectionObject = currentObject;
        }

        public void RemoveObject(LevelObject levelObject)
        {
            if (levelObject == CurrentSelectionObject) CurrentSelectionObject = null;
            levelData.RemoveObject(levelObject);
            UpdateObjects();
        }

        public void RemoveSelectedObject()
        {
            levelData.RemoveObject(CurrentSelectionObject);
            CurrentSelectionObject = null;
            UpdateObjects();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public void UpdateLevelData(object sender, LevelDataEventArgs e)
        {
            // Собственно говоря, этот код лучше не трогать, потому что он пока что идеально работает.
            // Здесь (достаточно криво) реализуется часть паттерна "наблюдатель", который, в общем-то, здесь почти и
            // не нужен (ObservableCollection все делает за нас), но более простого способа обновить байндинги я
            // так и не нашел.

            if (e.NeedsReload)
            {
                levelData = e.Data;
                if (levelData != null)
                    levelData.LevelResize += HandleLevelResize;
                CurrentSelectionObject = null;

                NotifyPropertyChanged("IsFileOpened");
                NotifyPropertyChanged("SpawnPosition");
                NotifyPropertyChanged("SpawnPositionVisibility");
                NotifyPropertyChanged("SpawnBig");
            }

            if (e.NeedsResize)
            {
                NotifyPropertyChanged("LevelWidth");
                NotifyPropertyChanged("LevelHeight");
                NotifyPropertyChanged("GridWidth");
                NotifyPropertyChanged("GridHeight");
            }

            // Возможность обновлять слои по-отдельности, вероятно, пригодится в дальнейшем.
            if (e.ObjectsToUpdate != null)
            {
                foreach (string objectToUpdate in e.ObjectsToUpdate)
                    NotifyPropertyChanged(objectToUpdate);
            }

            NotifyPropertyChanged("LevelObjects");
            NotifyPropertyChanged("CurrentSelectionObject");
        }

        public void UpdateObjects()
        {
            UpdateLevelData(this, new LevelDataEventArgs(levelData)
            {
                NeedsReload = false,
                NeedsResize = false,
                ObjectsToUpdate = { "Areas", "Listeners", "Entities" }
            });
        }

        private void HandleLevelResize(object sender, EventArgs e)
        {
            UpdateLevelData(sender, new LevelDataEventArgs(sender as LevelData) {
                NeedsReload = false,
                NeedsResize = true,
                ObjectsToUpdate = { "Blocks", "Areas", "Listeners", "Entities" }
            });
        }
    }

    enum EditingMode { Selection, BlockPlacement, ListenerPlacement }
    // enum BlockPlacementMode { Freehand, Fill }
}
