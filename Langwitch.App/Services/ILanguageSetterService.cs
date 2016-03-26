using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Langwitch
{
    public interface ILanguageSetterService
    {
        bool SetCurrentLayout(IntPtr handle);
    }
}
