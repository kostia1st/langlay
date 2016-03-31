using System.Collections.Generic;
using System.Windows.Forms;

namespace Product
{
    public interface IConfigService
    {
        IList<int> LanguageSwitchKeyArray { get; }
        Keys LanguageSwitchKeys { get; }
        IList<int> LayoutSwitchKeyArray { get; }
        Keys LayoutSwitchKeys { get; }
        bool ShowOverlay { get; }
        long OverlayMilliseconds { get; }
        bool DoRunAtWindowsStartup { get; }
    }
}
