using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for AttachmentEditor.xaml
    /// </summary>
    public partial class AttachmentEditor : UserControl {
        public event RoutedEventHandler RemoveClick;
        public event RoutedEventHandler Changed;

        public AttachmentViewModel ViewModel => (AttachmentViewModel) DataContext;

        public AttachmentEditor() {
            InitializeComponent();
            Loaded += AttachmentEditor_Loaded;
            Unloaded += AttachmentEditor_Unloaded;
        }

        private void AttachmentEditor_Unloaded(object sender, RoutedEventArgs e) {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void AttachmentEditor_Loaded(object sender, RoutedEventArgs e) {
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
