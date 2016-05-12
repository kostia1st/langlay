using System.Collections.Generic;
using Product.Common;

namespace Product
{
    public interface IConfigService
    {
        IList<KeyCode> LanguageSwitchKeyArray { get; }
        IList<KeyCode> LayoutSwitchKeyArray { get; }

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
        bool DoShowSettingsOnce { get; }
        bool DoShowCursorTooltip { get; }
        bool DoSwitchLayout { get; }
        bool DoForceThisInstance { get; }
        SwitchMethod SwitchMethod { get; }

        bool GetLanguageSwitchConfigured();

        bool GetLayoutSwitchConfigured();
    }
}