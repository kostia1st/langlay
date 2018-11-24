using System.Windows;
using System.Windows.Controls;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for PlainPasteSettings.xaml
    /// </summary>
    public partial class PlainPasteSettings : UserControl
    {
        internal ConfigViewModel ViewModel
        {
            get => (ConfigViewModel) DataContext;
            set => DataContext = value;
        }

        public PlainPasteSettings()
        {
            InitializeComponent();
        }

        private void HotkeyComposer_Paste_Changed(object sender, RoutedEventArgs e)
        {
            ViewModel.RaisePasteSequenceChanged();
        }
    }
}
