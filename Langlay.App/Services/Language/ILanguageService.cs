using System.Collections.Generic;

namespace Product
{
    public interface ILanguageService
    {
        void ConductSwitch(KeyboardSwitch keyboardSwitch);

        InputLayout GetCurrentLayout();

        IList<InputLayout> GetInputLayouts();
    }
}