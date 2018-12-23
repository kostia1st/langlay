namespace Product.SettingsUi {
    public class LayoutViewModel {
        private InputLayout Layout { get; set; }

        public int Id {
            get => Layout.Id;
        }

        public string DisplayName {
            get => Layout.LanguageNameThreeLetter + " - " + Layout.Name;
        }

        public LayoutViewModel(InputLayout layout) {
            Layout = layout;
        }
    }
}
