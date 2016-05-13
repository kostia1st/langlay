using System.Collections.Generic;
using Product.Common;

namespace Product
{
    public interface IConfigService
    {
        bool DoShowOverlay { get; }
        bool DoShowOverlayOnMainDisplayOnly { get; }
        bool DoShowOverlayRoundCorners { get; }
        bool DoShowLanguageNameInNative { get; }
        uint OverlayDuration { get; }
        uint OverlayOpacity { get; }
        uint OverlayScale { get; }
        OverlayLocation OverlayLocation { get; }
        bool DoRunAtWindowsStartup { get; }

        bool DoSwitchLanguage { get; }
        IList<KeyCode> LanguageSwitchKeyArray { get; }
        bool DoSwitchLayout { get; }
        IList<KeyCode> LayoutSwitchKeyArray { get; }

        bool DoDisableCapsLockToggle { get; }
        bool DoShowSettingsOnce { get; }
        bool DoShowCursorTooltip { get; }
        bool DoForceThisInstance { get; }
        SwitchMethod SwitchMethod { get; }

        bool GetLanguageSwitchConfigured();

        bool GetLayoutSwitchConfigured();
    }
}