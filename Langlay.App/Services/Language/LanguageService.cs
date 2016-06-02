using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class LanguageService : ILanguageService
    {
        private IDictionary<string, IntPtr> CultureToLastUsedLayout
            = new Dictionary<string, IntPtr>();

        private IOverlayService OverlayService { get; set; }
        private IConfigService ConfigService { get; set; }
        public ILanguageSetterService LanguageSetterService { get; set; }

        public LanguageService(
            IConfigService configService)
        {
            ConfigService = configService;
        }

        private void CheckServicesAreSet()
        {
            if (ConfigService == null)
                throw new NullReferenceException("ConfigService must not be null");
            if (LanguageSetterService == null)
                throw new NullReferenceException("LanguageSetterService");
        }

        private IDictionary<IntPtr, InputLayout> _inputLayouts
            = new Dictionary<IntPtr, InputLayout>();

        public IList<InputLayout> GetInputLayouts()
        {
            return InputLanguage.InstalledInputLanguages
              .Cast<InputLanguage>()
              .Select(x =>
              {
                  if (!_inputLayouts.ContainsKey(x.Handle))
                  {
                      // Surprisingly, this is quite expensive. According to
                      // ILSpy there're lots of unsafe method and registry
                      // usage each time you read the property and sub-properties.
                      _inputLayouts[x.Handle] = new InputLayout(x);
                  }
                  return _inputLayouts[x.Handle];
              })
              .ToList();
        }

        private IList<InputLayout> GetLayoutsByLanguage(
            string languageName, IList<InputLayout> inputLayouts = null)
        {
            if (inputLayouts == null)
                inputLayouts = GetInputLayouts();
            return inputLayouts.Where(x => x.LanguageName == languageName).ToList();
        }

        private string GetNextInputLayoutName(
            string currentLanguageName, string currentLayoutName, bool doWrap,
            IList<InputLayout> inputLayouts = null)
        {
            var layoutNames = GetLayoutsByLanguage(currentLanguageName, inputLayouts)
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

        /// <summary>
        /// Returns the currently active input layout.
        /// </summary>
        /// <returns>
        /// returning null means we cannot determine the current input
        /// layout from the OS.
        /// </returns>
        public InputLayout GetCurrentLayout()
        {
            var currentLayoutHandle = GetCurrentLayoutHandle();

            var currentLayout = GetInputLayouts()
                .FirstOrDefault(x => x.Handle == currentLayoutHandle);
            return currentLayout;

            // There are (for now) cases when we cannot determine the
            // currently used layout (in a command line prompt for example,
            // or in a window with a higher level of elevation, or in the
            // taskbar) so we need to handle this possible situation without
            // a crash.
        }

#if TRACE
        private uint _processId_old;
#endif

        public IntPtr GetCurrentLayoutHandle()
        {
            var currentLayoutHandle = IntPtr.Zero;
            var processId = 0U;
            var threadId = Win32.GetWindowThreadProcessId(
                Win32.GetForegroundWindow(), out processId);

            if (processId != 0)
            {
                var process = Process.GetProcessById((int) processId);
                if (process != null && process.ProcessName != "Idle")
                {
#if TRACE
                    if (processId != _processId_old)
                        Trace.WriteLine($"Process name: {process.ProcessName}");
                    _processId_old = processId;
#endif
                    currentLayoutHandle = Win32.GetKeyboardLayout(threadId);
                }
            }
            return currentLayoutHandle;
        }

        private string GetNextInputLanguageName(
            string currentLanguageName)
        {
            var inputLayouts = GetInputLayouts();
            var languageNames = inputLayouts.Select(x => x.LanguageName).Distinct().ToList();
            var indexOfNext = languageNames.IndexOf(currentLanguageName) + 1;
            if (indexOfNext >= languageNames.Count)
                indexOfNext = 0;
            return languageNames[indexOfNext];
        }

        private IntPtr GetDefaultLayoutForLanguage(
            string languageName)
        {
            // Avoid re-evaluating properties
            var inputLayouts = GetInputLayouts();
            var firstLanguageLayout = inputLayouts.FirstOrDefault(x => x.LanguageName == languageName);
            if (firstLanguageLayout == null)
                firstLanguageLayout = inputLayouts.FirstOrDefault();
            if (firstLanguageLayout == null)
                throw new NullReferenceException("Not a single language/layout installed in the system");

            return firstLanguageLayout.Handle;
        }

        private InputLayout GetLayoutByLanguageAndLayoutName(
            string languageName, string layoutName)
        {
            var inputLayouts = GetInputLayouts();
            return inputLayouts.FirstOrDefault(x => x.LanguageName == languageName && x.Name == layoutName);
        }

        protected void SwitchLanguage(bool restoreSavedLayout)
        {
            CheckServicesAreSet();

            var currentLayout = GetCurrentLayout();
            if (currentLayout != null)
            {
                var inputLayouts = GetInputLayouts();

                // Here we save the layout last used within the language, so
                // that it could be restored later.
                CultureToLastUsedLayout[currentLayout.LanguageName] = currentLayout.Handle;

                var nextLanguageName = GetNextInputLanguageName(
                    currentLayout.LanguageName);
                IntPtr layoutToSet;
                if (restoreSavedLayout && CultureToLastUsedLayout.ContainsKey(nextLanguageName))
                    layoutToSet = CultureToLastUsedLayout[nextLanguageName];
                else
                    layoutToSet = GetDefaultLayoutForLanguage(
                        nextLanguageName);
                LanguageSetterService.SetCurrentLayout(layoutToSet);
            }
        }

        protected bool SwitchLayout(bool doWrap)
        {
            CheckServicesAreSet();
            var result = false;
            var currentLayout = GetCurrentLayout();
            if (currentLayout != null)
            {
                var inputLayouts = GetInputLayouts();
                var nextLayoutName = GetNextInputLayoutName(
                    currentLayout.LanguageName, currentLayout.Name, doWrap);

                if (!string.IsNullOrEmpty(nextLayoutName))
                {
                    var layoutToSet = GetLayoutByLanguageAndLayoutName(
                        currentLayout.LanguageName, nextLayoutName).Handle;
                    LanguageSetterService.SetCurrentLayout(layoutToSet);

                    CultureToLastUsedLayout[currentLayout.LanguageName] = layoutToSet;

                    result = true;
                }
            }
            return result;
        }

        protected void SwitchLanguageAndLayout()
        {
            if (!SwitchLayout(false))
                SwitchLanguage(false);
        }

        public void ConductSwitch(KeyboardSwitch keyboardSwitch)
        {
            switch (keyboardSwitch)
            {
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