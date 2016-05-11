using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public static class InputLayoutHelper
    {
        public static IList<InputLayout> GetInputLayouts()
        {
            // Surprisingly, this is quite expensive.
            // According to ILSpy there're lots of unsafe method and registry usage
            // each time you read the property and sub-properties.
            return InputLanguage.InstalledInputLanguages
              .Cast<InputLanguage>()
              .Select(x => new InputLayout(x))
              .ToList();
        }

        public static IList<InputLayout> GetLayoutsByLanguage(string languageName)
        {
            return GetInputLayouts().Where(x => x.LanguageName == languageName).ToList();
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
            return GetInputLayouts().FirstOrDefault(x => x.Handle == handle);
        }

        public static InputLayout GetCurrentLayout()
        {
            var currentLayoutHandle = Win32.GetKeyboardLayout(
               Win32.GetWindowThreadProcessId(Win32.GetForegroundWindow(), IntPtr.Zero));
            return GetInputLayouts().FirstOrDefault(x => x.Handle == currentLayoutHandle);
        }

        public static string GetNextInputLanguageName(string currentLanguageName)
        {
            var languageNames = GetInputLayouts().Select(x => x.LanguageName).Distinct().ToList();
            var indexOfNext = languageNames.IndexOf(currentLanguageName) + 1;
            if (indexOfNext >= languageNames.Count)
                indexOfNext = 0;
            return languageNames[indexOfNext];
        }

        public static IntPtr GetDefaultLayoutForLanguage(string languageName)
        {
            // Avoid re-evaluating properties
            var inputLanguages = GetInputLayouts();
            var firstLanguageLayout = inputLanguages.FirstOrDefault(x => x.LanguageName == languageName);
            if (firstLanguageLayout == null)
                firstLanguageLayout = inputLanguages.FirstOrDefault();
            if (firstLanguageLayout == null)
                throw new NullReferenceException("Not a single language's installed in the system");

            return firstLanguageLayout.Handle;
        }

        public static InputLayout GetLayoutByLanguageAndLayoutName(string languageName, string layoutName)
        {
            return GetInputLayouts().FirstOrDefault(x => x.LanguageName == languageName && x.Name == layoutName);
        }
    }
}