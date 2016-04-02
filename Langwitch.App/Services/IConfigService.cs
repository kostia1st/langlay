using System.Collections.Generic;
using Product.Common;

namespace Product
{
    public interface IConfigService
    {
        IList<KeyCode> LanguageSwitchKeyArray { get; }
        KeyCode LanguageSwitchKeys { get; }
        IList<KeyCode> LayoutSwitchKeyArray { get; }
        KeyCode LayoutSwitchKeys { get; }
        bool ShowOverlay { get; }
        long OverlayMilliseconds { get; }
        bool DoRunAtWindowsStartup { get; }
        bool DoSwitchLanguage { get; }
        bool DoSwitchLayout { get; }
    }
}
