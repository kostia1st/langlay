using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public static class InputLayoutHelper
    {
        public static IList<InputLayout> InputLayouts
        {
            get
            {
                return InputLanguage.InstalledInputLanguages
                  .Cast<InputLanguage>()
                  .Select(x => new InputLayout(x))
                  .ToList();
            }
        }

        public static IList<InputLayout> GetLayoutsByLanguage(string languageName)
        {
            return InputLayouts.Where(x => x.LanguageName == languageName).ToList();
        }

        public static string GetNextInputLayoutName(string currentLanguageName, string currentLayoutName, bool doWrap)
        {
            var layoutNames = GetLayoutsByLanguage(currentLanguageName)
                .Select(x => x.Name)
                .ToList();
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

        public static InputLayout GetInputLanguageByHandle(IntPtr handle)
        {
            return InputLayouts.FirstOrDefault(x => x.Handle == handle);
        }

        public static InputLayout GetCurrentLayout()
        {
            var currentLayoutHandle = Win32.GetKeyboardLayout(
               Win32.GetWindowThreadProcessId(Win32.GetForegroundWindow(), IntPtr.Zero));
            return InputLayouts.FirstOrDefault(x => x.Handle == currentLayoutHandle);
        }

        public static string GetNextInputLanguageName(string currentLanguageName)
        {
            var languageNames = InputLayouts.Select(x => x.LanguageName).Distinct().ToList();
            var indexOfNext = languageNames.IndexOf(currentLanguageName) + 1;
            if (indexOfNext >= languageNames.Count)
                indexOfNext = 0;
            return languageNames[indexOfNext];
        }

        public static IntPtr GetDefaultLayoutForLanguage(string languageName)
        {
            // Avoid re-evaluating properties
            var inputLanguages = InputLayouts;
            var firstLanguageLayout = inputLanguages.FirstOrDefault(x => x.LanguageName == languageName);
            if (firstLanguageLayout == null)
                firstLanguageLayout = inputLanguages.FirstOrDefault();
            if (firstLanguageLayout == null)
                throw new NullReferenceException("Not a single language's installed in the system");

            return firstLanguageLayout.Handle;
        }

        public static InputLayout GetLayoutByLanguageAndLayoutName(string languageName, string layoutName)
        {
            return InputLayouts.FirstOrDefault(x => x.LanguageName == languageName && x.Name == layoutName);
        }

    }
}
