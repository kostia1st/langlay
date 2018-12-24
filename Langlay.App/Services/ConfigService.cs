using System.Collections.Generic;
using Product.Common;

namespace Product {
    public class ConfigService : IConfigService {
        private ConfigPersister Persister { get; set; }

        public bool DoSwitchLanguage { get; set; }
        public IList<KeyCode> LanguageSwitchKeyArray { get; set; }

        public bool DoSwitchLayout { get; set; }
        public IList<KeyCode> LayoutSwitchKeyArray { get; set; }

        public bool DoPasteWithoutFormatting { get; set; }
        public IList<KeyCode> PasteKeyArray { get; set; }

        public IList<AppAttachment> AppAttachmentArray { get; set; }

        public bool DoDisableCapsLockToggle { get; set; }

        public bool DoShowOverlay { get; set; }
        public bool DoShowOverlayOnMainDisplayOnly { get; set; }
        public bool DoShowOverlayRoundCorners { get; set; }
        public bool DoShowLanguageNameInNative { get; set; }
        public uint OverlayDuration { get; set; }
        public uint OverlayOpacity { get; set; }
        public uint OverlayScale { get; set; }
        public OverlayLocation OverlayLocation { get; set; }

        public SwitchMethod SwitchMethod { get; set; }
        public bool DoRunAtWindowsStartup { get; set; }
        public bool DoShowSettingsOnce { get; set; }

        public bool DoForceThisInstance { get; set; }

        public bool DoShowCursorTooltip { get; set; }
        public bool DoShowCursorTooltip_WhenFocusNotChanged { get; set; }

        public ConfigService() {
            InitializeDefaultValues();
            Persister = new ConfigPersister(this);
        }

        private void InitializeDefaultValues() {
            DoRunAtWindowsStartup = true;
            DoShowSettingsOnce = true;

            DoSwitchLanguage = true;
            LanguageSwitchKeyArray = new KeyCode[] { KeyCode.CapsLock };

            DoSwitchLayout = false;
            LayoutSwitchKeyArray = new KeyCode[] { };

            DoPasteWithoutFormatting = false;
            PasteKeyArray = new KeyCode[] { KeyCode.LControlKey, KeyCode.LShiftKey, KeyCode.V };

            AppAttachmentArray = new AppAttachment[] { };

            DoDisableCapsLockToggle = false;
            SwitchMethod = SwitchMethod.InputSimulation;
            DoShowOverlay = true;
            OverlayDuration = 300;
            OverlayOpacity = 80;
            OverlayScale = 100;
            OverlayLocation = OverlayLocation.BottomCenter;
        }

        public bool GetLanguageSwitchConfigured() {
            return DoSwitchLanguage
                && LanguageSwitchKeyArray.Count > 0;
        }

        public bool GetLayoutSwitchConfigured() {
            return DoSwitchLayout
                && LayoutSwitchKeyArray.Count > 0;
        }

        public void SaveToFile() {
            Persister.SaveToFile();
        }

        public void ReadFromConfigFile(bool isUserLevel) {
            Persister.ReadFromConfigFile(isUserLevel);
        }

        public void ReadFromCommandLineArguments() {
            Persister.ReadFromCommandLineArguments();
        }
    }
}