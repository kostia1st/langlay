using System.Windows;
using System.Windows.Controls;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for AutoSwitchSettings.xaml
    /// </summary>
    public partial class ColorSettings : UserControl {
        internal ConfigViewModel ViewModel {
            get => (ConfigViewModel) DataContext;
            set => DataContext = value;
        }

        public ColorSettings() {
            InitializeComponent();
        }

        private void LayoutColorSetComposer_Changed(object sender, RoutedEventArgs e) {
            ViewModel.RaiseLayoutColorSetListChanged();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) {

        }
    }
}
