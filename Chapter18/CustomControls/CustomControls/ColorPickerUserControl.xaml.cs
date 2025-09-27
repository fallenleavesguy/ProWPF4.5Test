using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomControls
{
    /// <summary>
    /// Interaction logic for ColorPickerUserControl.xaml
    /// </summary>
    public partial class ColorPickerUserControl : UserControl
    {
        private Color? previousColor;

        public static readonly RoutedEvent ColorChangedEvent;
        public static DependencyProperty ColorProperty;
        public static DependencyProperty RedProperty;
        public static DependencyProperty GreenProperty;
        public static DependencyProperty BlueProperty;

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public byte Red
        {
            get { return (byte)GetValue(RedProperty); }
            set { SetValue(RedProperty, value); }
        }
        public byte Green
        {
            get { return (byte)GetValue(GreenProperty); }
            set { SetValue(GreenProperty, value); }
        }
        public byte Blue
        {
            get { return (byte)GetValue(BlueProperty); }
            set { SetValue(BlueProperty, value); }
        }

        static ColorPickerUserControl()
        {
            ColorProperty = DependencyProperty.Register(
                "Color",
                typeof(Color),
                typeof(ColorPickerUserControl),
                new FrameworkPropertyMetadata(
                    Colors.Black,
                    new PropertyChangedCallback(OnColorChanged)
                )
            );
            RedProperty = DependencyProperty.Register(
                "Red",
                typeof(byte),
                typeof(ColorPickerUserControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorRGBChanged))
            );
            GreenProperty = DependencyProperty.Register(
                "Green",
                typeof(byte),
                typeof(ColorPickerUserControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorRGBChanged))
            );

            BlueProperty = DependencyProperty.Register(
                "Blue",
                typeof(byte),
                typeof(ColorPickerUserControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnColorRGBChanged))
            );

            ColorChangedEvent = EventManager.RegisterRoutedEvent(
                "ColorChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<Color>),
                typeof(ColorPickerUserControl)
            );

            CommandManager.RegisterClassCommandBinding(
                typeof(ColorPickerUserControl),
                new CommandBinding(
                    ApplicationCommands.Undo,
                    UndoCommand_Executed,
                    UndoCommand_CanExecute
                )
            );
        }

        public event RoutedPropertyChangedEventHandler<Color> ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }

        public ColorPickerUserControl()
        {
            InitializeComponent();
            //SetUpCommands();
        }

        //private void SetUpCommands()
        //{
        //    CommandBinding binding = new CommandBinding(ApplicationCommands.Undo,
        //        UndoCommand_Executed, UndoCommand_CanExecute);

        //    this.CommandBindings.Add(binding);
        //}

        private static void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ColorPickerUserControl colorPicker = (ColorPickerUserControl)sender;
            e.CanExecute = colorPicker.previousColor.HasValue;
        }

        private static void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ColorPickerUserControl colorPicker = (ColorPickerUserControl)sender;
            colorPicker.Color = (Color)colorPicker.previousColor;
        }

        static void OnColorRGBChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerUserControl colorPicker = (ColorPickerUserControl)sender;
            Color color = colorPicker.Color;

            if (e.Property == RedProperty)
                color.R = (byte)e.NewValue;
            else if (e.Property == GreenProperty)
                color.G = (byte)e.NewValue;
            else if (e.Property == BlueProperty)
                color.B = (byte)e.NewValue;

            colorPicker.Color = color;
        }

        static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Color newColor = (Color)e.NewValue;

            ColorPickerUserControl colorPicker = (ColorPickerUserControl)sender;
            colorPicker.Red = newColor.R;
            colorPicker.Green = newColor.G;
            colorPicker.Blue = newColor.B;

            Color oldColor = (Color)e.OldValue;

            RoutedPropertyChangedEventArgs<Color> args = new RoutedPropertyChangedEventArgs<Color>(
                oldColor,
                newColor
            );
            args.RoutedEvent = ColorChangedEvent;
            colorPicker.RaiseEvent(args);

            colorPicker.previousColor = oldColor;
        }
    }
}
