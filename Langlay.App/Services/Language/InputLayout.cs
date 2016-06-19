using System;
using System.Windows.Forms;

namespace Product
{
    public class InputLayout
    {
        public string LanguageName { get; private set; }
        public string LanguageNameNative { get; private set; }
        public string LanguageNameTwoLetter { get; private set; }
        public string LanguageNameTwoLetterNative { get; private set; }
        public string LanguageNameThreeLetter { get; private set; }
        public string LanguageNameThreeLetterNative { get; private set; }
        public string Name { get; private set; }
        public IntPtr Handle { get; private set; }

        public InputLayout(InputLanguage inputLanguage)
        {
            Handle = inputLanguage.Handle;
            LanguageName = inputLanguage.Culture.EnglishName;
            LanguageNameNative = inputLanguage.Culture.NativeName;

            LanguageNameTwoLetter = inputLanguage.Culture.TwoLetterISOLanguageName;
            LanguageNameTwoLetterNative = inputLanguage.Culture.NativeName.Substring(0, 2);
            LanguageNameThreeLetter = inputLanguage.Culture.ThreeLetterISOLanguageName;
            LanguageNameThreeLetterNative = inputLanguage.Culture.NativeName.Substring(0, 3);

            Name = inputLanguage.LayoutName;
        }
    }
}