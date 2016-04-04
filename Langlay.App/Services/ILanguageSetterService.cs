using System;

namespace Product
{
    public interface ILanguageSetterService
    {
        bool SetCurrentLayout(IntPtr handle);
    }
}
