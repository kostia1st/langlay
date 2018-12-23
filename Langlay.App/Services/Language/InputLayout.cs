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
        public int Id { get; private set; }
        public IntPtr Handle { get; private set; }

        public InputLayout(InputLanguage inputLanguage)
        {
            Handle = inputLanguage.Handle;
            Name = inputLanguage.LayoutName;
            Id = Name.GetHashCode();

            var culture = inputLanguage.Culture;

            LanguageName = culture.EnglishName;
            LanguageNameNative = culture.NativeName;

            LanguageNameTwoLetter = culture.TwoLetterISOLanguageName;
            LanguageNameTwoLetterNative = culture.NativeName.Substring(0, 2);
            LanguageNameThreeLetter = culture.ThreeLetterISOLanguageName;
            LanguageNameThreeLetterNative = culture.NativeName.Substring(0, 3);
        }

        public override string ToString() {
            return $"{LanguageName} - {Name} - {Id}";
        }
    }
}