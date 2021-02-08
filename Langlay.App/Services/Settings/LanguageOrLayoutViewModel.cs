namespace Product.SettingsUi {
    public class LanguageOrLayoutViewModel {
        private InputLayout Layout { get; set; }
        private InputLanguage Language { get; set; }

        public string LanguageOrLayoutId {
            get {
                if (Layout != null) return Layout.LayoutId;
                if (Language != null) return Language.LanguageId;
                return null;
            }
        }

        public string DisplayName {
            get {
                if (Layout != null) {
                    return Layout.LanguageNameThreeLetter + " - " + Layout.Name;
                } else if (Language != null) {
                    return Language.LanguageNameThreeLetter;
                }
                return "Invalid object";
            }
        }

        public LanguageOrLayoutViewModel(InputLayout layout) {
            Layout = layout;
        }

        public LanguageOrLayoutViewModel(InputLanguage language) {
            Language = language;
        }
    }
}
