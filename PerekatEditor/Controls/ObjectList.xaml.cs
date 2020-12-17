using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PerekatEditor.Controls
{
    /// <summary>
    /// Логика взаимодействия для ObjectList.xaml
    /// </summary>
    public partial class ObjectList : UserControl
    {
        private MainDataContext mainDataContext;

        public ObjectList()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mainDataContext.SelectObjectWithIndex(ObjectListView.SelectedIndex);
        }

        private void ListView_GotFocus(object sender, RoutedEventArgs e)
        {
            mainDataContext.SelectObjectWithIndex(ObjectListView.SelectedIndex);
        }

        private void ListView_LostFocus(object sender, RoutedEventArgs e)
        {
            mainDataContext.SelectObjectWithIndex(-1);
        }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            // Чтобы облегчить удаление последующих объектов, сохраняем индекс текущего выделенного объекта.
            int selectedIndex = ObjectListView.SelectedIndex;

            if (e.Key == Key.Delete)
                RemoveSelectedObject();

            ObjectListView.SelectedIndex = selectedIndex;
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            mainDataContext = DataContext as MainDataContext;
        }

        private void Properties_Click(object sender, RoutedEventArgs e)
        {
            var propertiesWindow = new PropertiesWindow(mainDataContext.CurrentSelectionObject, mainDataContext);
            propertiesWindow.ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedObject();
        }

        private void RemoveSelectedObject()
        {
            mainDataContext.RemoveSelectedObject();
        }
    }
}
