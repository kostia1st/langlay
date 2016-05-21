using System;
using System.Collections.Generic;

namespace Product
{
    public interface ILanguageService
    {
        void ConductSwitch(KeyboardSwitch keyboardSwitch);

        InputLayout GetCurrentLayout();

        IntPtr GetCurrentLayoutHandle();

        IList<InputLayout> GetInputLayouts();
    }
}