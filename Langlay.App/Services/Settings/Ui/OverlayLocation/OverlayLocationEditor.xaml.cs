using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for OverlayLocationEditor.xaml
    /// </summary>
    public partial class OverlayLocationEditor : UserControl {
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(
               "Value", typeof(OverlayLocation), typeof(OverlayLocationEditor),
               new PropertyMetadata(OverlayLocation.None, OnValueChanged));

        public OverlayLocation Value {
            get => (OverlayLocation) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private OverlayLocationHolder[] Holders { get; set; }

        public OverlayLocationEditor() {
            InitializeComponent();
            Holders = new[]
            {
                hTopLeft, hTopCenter, hTopRight,
                hMiddleLeft, hMiddleCenter, hMiddleRight,
                hBottomLeft, hBottomCenter, hBottomRight
            };
            Loaded += OverlayLocationEditor_Loaded;
        }

        private void OverlayLocationEditor_Loaded(object sender, RoutedEventArgs e) {
            foreach (var holder in Holders) {
                holder.MouseDown += Holder_MouseDown;
            }
        }

        private void Holder_MouseDown(object sender, MouseButtonEventArgs e) {
            var holder = (OverlayLocationHolder) sender;
            Value = holder.Location;
            e.Handled = true;
        }

        private void OnValueChanged(OverlayLocation location) {
            foreach (var holder in Holders) {
                holder.IsSelected = holder.Location == location;
            }
        }

        protected static void OnValueChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs args) {
            var control = (OverlayLocationEditor) d;
            if (control != null) {
                control.OnValueChanged((OverlayLocation) args.NewValue);
            }
        }
    }
}