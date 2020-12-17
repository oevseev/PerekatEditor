using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace PerekatEditor
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal LevelData CurrentLevelData { get; private set; }
        internal event EventHandler<LevelDataEventArgs> LevelDataUpdate;
        internal string CurrentFileName { get; private set; }

        public App()
        {
            Startup += App_Startup;

            AppDomain.CurrentDomain.UnhandledException += PrintTraceback;
            RegisterUnhandledExceptionHandler();
        }

        private void PrintTraceback(object sender, UnhandledExceptionEventArgs e)
        {
            File.WriteAllText("traceback.log", e.ExceptionObject.ToString());
        }

        internal void NewLevel(int width, int height)
        {
            CurrentFileName = null;
            CurrentLevelData = new LevelData(width, height);
            OnLevelDataChange();
        }

        internal void OpenLevel()
        {
            // TODO: Перенести строку фильтра в ресурсы
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Level data (*.json)|*.json";
 
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentFileName = openFileDialog.FileName;

                var newLevelData = new LevelData(File.ReadAllText(CurrentFileName));
                if (newLevelData.Ready)
                {
                    CurrentLevelData = newLevelData;
                    OnLevelDataChange();
                }
                else
                {
                    MessageBox.Show("Failed to open level file.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        internal void SaveLevel()
        {
            File.WriteAllText(CurrentFileName, CurrentLevelData.Serialize());
        }

        internal void SaveLevelAs()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Level data (*.json)|*.json";

            if (saveFileDialog.ShowDialog() == true)
            {
                CurrentFileName = saveFileDialog.FileName;
                SaveLevel();
            }
        }

        internal void CloseLevel()
        {
            CurrentFileName = null;
            CurrentLevelData = null;
            OnLevelDataChange();
        }

        private void App_Startup(object sender, StartupEventArgs eventArgs)
        {
            // Почему-то не работает вне Visual Studio.
            // InitTheme();
        }

        private void InitTheme()
        {
            Version windows8Version = new Version(6, 2);
            Version windowsXpVersion = new Version(5, 1);

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (Environment.OSVersion.Version >= windows8Version)
                    SetTheme("Aero2");
                else if (Environment.OSVersion.Version >= windowsXpVersion)
                    SetTheme("Aero");
                else
                    SetTheme("Classic");
            }
        }

        private void SetTheme(string themeName)
        {
            StringBuilder xamlName = new StringBuilder(themeName);
            if (themeName != "Classic")
                xamlName.Append(".NormalColor");

            string themeUriString = string.Format("/PresentationFramework.{0};component/themes/{1}.xaml",
                themeName, xamlName.ToString());
            Uri themeUri = new Uri(themeUriString, uriKind: UriKind.Relative);

            ResourceDictionary resourceDictionary = new ResourceDictionary() { Source = themeUri };
            Resources.MergedDictionaries.Add(resourceDictionary);
        }

        private void OnLevelDataUpdate(LevelDataEventArgs e)
        {
            if (LevelDataUpdate != null)
                LevelDataUpdate(this, e);
        }

        private void OnLevelDataChange()
        {
            // Это работает, и, думаю, лучше это трогать не надо. Потому что это работает.
            OnLevelDataUpdate(new LevelDataEventArgs(CurrentLevelData) {
                NeedsReload = true,
                NeedsResize = true,
                ObjectsToUpdate = { "Blocks", "Areas", "Listeners", "Entities" }
            });
        }

        [Conditional("DEBUG")]
        private void RegisterUnhandledExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += DisplayExceptionMessage;
        }

        private void DisplayExceptionMessage(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), "An exception has occured");
        }
    }
}
