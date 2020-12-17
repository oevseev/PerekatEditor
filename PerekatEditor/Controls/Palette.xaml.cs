using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PerekatEditor.Converters;

namespace PerekatEditor.Controls
{
    /// <summary>
    /// Логика взаимодействия для Palette.xaml
    /// </summary>
    public partial class Palette : UserControl
    {
        private const int BUTTON_SIZE = 24;
        private const int BUTTON_MARGIN = 4;

        private MainDataContext mainDataContext;
        private Dictionary<int, ToggleButton> blockButtons = new Dictionary<int, ToggleButton>();
        private Dictionary<string, ToggleButton> listenerButtons = new Dictionary<string, ToggleButton>();

        public Palette()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this as DependencyObject))
                ConstructButtons();
        }

        private void ConstructSeparator()
        {
            Separator separator = new Separator();
            separator.Style = FindResource(ToolBar.SeparatorStyleKey) as Style;
            ButtonPanel.Children.Add(separator);
        }

        private void ConstructButtons()
        {
            for (int i = 0; i < BlockConverter.MAX_ID; i++)
            {
                string blockName = BlockConverter.GetBlockName(i);
                if (blockName != null)
                {
                    // Блин, как-то коряво выглядит. Можно, конечно, было передавать только ID, но вариант с
                    // двукратным получением имени блока мне кажется еще хуже.
                    ToggleButton button = ConstructBlockButton(i, blockName);
                    blockButtons[i] = button;
                    ButtonPanel.Children.Add(button);
                }
            }
            ConstructSeparator();
            foreach (string objectType in LevelObjectTypeConverter.GetAllListeners())
            {
                ToggleButton button = ConstructListenerButton(objectType);
                listenerButtons[objectType] = button;
                ButtonPanel.Children.Add(button);
            }
            ConstructSeparator();
            foreach (string objectType in LevelObjectTypeConverter.GetAllEntities())
            {
                ToggleButton button = ConstructListenerButton(objectType);
                listenerButtons[objectType] = button;
                ButtonPanel.Children.Add(button);
            }
        }

        private ToggleButton ConstructBlockButton(int buttonId, string blockName)
        {
            BlockConverter blockConverter = new BlockConverter();

            // Что касается "Even", то, очевидно, у нас нет никакой вертикали, к которой можно привязать
            // блок. Но спрайты у нас зависят от четности.
            BitmapImage backgroundImage = blockConverter.Convert("Even" + blockName, typeof(BitmapImage),
                null, CultureInfo.CurrentCulture) as BitmapImage;

            ToggleButton button = ConstructButton(backgroundImage, buttonId, BlockButton_Click);
            SetButtonIdAsContent(button, buttonId);

            return button;
        }

        private ToggleButton ConstructListenerButton(string objectType)
        {
            LevelObjectTypeConverter objectTypeConverter = new LevelObjectTypeConverter();

            BitmapImage backgroundImage = objectTypeConverter.Convert(objectType, typeof(BitmapImage),
                null, CultureInfo.CurrentCulture) as BitmapImage;

            ToggleButton button = ConstructButton(backgroundImage, objectType, ListenerButton_Click);
            button.ToolTip = objectType;

            return button;
        }

        private ToggleButton ConstructButton(BitmapImage backgroundImage, object tag, RoutedEventHandler e)
        {
            ToggleButton button = new ToggleButton()
            {
                Background = new ImageBrush(backgroundImage) { Stretch = Stretch.Uniform },
                Width = BUTTON_SIZE,
                Height = BUTTON_SIZE,
                Margin = new Thickness(BUTTON_MARGIN),
                Tag = tag,
            };

            button.Click += Button_Click; // To keep the code DRY
            button.Click += e;

            return button;
        }

        [Conditional("DEBUG")]
        private void SetButtonIdAsContent(ToggleButton button, int buttonId)
        {
            button.Content = buttonId.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentButtonState(false);
            (sender as ToggleButton).IsEnabled = false;
        }

        private void SelectionButton_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentButtonState(false);
            mainDataContext.CurrentEditingMode = EditingMode.Selection;
        }

        private void BlockButton_Click(object sender, RoutedEventArgs e)
        {
            mainDataContext.CurrentEditingMode = EditingMode.BlockPlacement;
            mainDataContext.SelectedBlockId = (int)((sender as ToggleButton).Tag);
        }

        private void ListenerButton_Click(object sender, RoutedEventArgs e)
        {
            mainDataContext.CurrentEditingMode = EditingMode.ListenerPlacement;
            mainDataContext.SelectedListenerType = (string)((sender as ToggleButton).Tag);
        }

        private void ButtonPanel_Loaded(object sender, RoutedEventArgs e)
        {
            mainDataContext = DataContext as MainDataContext;

            // MS, ну хули у тебя дизайнер-то костыли требует, а?
            if (!DesignerProperties.GetIsInDesignMode(sender as DependencyObject))
                SetCurrentButtonState(true);
        }

        private void SetCurrentButtonState(bool state)
        {
            if (mainDataContext.CurrentEditingMode == EditingMode.Selection)
            {
                SelectionButton.IsChecked = state;
                SelectionButton.IsEnabled = !state;
            }
            else if (mainDataContext.CurrentEditingMode == EditingMode.BlockPlacement)
            {
                blockButtons[mainDataContext.SelectedBlockId].IsChecked = state;
                blockButtons[mainDataContext.SelectedBlockId].IsEnabled = !state;
            }
            else if (mainDataContext.CurrentEditingMode == EditingMode.ListenerPlacement)
            {
                listenerButtons[mainDataContext.SelectedListenerType].IsChecked = state;
                listenerButtons[mainDataContext.SelectedListenerType].IsEnabled = !state;
            }
        }
    }
}
