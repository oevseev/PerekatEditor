using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using PerekatEditor.Controls;

namespace PerekatEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private App application;
        private MainDataContext mainDataContext;

        public MainWindow()
        {
            InitializeComponent();
            application = Application.Current as App;

            // Написанный ниже код связывает создаваемый контекст окна с глобальным состоянием уровня.
            // При желании этот код можно переписать с поддержкой нескольких открытых уровней.
            //
            // Немного не уверен насчет правильности использования событийной концепции и то, как будет вести себя
            // сборщик мусора, но при использовании SDI-подхода все работает как часы.
            mainDataContext = new MainDataContext(application.CurrentLevelData);
            application.LevelDataUpdate += mainDataContext.UpdateLevelData;
            DataContext = mainDataContext;
        }

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var newLevelDialog = new NewLevelDialog();

            if (newLevelDialog.ShowDialog() == true)
            {
                application.NewLevel(newLevelDialog.LevelWidth, newLevelDialog.LevelHeight);
            }

            UpdateTitle();
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            application.OpenLevel();
            UpdateTitle();
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (application.CurrentFileName != null)
                application.SaveLevel();
            else application.SaveLevelAs();
        }
 
        private void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            application.SaveLevelAs();
            UpdateTitle();
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            application.CloseLevel();
            UpdateTitle();
        }

        private void ExitCommandBinding_Executed(object sender, RoutedEventArgs e)
        {
            Close(); // Серьезно?
        }

        private void FileCommands_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mainDataContext.IsFileOpened;
        }

        private void UpdateTitle()
        {
            if (application.CurrentLevelData != null)
            {
                string fileName = "<untitled>";
                if (application.CurrentFileName != null)
                    fileName = Path.GetFileNameWithoutExtension(application.CurrentFileName);
                Title = string.Format("{0} \u2014 Perekat Editor Alpha", fileName);
            }
            else Title = "Perekat Editor Alpha";
        }

        private void LevelPropertiesCommandBinding_Executed(object sender, RoutedEventArgs e)
        {
            var levelPropertiesDialog = new LevelPropertiesDialog();
            levelPropertiesDialog.UpdateData(mainDataContext);

            if (levelPropertiesDialog.ShowDialog() == true)
            {
                mainDataContext.ResizeLevel(levelPropertiesDialog.LevelWidth, levelPropertiesDialog.LevelHeight);
                mainDataContext.SpawnPosition = new Point(levelPropertiesDialog.SpawnX, levelPropertiesDialog.SpawnY);
                mainDataContext.SpawnBig = levelPropertiesDialog.SpawnBig;
            }
        }
    }

    public static class MainWindowCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Exit",
            "Exit",
            typeof(MainWindowCommands),
            new InputGestureCollection() {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            });

        public static readonly RoutedUICommand LevelProperties = new RoutedUICommand(
            "LevelProperties",
            "Level Properties",
            typeof(MainWindowCommands),
            null);
    }
}
