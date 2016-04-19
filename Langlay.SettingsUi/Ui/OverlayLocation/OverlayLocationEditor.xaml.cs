using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Product.Common;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for OverlayLocationEditor.xaml
    /// </summary>
    public partial class OverlayLocationEditor : UserControl
    {
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(
               "Value", typeof(OverlayLocation), typeof(OverlayLocationEditor),
               new PropertyMetadata(OverlayLocation.None, OnValueChanged));
        public OverlayLocation Value
        {
            get { return (OverlayLocation) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private OverlayLocationHolder[] Holders { get; set; }
        public OverlayLocationEditor()
        {
            InitializeComponent();
            Holders = new[] {
                hTopLeft, hTopCenter, hTopRight,
                hMiddleLeft, hMiddleCenter, hMiddleRight,
                hBottomLeft, hBottomCenter, hBottomRight
            };
            Loaded += OverlayLocationEditor_Loaded;
        }

        private void OverlayLocationEditor_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var holder in Holders)
            {
                holder.MouseDown += Holder_MouseDown;
            }
        }

        private void Holder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var holder = (OverlayLocationHolder) sender;
            Value = holder.Location;
            e.Handled = true;
        }

        protected static void OnValueChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var control = (OverlayLocationEditor) d;
            if (control != null)
            {
                var value = (OverlayLocation) args.NewValue;
                foreach (var holder in control.Holders)
                {
                    holder.IsSelected = holder.Location == value;
                }
            }
        }
    }
}
