using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Product
{
    public interface IHotkeyService
    {
        bool GetIsEnabled();
        void SetEnabled(bool isEnabled);
    }
}
