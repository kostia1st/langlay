using System;
using System.Collections.Generic;

namespace Langwitch
{
    public class LanguageService : ILanguageService
    {
        private IDictionary<string, IntPtr> CultureToLastUsedLayout
            = new Dictionary<string, IntPtr>();
        private IOverlayService OverlayService { get; set; }
        private IConfigService ConfigService { get; set; }

        public LanguageService(IConfigService configService, IOverlayService overlayService)
        {
            if (configService == null)
                throw new ArgumentNullException("configService");
            if (overlayService == null)
                throw new ArgumentNullException("overlayService");
            ConfigService = configService;
            OverlayService = overlayService;
        }

        public bool SwitchLanguage(bool restoreSavedLayout)
        {
            try
            {
                var currentLanguage = InputLanguageHelper.GetCurrentInputLanguage();
                if (currentLanguage != null)
                {
                    // Here we save the layout last used within the language, so that it could be restored later.
                    CultureToLastUsedLayout[currentLanguage.Culture.EnglishName] = currentLanguage.Handle;

                    var nextLanguageName = InputLanguageHelper.GetNextInputLanguageName(currentLanguage.Culture.EnglishName);
                    IntPtr layoutToSet;
                    if (restoreSavedLayout && CultureToLastUsedLayout.ContainsKey(nextLanguageName))
                        layoutToSet = CultureToLastUsedLayout[nextLanguageName];
                    else
                        layoutToSet = InputLanguageHelper.GetDefaultLayoutForLanguage(nextLanguageName);
                    SetCurrentLayout(layoutToSet);

                    currentLanguage = InputLanguageHelper.GetCurrentInputLanguage();
                    if (currentLanguage != null)
                    {
                        OverlayService.PushMessage(
                            currentLanguage.Culture.TwoLetterISOLanguageName.CapitalizeFirst(),
                            currentLanguage.LayoutName);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return false;
        }

        public bool SwitchLayout(bool doWrap)
        {
            try
            {
                var currentLayout = InputLanguageHelper.GetCurrentInputLanguage();
                if (currentLayout != null)
                {
                    var nextLayoutName = InputLanguageHelper.GetNextInputLayoutName(currentLayout.Culture.EnglishName, currentLayout.LayoutName, doWrap);

                    if (!string.IsNullOrEmpty(nextLayoutName))
                    {
                        IntPtr layoutToSet = InputLanguageHelper.GetLayoutByLanguageAndLayoutName(currentLayout.Culture.EnglishName, nextLayoutName).Handle;
                        SetCurrentLayout(layoutToSet);

                        CultureToLastUsedLayout[currentLayout.Culture.EnglishName] = layoutToSet;
                        currentLayout = InputLanguageHelper.GetCurrentInputLanguage();
                        if (currentLayout != null)
                        {
                            OverlayService.PushMessage(
                                currentLayout.Culture.TwoLetterISOLanguageName.CapitalizeFirst(),
                                currentLayout.LayoutName);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return false;
        }

        public bool SwitchLanguageAndLayout()
        {
            return SwitchLayout(false) || SwitchLanguage(false);

        }

        private void SetCurrentLayout(IntPtr layoutHandle)
        {
            var foregroundWindowHandle = SafeMethods.GetForegroundWindow();

            // TODO: this does not work with Skype, for some reason.
            IntPtr result;
            SafeMethods.SendMessageTimeout(
                foregroundWindowHandle, SafeMethods.WM_INPUTLANGCHANGEREQUEST, 0, layoutHandle.ToInt32(),
                SafeMethods.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 500, out result);
        }

    }
}
