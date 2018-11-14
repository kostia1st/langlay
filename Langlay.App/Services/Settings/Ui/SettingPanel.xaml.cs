using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Product.Common;

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
            Loaded += SettingPanel_Loaded;
        }

        private void SettingPanel_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateHotkeyAnalysis();
        }

        private void UpdateHotkeyAnalysis()
        {
            if (IsLoaded)
            {
                tbkFeedbackLanguage.Text = GetAnalysisByHotkey(ViewModel.LanguageSwitchSequence);
                tbkFeedbackLayout.Text = GetAnalysisByHotkey(ViewModel.LayoutSwitchSequence);
            }
        }

        private string GetAnalysisByHotkey(IList<KeyCodeViewModel> keyViewModels)
        {
            var keys = keyViewModels.Select(x => x.KeyCode).ToList();
            var result = new StringBuilder();
            if (keys.Count == 1)
            {
                if (keys[0] == KeyCode.CapsLock)
                    result.AppendLine("Note, to press the *actual* Caps Lock you can use Shift + Caps Lock.");
            }
            else if (keys.Count > 1)
            {
                if (keys.Contains(KeyCode.LWin) || keys.Contains(KeyCode.RWin))
                    result.AppendLine("The use of the Win key in a combination could probably lead to issues"
                        + " if Win is applied not the last in the sequence.");
            }
            return result.ToString().TrimEnd('\r', '\n');
        }
        private void DoOnViewModelChanged()
        {
            UpdateHotkeyAnalysis();
        }

        private void HotkeyComposer_Layout_Changed(object sender, RoutedEventArgs e)
        {
            ViewModel.NotifyLayoutSequenceChanged();
        }

        private void HotkeyComposer_Language_Changed(object sender, RoutedEventArgs e)
        {
            ViewModel.RaiseLanguageSequenceChanged();
        }

        private void HotkeyComposer_Paste_Changed(object sender, RoutedEventArgs e)
        {
            ViewModel.RaisePasteSequenceChanged();
        }
    }
}
