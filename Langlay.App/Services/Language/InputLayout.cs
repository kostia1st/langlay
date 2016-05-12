using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Product
{
    public class InputLayout
    {
        public string LanguageName { get; set; }
        public string LanguageNameNative { get; set; }
        public string LanguageNameTwoLetter { get; set; }
        public string LanguageNameTwoLetterNative { get; set; }
        public string LanguageNameThreeLetter { get; set; }
        public string LanguageNameThreeLetterNative { get; set; }
        public string Name { get; set; }
        public IntPtr Handle { get; set; }

        private static IDictionary<IntPtr, string> _layoutNames
            = new Dictionary<IntPtr, string>();

        public InputLayout(InputLanguage inputLanguage)
        {
            Handle = inputLanguage.Handle;
            LanguageName = inputLanguage.Culture.EnglishName;
            LanguageNameNative = inputLanguage.Culture.NativeName;

            LanguageNameTwoLetter = inputLanguage.Culture.TwoLetterISOLanguageName;
            LanguageNameTwoLetterNative = inputLanguage.Culture.NativeName.Substring(0, 2);
            LanguageNameThreeLetter = inputLanguage.Culture.ThreeLetterISOLanguageName;
            LanguageNameThreeLetterNative = inputLanguage.Culture.NativeName.Substring(0, 3);

            // Layout names per handle are not going to change, ever
            if (!_layoutNames.ContainsKey(Handle))
                _layoutNames[Handle] = inputLanguage.LayoutName;
            Name = _layoutNames[Handle];
        }
    }
}