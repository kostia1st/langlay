using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Langwitch
{
    public class InputLayout
    {
        public string LanguageName { get; set; }
        public string LanguageNameTwoLetter { get; set; }
        public string Name { get; set; }
        public IntPtr Handle { get; set; }

        public InputLayout(InputLanguage inputLanguage)
        {
            LanguageName = inputLanguage.Culture.EnglishName;
            LanguageNameTwoLetter = inputLanguage.Culture.TwoLetterISOLanguageName;
            Name = inputLanguage.LayoutName;
            Handle = inputLanguage.Handle;
        }
    }
}
