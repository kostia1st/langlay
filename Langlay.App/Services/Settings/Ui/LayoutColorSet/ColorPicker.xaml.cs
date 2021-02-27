using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Product.Common;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl {
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(
               "Value", typeof(Color), typeof(ColorPicker),
               new PropertyMetadata(Colors.Black, OnValueChanged));
        
        public Color Value {
            get => (Color) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public ColorPicker() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var color = Value;
            var colorDialog = new System.Windows.Forms.ColorDialog() {
                Color = color.ToWinForms()
            };
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Value = colorDialog.Color.ToWpf();
            }
        }

        private void OnValueChanged(Color value) {
            Value = value;
        }

        protected static void OnValueChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs args) {
            var control = (ColorPicker) d;
            if (control != null) {
                control.OnValueChanged((Color) args.NewValue);
            }
        }
    }
}
