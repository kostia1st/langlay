using System.Windows;
using System.Windows.Controls;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for SettingPanel.xaml
    /// </summary>
    public partial class SettingPanel : UserControl
    {
        internal ConfigViewModel ViewModel
        {
            get => (ConfigViewModel) DataContext;
            set => DataContext = value;
        }

        public SettingPanel()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            spSettings.Content = null;
            var item = (ListBoxItem) lbCategory.SelectedItem;
            if (item != null)
            {
                UserControl control = null;
                switch (item.Name)
                {
                    case "lbiGeneral":
                        control = new GeneralSettings();
                        break;
                    case "lbiOverlay":
                        control = new OverlaySettings();
                        break;
                    case "lbiLanguage":
                        control = new LanguageSwitchSettings();
                        break;
                    case "lbiPlainPaste":
                        control = new PlainPasteSettings();
                        break;
                }
                if (control != null)
                {
                    control.DataContext = ViewModel;
                    spSettings.Content = control;
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            lbCategory.SelectedIndex = 0;
        }
    }
}
