using System;

namespace Langwitch
{
    public interface ILanguageSetterService
    {
        bool SetCurrentLayout(IntPtr handle);
    }
}
