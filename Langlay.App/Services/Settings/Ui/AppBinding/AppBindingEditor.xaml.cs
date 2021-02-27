using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for AppBindingEditor.xaml
    /// </summary>
    public partial class AppBindingEditor : UserControl {
        public event RoutedEventHandler RemoveClick;
        public event RoutedEventHandler Changed;

        public AppBindingViewModel ViewModel => (AppBindingViewModel) DataContext;

        public AppBindingEditor() {
            InitializeComponent();
            Loaded += AppBindingEditor_Loaded;
            Unloaded += AppBindingEditor_Unloaded;
        }

        public void SetFocus() {
            this.tbAppTitleMask.Focus();
        }

        private void AppBindingEditor_Unloaded(object sender, RoutedEventArgs e) {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void AppBindingEditor_Loaded(object sender, RoutedEventArgs e) {
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
