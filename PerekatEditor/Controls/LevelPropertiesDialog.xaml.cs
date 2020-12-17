using System;
using System.Windows;
using PerekatEditor;

namespace PerekatEditor.Controls
{
    /// <summary>
    /// Логика взаимодействия для LevelPropertiesDialog.xaml
    /// </summary>
    public partial class LevelPropertiesDialog : Window
    {
        public int LevelWidth { get; private set; }
        public int LevelHeight { get; private set; }

        public double SpawnX { get; private set; }
        public double SpawnY { get; private set; }
        public bool SpawnBig { get; private set; }

        public LevelPropertiesDialog()
        {
            InitializeComponent();
        }

        internal void UpdateData(MainDataContext mdc)
        {
            LevelWidthBox.Text = mdc.LevelWidth.ToString();
            LevelHeightBox.Text = mdc.LevelHeight.ToString();

            Point spawnPosition = mdc.SpawnPosition;
            SpawnXBox.Text = spawnPosition.X.ToString();
            SpawnYBox.Text = spawnPosition.Y.ToString();

            SpawnBigCheckbox.IsChecked = mdc.SpawnBig;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LevelWidth = int.Parse(LevelWidthBox.Text);
                LevelHeight = int.Parse(LevelHeightBox.Text);

                SpawnX = double.Parse(SpawnXBox.Text);
                SpawnY = double.Parse(SpawnYBox.Text);
                SpawnBig = (SpawnBigCheckbox.IsChecked == true);

                if (LevelWidth <= 0 || LevelHeight <= 0)
                    throw new InvalidOperationException("Dimensions must be positive.");
                if (SpawnX < 0 || SpawnY < 0)
                    throw new InvalidOperationException("Spawn coordinates must be nonnegative.");
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex) when (ex is FormatException || ex is InvalidOperationException)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
