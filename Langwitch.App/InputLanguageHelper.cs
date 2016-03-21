using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Langwitch
{
    public static class InputLanguageHelper
    {
        private static IList<InputLanguage> InputLanguages
        {
            get { return InputLanguage.InstalledInputLanguages.Cast<InputLanguage>().ToList(); }
        }

        public static IList<InputLanguage> GetLayoutsByLanguage(string languageName)
        {
            return InputLanguages.Where(x => x.Culture.EnglishName == languageName).ToList();
        }

        public static string GetNextInputLayoutName(string currentLanguageName, string currentLayoutName, bool doWrap)
        {
            var layoutNames = GetLayoutsByLanguage(currentLanguageName).Select(x => x.LayoutName).ToList();
            var indexOfNext = layoutNames.IndexOf(currentLayoutName) + 1;
            if (indexOfNext >= layoutNames.Count)
            {
                if (doWrap)
                    return layoutNames[0];
                else
                    return null;
            }
            return layoutNames[indexOfNext];
        }

        public static InputLanguage GetInputLanguageByHandle(IntPtr handle)
        {
            return InputLanguages.FirstOrDefault(x => x.Handle == handle);
        }

        public static InputLanguage GetCurrentInputLanguage()
        {
            var currentLayoutHandle = SafeMethods.GetKeyboardLayout(
               SafeMethods.GetWindowThreadProcessId(SafeMethods.GetForegroundWindow(), 0));
            return InputLanguages.FirstOrDefault(x => x.Handle == currentLayoutHandle);
        }

        public static string GetNextInputLanguageName(string currentLanguageName)
        {
            var languageNames = InputLanguages.Select(x => x.Culture.EnglishName).Distinct().ToList();
            var indexOfNext = languageNames.IndexOf(currentLanguageName) + 1;
            if (indexOfNext >= languageNames.Count)
                indexOfNext = 0;
            return languageNames[indexOfNext];
        }

        public static IntPtr GetDefaultLayoutForLanguage(string languageName)
        {
            // Avoid re-evaluating properties
            var inputLanguages = InputLanguages;
            var firstLanguageLayout = inputLanguages.FirstOrDefault(x => x.Culture.EnglishName == languageName);
            if (firstLanguageLayout == null)
                firstLanguageLayout = inputLanguages.FirstOrDefault();
            if (firstLanguageLayout == null)
                throw new NullReferenceException("Not a single language's installed in the system");

            return firstLanguageLayout.Handle;
        }

        public static InputLanguage GetLayoutByLanguageAndLayoutName(string languageName, string layoutName)
        {
            return InputLanguages.FirstOrDefault(x => x.Culture.EnglishName == languageName && x.LayoutName == layoutName);
        }

    }
}
