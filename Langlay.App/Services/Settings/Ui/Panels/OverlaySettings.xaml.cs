using System.Windows.Controls;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for OverlaySettings.xaml
    /// </summary>
    public partial class OverlaySettings : UserControl
    {
        internal ConfigViewModel ViewModel
        {
            get => (ConfigViewModel) DataContext;
            set => DataContext = value;
        }

        public OverlaySettings()
        {
            InitializeComponent();
        }
    }
}
