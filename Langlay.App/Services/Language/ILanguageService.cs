using System;
using System.Collections.Generic;

namespace Product
{
    public interface ILanguageService
    {
        void ConductSwitch(KeyboardSwitch keyboardSwitch);

        /// <summary>
        /// Returns the currently active input layout.
        /// </summary>
        /// <returns>
        /// returning null makes no sense, so the method will throw an
        /// exception if it cannot resolve the layout to a usable value.
        /// </returns>
        InputLayout GetCurrentLayout();

        IntPtr GetCurrentLayoutHandle();

        IList<InputLayout> GetInputLayouts();
    }
}