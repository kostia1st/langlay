using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for LayoutColorSetEditor.xaml
    /// </summary>
    public partial class LayoutColorSetEditor : UserControl {
        public event RoutedEventHandler RemoveClick;
        public event RoutedEventHandler Changed;

        public LayoutColorSetViewModel ViewModel => (LayoutColorSetViewModel) DataContext;

        public LayoutColorSetEditor() {
            InitializeComponent();
            Loaded += LayoutColorSetEditor_Loaded;
            Unloaded += LayoutColorSetEditor_Unloaded;
        }

        public void SetFocus() {
            this.cbLayouts.Focus();
        }

        private void LayoutColorSetEditor_Unloaded(object sender, RoutedEventArgs e) {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void LayoutColorSetEditor_Loaded(object sender, RoutedEventArgs e) {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            Changed?.Invoke(this, new RoutedEventArgs());
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            RemoveClick?.Invoke(this, e);
        }
    }
}
