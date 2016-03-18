using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Langwitch
{
    public interface IConfigService
    {
        IList<int> LanguageSwitchKeyArray { get; }
        Keys LanguageSwitchKeys { get; }
        IList<int> LayoutSwitchKeyArray { get; }
        Keys LayoutSwitchKeys { get; }
    }
}
