using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PerekatEditor.Objects;
using PerekatEditor.Converters;
using System.Reflection;

namespace PerekatEditor.Controls
{
    /// <summary>
    /// Логика взаимодействия для PropertiesWindow.xaml
    /// </summary>
    public partial class PropertiesWindow : Window
    {
        private LevelObject levelObject;
        private List<PropertyListEntry> propertyList = new List<PropertyListEntry>();
        private MainDataContext mainDataContext = null;

        internal PropertiesWindow(LevelObject levelObject, MainDataContext mainDataContext)
        {
            InitializeComponent();
            this.levelObject = levelObject;
            this.mainDataContext = mainDataContext;
        }

        private void PropertiesPanel_Loaded(object sender, RoutedEventArgs e)
        {
            var typeConverter = new LevelObjectTypeConverter();

            ObjectIcon.Source = typeConverter.Convert(levelObject.ObjectType, typeof(BitmapImage),
                "IgnoreExceptions", CultureInfo.InvariantCulture) as BitmapImage;
            ObjectName.Content = levelObject.ObjectType;

            foreach (var editableProperty in levelObject.GetEditableProperties())
            {
                string value = levelObject.GetType().GetProperty(editableProperty.Name).GetValue(levelObject).ToString();
                propertyList.Add(new PropertyListEntry { Name = editableProperty.Name, Value = value });
            }

            PropertyList.ItemsSource = propertyList;
        }

        private void PropertyList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            PropertyListEntry currentItem = dataGrid.CurrentCell.Item as PropertyListEntry;
            string newValue = (e.EditingElement as TextBox).Text.ToString();

            if (currentItem == null)
                return;

            PropertyInfo property = levelObject.GetType().GetProperty(currentItem.Name);
            try
            {
                property.SetValue(levelObject, Convert.ChangeType(newValue, property.PropertyType));
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            mainDataContext.UpdateObjects();
        }
    }

    class PropertyListEntry
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
