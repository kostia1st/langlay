using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Product.Common;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for LanguageSwitchSettings.xaml
    /// </summary>
    public partial class LanguageSwitchSettings : UserControl {
        internal ConfigViewModel ViewModel {
            get => (ConfigViewModel) DataContext;
            set => DataContext = value;
        }

        public LanguageSwitchSettings() {
            InitializeComponent();
        }

        private void UpdateHotkeyAnalysis() {
            if (IsLoaded) {
                tbkFeedbackLanguage.Text = GetAnalysisByHotkey(ViewModel.LanguageSwitchSequence);
                tbkFeedbackLayout.Text = GetAnalysisByHotkey(ViewModel.LayoutSwitchSequence);
            }
        }

        private string GetAnalysisByHotkey(IList<KeyCodeViewModel> keyViewModels) {
            var keys = keyViewModels.Select(x => x.KeyCode).ToList();
            var result = new StringBuilder();
            if (keys.Count == 1) {
                if (keys[0] == KeyCode.CapsLock)
                    result.AppendLine("Note, to press the *actual* Caps Lock you can use Shift + Caps Lock.");
            } else if (keys.Count > 1) {
                if (keys.Contains(KeyCode.LWin) || keys.Contains(KeyCode.RWin))
                    result.AppendLine("The use of the Win key in a combination could probably lead to issues"
                        + " if Win is applied not the last in the sequence.");
            }
            return result.ToString().TrimEnd('\r', '\n');
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            UpdateHotkeyAnalysis();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            UpdateHotkeyAnalysis();
        }

        private void HotkeyComposer_Layout_Changed(object sender, RoutedEventArgs e) {
            ViewModel.RaiseLayoutSequenceChanged();
        }

        private void HotkeyComposer_Language_Changed(object sender, RoutedEventArgs e) {
            ViewModel.RaiseLanguageSequenceChanged();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }
    }
}
