using System.Globalization;

namespace Product
{
	public class InputLanguage
	{
        public CultureInfo Culture { get; private set; }
        public string LanguageId { get => LanguageName; }
        public string LanguageName { get; private set; }
        public string LanguageNameNative { get; private set; }
        public string LanguageNameTwoLetter { get; private set; }
        public string LanguageNameTwoLetterNative { get; private set; }
        public string LanguageNameThreeLetter { get; private set; }
        public string LanguageNameThreeLetterNative { get; private set; }

        public InputLanguage(CultureInfo culture)
		{
            Culture = culture;
            LanguageName = culture.EnglishName;
            LanguageNameNative = culture.NativeName;

            LanguageNameTwoLetter = culture.TwoLetterISOLanguageName;
            LanguageNameTwoLetterNative = culture.NativeName.Substring(0, 2);
            LanguageNameThreeLetter = culture.ThreeLetterISOLanguageName;
            LanguageNameThreeLetterNative = culture.NativeName.Substring(0, 3);
        }

        public override string ToString()
        {
            return $"{LanguageName}";
        }
    }
}
