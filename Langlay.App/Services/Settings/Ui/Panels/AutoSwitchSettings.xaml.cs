using System.Windows;
using System.Windows.Controls;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for AutoSwitchSettings.xaml
    /// </summary>
    public partial class AutoSwitchSettings : UserControl {
        internal ConfigViewModel ViewModel {
            get => (ConfigViewModel) DataContext;
            set => DataContext = value;
        }

        public AutoSwitchSettings() {
            InitializeComponent();
        }

        private void AttachmentComposer_Changed(object sender, RoutedEventArgs e) {
            ViewModel.RaiseAttachmentListChanged();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) {

        }
    }
}
