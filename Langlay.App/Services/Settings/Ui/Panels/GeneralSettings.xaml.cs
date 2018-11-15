using System.Windows.Controls;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for GeneralSettings.xaml
    /// </summary>
    public partial class GeneralSettings : UserControl
    {
        internal ConfigViewModel ViewModel
        {
            get => (ConfigViewModel) DataContext;
            set => DataContext = value;
        }

        public GeneralSettings()
        {
            InitializeComponent();
        }
    }
}
