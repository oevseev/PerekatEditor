using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PerekatEditor.Objects;

namespace PerekatEditor.Controls
{
    /// <summary>
    /// Логика взаимодействия для LevelViewport.xaml
    /// </summary>
    public partial class LevelViewport : UserControl
    {
        private MainDataContext mainDataContext;
        private UIElement viewportGrid;

        bool waterMode = false;
        Point upperLeftCoords = new Point(0, 0);

        public LevelViewport()
        {
            InitializeComponent();
        }

        private void Viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {   
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                waterMode = true;

                Point mousePosition = e.GetPosition(viewportGrid);
                double x = Math.Round(2 * mousePosition.X / mainDataContext.BlockSize) / 2.0;
                double y = Math.Round(2 * mousePosition.Y / mainDataContext.BlockSize) / 2.0;
 
                upperLeftCoords = new Point(x, y);
            }
            else if (mainDataContext.CurrentEditingMode == EditingMode.Selection)
            {
                LevelObject levelObject = mainDataContext.GetObjectAtCurrentPosition();

                if (levelObject != null)
                {
                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        mainDataContext.RemoveObject(levelObject);
                    }
                    else if (e.ClickCount == 2)
                    {
                        var propertiesWindow = new PropertiesWindow(levelObject, mainDataContext);
                        propertiesWindow.ShowDialog();
                    }
                    else mainDataContext.SelectObjectAtCurrentPosition();
                }
            }
            else if (mainDataContext.CurrentEditingMode == EditingMode.BlockPlacement)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    mainDataContext.PlaceBlockAtCurrentPosition();
                else if (e.RightButton == MouseButtonState.Pressed)
                    mainDataContext.RemoveBlockAtCurrentPosition();
                
            }
            else if (mainDataContext.CurrentEditingMode == EditingMode.ListenerPlacement)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    mainDataContext.PlaceListenerAtCurrentPosition();
                else if (e.RightButton == MouseButtonState.Pressed)
                    mainDataContext.NextDirection();
            }
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(viewportGrid);
            double x = 0, y = 0;

            if (mainDataContext.CurrentEditingMode == EditingMode.BlockPlacement)
            {
                x = (int)(mousePosition.X / mainDataContext.BlockSize);
                y = (int)(mousePosition.Y / mainDataContext.BlockSize);
            }
            else
            {
                x = (int)(2 * mousePosition.X / mainDataContext.BlockSize) / 2.0;
                y = (int)(2 * mousePosition.Y / mainDataContext.BlockSize) / 2.0;
            }

            Point position = new Point(x, y);

            if (position != mainDataContext.CurrentPosition)
                mainDataContext.CurrentPosition = position;
        }

        private void Viewport_Loaded(object sender, RoutedEventArgs e)
        {
            // Б-же, ну какой же это все-таки костыль, а.
            viewportGrid = sender as UIElement;

            // А вот контекст реально раньше работал, если присвоить значение полю в конструкторе, но сейчас при
            // попытке изменить модель представления все крэшится. BLUH.
            mainDataContext = DataContext as MainDataContext;
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (waterMode)
            {
                waterMode = false;

                Point mousePosition = e.GetPosition(viewportGrid);
                double x = Math.Round(2 * mousePosition.X / mainDataContext.BlockSize) / 2.0;
                double y = Math.Round(2 * mousePosition.Y / mainDataContext.BlockSize) / 2.0;

                mainDataContext.CreateArea(upperLeftCoords.X, upperLeftCoords.Y, x, y);
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (waterMode) waterMode = false;
        }
    }
}
