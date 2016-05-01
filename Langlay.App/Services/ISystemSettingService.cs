using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Product
{
    public interface ISystemSettingService
    {
        WindowsSequenceCode? GetLanguageSwitchSequence();
        WindowsSequenceCode? GetLayoutSwitchSequence();
        bool GetIsShiftToDisableCapsLock();
    }
}
