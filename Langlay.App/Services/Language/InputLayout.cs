using System;
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