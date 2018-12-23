using System.Collections.Generic;
using Product.Common;

namespace Product {
    public interface IConfigService {
        bool DoShowOverlay { get; set; }
        bool DoShowOverlayOnMainDisplayOnly { get; set; }
        bool DoShowOverlayRoundCorners { get; set; }
        bool DoShowLanguageNameInNative { get; set; }
        uint OverlayDuration { get; set; }
        uint OverlayOpacity { get; set; }
        uint OverlayScale { get; set; }
        OverlayLocation OverlayLocation { get; set; }
        bool DoRunAtWindowsStartup { get; set; }

        bool DoSwitchLanguage { get; set; }
        IList<KeyCode> LanguageSwitchKeyArray { get; set; }
        bool DoSwitchLayout { get; set; }
        IList<KeyCode> LayoutSwitchKeyArray { get; set; }

        bool DoPasteWithoutFormatting { get; set; }
        IList<KeyCode> PasteKeyArray { get; set; }

        IList<AppAttachment> AppAttachmentArray { get; set; }

        bool DoDisableCapsLockToggle { get; set; }
        bool DoShowSettingsOnce { get; set; }
        bool DoShowCursorTooltip { get; set; }
        bool DoShowCursorTooltip_WhenFocusNotChanged { get; set; }
        bool DoForceThisInstance { get; set; }
        SwitchMethod SwitchMethod { get; set; }

        bool GetLanguageSwitchConfigured();

        bool GetLayoutSwitchConfigured();

        void SaveToFile();
    }
}