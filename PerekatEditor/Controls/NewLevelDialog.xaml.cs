using System;
using System.Windows;

namespace PerekatEditor.Controls
{
    /// <summary>
    /// Логика взаимодействия для NewLevelDialog.xaml
    /// </summary>
    public partial class NewLevelDialog : Window
    {
        public int LevelWidth { get; private set; }
        public int LevelHeight { get; private set; }

        public NewLevelDialog()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LevelWidth = int.Parse(LevelWidthBox.Text);
                LevelHeight = int.Parse(LevelHeightBox.Text);

                if (LevelWidth <= 0 || LevelHeight <= 0)
                    throw new InvalidOperationException("Dimensions must be positive.");
                
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
