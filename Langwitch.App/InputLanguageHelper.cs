using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Langwitch
{
    public static class InputLanguageHelper
    {
        private static IEnumerable<InputLanguage> InputLanguages
        {
            get { return InputLanguage.InstalledInputLanguages.Cast<InputLanguage>(); }
        }

        public static InputLanguage GetInputLanguageByHandle(IntPtr handle)
        {
            return InputLanguages.FirstOrDefault(x=> x.Handle == handle);
        }

        public static InputLanguage GetGlobalCurrentInputLanguage()
        {
            var currentLayoutHandle = SafeMethods.GetKeyboardLayout(
               SafeMethods.GetWindowThreadProcessId(SafeMethods.GetForegroundWindow(), 0));
            return InputLanguages.FirstOrDefault(x => x.Handle == currentLayoutHandle);
        }

        public static IList<string> GetLanguageNamesOrdered()
        {
            return InputLanguages.Select(x => x.Culture.EnglishName).Distinct().ToList();
        }

        public static string GetNextInputLanguageName(string currentLanguageName)
        {
            var languageNames = GetLanguageNamesOrdered();
            var indexOfNext = languageNames.IndexOf(currentLanguageName) + 1;
            if (indexOfNext >= languageNames.Count)
                indexOfNext = 0;
            return languageNames[indexOfNext];
        }

        public static IntPtr GetDefaultLayoutForLanguage(string languageName)
        {
            var firstLanguageLayout = InputLanguages.FirstOrDefault(x => x.Culture.EnglishName == languageName);
            if (firstLanguageLayout == null)
                firstLanguageLayout = InputLanguages.First();
            if (firstLanguageLayout == null)
                throw new NullReferenceException("Not a single language's installed in the system");

            return firstLanguageLayout.Handle;
        }

        public static void SetCurrentLayout(IntPtr layoutHandle)
        {
            var foregroundWindowHandle = SafeMethods.GetForegroundWindow();
            SafeMethods.PostMessage(foregroundWindowHandle, SafeMethods.WM_INPUTLANGCHANGEREQUEST, 0, layoutHandle.ToInt32());
        }
    }
}
