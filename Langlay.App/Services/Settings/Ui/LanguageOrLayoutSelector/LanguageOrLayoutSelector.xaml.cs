using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for LanguageOrLayoutSelector.xaml
    /// </summary>
    public partial class LanguageOrLayoutSelector : UserControl {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
             "Value", typeof(string), typeof(LanguageOrLayoutSelector),
             new PropertyMetadata(null, OnValueChanged));

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
             "Mode", typeof(LanguageOrLayoutSelectorMode), typeof(LanguageOrLayoutSelector),
             new PropertyMetadata(LanguageOrLayoutSelectorMode.LanguageAndLayout, OnModeChanged));

        public static readonly DependencyProperty LanguageOrLayoutListProperty = DependencyProperty.Register(
             "LanguageOrLayoutList", typeof(IList<LanguageOrLayoutViewModel>), typeof(LanguageOrLayoutSelector),
             new PropertyMetadata(null, OnLanguageOrLayoutListChanged));

        public string Value {
            get => (string) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public LanguageOrLayoutSelectorMode Mode {
            get => (LanguageOrLayoutSelectorMode) GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        internal protected IList<LanguageOrLayoutViewModel> LanguageOrLayoutList {
            get => (IList<LanguageOrLayoutViewModel>) GetValue(LanguageOrLayoutListProperty);
            set => SetValue(LanguageOrLayoutListProperty, value);
        }

        public LanguageOrLayoutSelector() {
            InitializeComponent();

            if (ServiceRegistry.Instance != null) {
                var languageService = ServiceRegistry.Instance.Get<ILanguageService>();
                if (languageService != null) {
                    LanguageOrLayoutList = LanguageOrLayoutHelper.GetList(Mode);
                }
            }
            cbLanguagesAndLayouts.DataContext = this;
        }

        private void OnValueChanged(string value) {
            Value = value;
        }

        protected static void OnValueChanged(
           DependencyObject d, DependencyPropertyChangedEventArgs args) {
            var control = (LanguageOrLayoutSelector) d;
            if (control != null) {
                control.OnValueChanged((string) args.NewValue);
            }
        }

        private void OnModeChanged(LanguageOrLayoutSelectorMode value) {
            Mode = value;
            LanguageOrLayoutList = LanguageOrLayoutHelper.GetList(value);
        }

        protected static void OnModeChanged(
           DependencyObject d, DependencyPropertyChangedEventArgs args) {
            var control = (LanguageOrLayoutSelector) d;
            if (control != null) {
                control.OnModeChanged((LanguageOrLayoutSelectorMode) args.NewValue);
            }
        }

        private void OnLanguageOrLayoutListChanged(IList<LanguageOrLayoutViewModel> value) {
            LanguageOrLayoutList = value;
        }

        protected static void OnLanguageOrLayoutListChanged(
           DependencyObject d, DependencyPropertyChangedEventArgs args) {
            var control = (LanguageOrLayoutSelector) d;
            if (control != null) {
                control.OnLanguageOrLayoutListChanged((IList<LanguageOrLayoutViewModel>) args.NewValue);
            }
        }

    }
}
