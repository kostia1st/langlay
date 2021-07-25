using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Product.Common;

namespace Product {
    public class LanguageService : ILanguageService {
        private IDictionary<string, IntPtr> LanguageToLastUsedLayout
            = new Dictionary<string, IntPtr>();

        private IDictionary<IntPtr, InputLayout> _inputLayouts
            = new Dictionary<IntPtr, InputLayout>();

        public IList<InputLayout> GetInputLayouts() {
            return System.Windows.Forms.InputLanguage.InstalledInputLanguages
              .Cast<System.Windows.Forms.InputLanguage>()
              .Select(x => {
                  if (!_inputLayouts.ContainsKey(x.Handle)) {
                      // Surprisingly, this is quite expensive. According to
                      // ILSpy there're lots of unsafe method and registry
                      // usage each time you read the property and sub-properties.
                      _inputLayouts[x.Handle] = new InputLayout(x);
                      Debug.WriteLine($"System layout found: {x.LayoutName}, {x.Culture.Name}, {x.Culture.EnglishName}");
                  }
                  return _inputLayouts[x.Handle];
              })
              .ToList();
        }

        private IList<InputLayout> GetLayoutsByLanguage(string languageName, IList<InputLayout> inputLayouts = null) {
            if (inputLayouts == null)
                inputLayouts = GetInputLayouts();
            return inputLayouts.Where(x => x.LanguageName == languageName).ToList();
        }

        private string GetNextInputLayoutName(string currentLanguageName, string currentLayoutName, bool doWrap,
            IList<InputLayout> inputLayouts = null) {
            var layoutNames = GetLayoutsByLanguage(currentLanguageName, inputLayouts)
                .Select(x => x.Name)
                .ToList();
            var indexOfNext = layoutNames.IndexOf(currentLayoutName) + 1;
            if (indexOfNext >= layoutNames.Count) {
                if (doWrap)
                    return layoutNames[0];
                else
                    return null;
            }
            return layoutNames[indexOfNext];
        }

        private InputLayout GetLayoutByPtr(IntPtr layoutHandle) {
            return GetInputLayouts()
                .FirstOrDefault(x => x.Handle == layoutHandle);
        }

        /// <summary>
        /// Returns the currently active input layout.
        /// </summary>
        /// <returns>
        /// returning null means we cannot determine the current input
        /// layout from the OS.
        /// </returns>
        public InputLayout GetCurrentLayout() {
            return GetLayoutByPtr(GetCurrentLayoutHandle());

            // There are (for now) cases when we cannot determine the
            // currently used layout (in a command line prompt for example,
            // or in a window with a higher level of elevation, or in the
            // "idle" process) so we need to handle this possible situation
            // without a crash.
        }

        public IntPtr GetCurrentLayoutHandle() {
            var currentLayoutHandle = IntPtr.Zero;
            var threadAndProcessInfo = ProcessUtils.GetThreadAndProcessInfoByWindowHandle(
                Win32.GetForegroundWindow());

            if (threadAndProcessInfo != null) {
                currentLayoutHandle = Win32.GetKeyboardLayout(threadAndProcessInfo.ThreadId);
            }
            return currentLayoutHandle;
        }

        private string GetNextInputLanguageName(string currentLanguageName) {
            var inputLayouts = GetInputLayouts();
            var languageNames = inputLayouts.Select(x => x.LanguageName).Distinct().ToList();
            var indexOfNext = languageNames.IndexOf(currentLanguageName) + 1;
            if (indexOfNext >= languageNames.Count)
                indexOfNext = 0;
            return languageNames[indexOfNext];
        }

        private InputLayout GetDefaultLayoutForLanguage(string languageName) {
            // Avoid re-evaluating properties
            var inputLayouts = GetInputLayouts();
            var firstLanguageLayout = inputLayouts.FirstOrDefault(x => x.LanguageName == languageName);
            if (firstLanguageLayout == null)
                firstLanguageLayout = inputLayouts.FirstOrDefault();
            if (firstLanguageLayout == null)
                throw new NullReferenceException("Not a single language/layout installed in the system");

            return firstLanguageLayout;
        }

        private InputLayout GetLayoutByLanguageAndLayoutName(string languageName, string layoutName) {
            var inputLayouts = GetInputLayouts();
            return inputLayouts.FirstOrDefault(x => x.LanguageName == languageName && x.Name == layoutName);
        }

        public void SetCurrentLayout(InputLayout layoutToSet) {
            var formerLayout = GetCurrentLayout();
            if (formerLayout != null) {
                // Here we save the layout last used within the language, so
                // that it could be restored later.
                LanguageToLastUsedLayout[formerLayout.LanguageName] = formerLayout.Handle;
            }
            if (layoutToSet != null) {
                var languageSetterService = ServiceRegistry.Instance.Get<ILanguageSetterService>();
                languageSetterService.SetCurrentLayout(layoutToSet.Handle);
                // Here we save the layout last used within the language, so
                // that it could be restored later.
                LanguageToLastUsedLayout[layoutToSet.LanguageName] = layoutToSet.Handle;
            }
        }

        public void SetCurrentLanguage(string languageName, bool restoreLastUsedLayout) {
            InputLayout layoutToSet;
            if (restoreLastUsedLayout && LanguageToLastUsedLayout.ContainsKey(languageName))
                layoutToSet = GetLayoutByPtr(LanguageToLastUsedLayout[languageName]);
            else
                layoutToSet = GetDefaultLayoutForLanguage(languageName);
            SetCurrentLayout(layoutToSet);

        }

        protected bool SwitchLanguage(bool restoreLastUsedLayout) {
            var currentLayout = GetCurrentLayout();
            if (currentLayout != null) {
                var nextLanguageName = GetNextInputLanguageName(
                    currentLayout.LanguageName);
                InputLayout layoutToSet;
                if (restoreLastUsedLayout && LanguageToLastUsedLayout.ContainsKey(nextLanguageName))
                    layoutToSet = GetLayoutByPtr(LanguageToLastUsedLayout[nextLanguageName]);
                else
                    layoutToSet = GetDefaultLayoutForLanguage(nextLanguageName);
                SetCurrentLayout(layoutToSet);
                return true;
            }
            return false;
        }

        protected bool SwitchLayout(bool doWrap) {
            var currentLayout = GetCurrentLayout();
            if (currentLayout != null) {
                var nextLayoutName = GetNextInputLayoutName(
                    currentLayout.LanguageName, currentLayout.Name, doWrap);

                if (!string.IsNullOrEmpty(nextLayoutName)) {
                    var layoutToSet = GetLayoutByLanguageAndLayoutName(
                        currentLayout.LanguageName, nextLayoutName);
                    SetCurrentLayout(layoutToSet);
                    return true;
                }
            }
            return false;
        }

        protected void SwitchLanguageAndLayout() {
            // If we have no layout to switch to, then we switch the language.
            if (!SwitchLayout(false))
                SwitchLanguage(false);
        }

        public void ConductSwitch(KeyboardSwitch keyboardSwitch) {
            switch (keyboardSwitch) {
                case KeyboardSwitch.Language:
                    SwitchLanguage(false);
                    break;

                case KeyboardSwitch.LanguageRestoreLayout:
                    SwitchLanguage(true);
                    break;

                case KeyboardSwitch.LayoutNoWrap:
                    SwitchLayout(false);
                    break;

                case KeyboardSwitch.Layout:
                    SwitchLayout(true);
                    break;

                case KeyboardSwitch.LanguageAndLayout:
                    SwitchLanguageAndLayout();
                    break;
            }
        }
    }
}