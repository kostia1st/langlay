using System;
using System.Collections.Generic;
using System.Threading;

namespace Langwitch
{
    public class LanguageService : ILanguageService
    {
        private IDictionary<string, IntPtr> CultureToLastUsedLayout
            = new Dictionary<string, IntPtr>();
        private IOverlayService OverlayService { get; set; }
        private IConfigService ConfigService { get; set; }
        private ILanguageSetterService LanguageSetterService { get; set; }

        public LanguageService(
            IConfigService configService, IOverlayService overlayService,
            ILanguageSetterService languageSetterService)
        {
            if (configService == null)
                throw new ArgumentNullException("configService");
            if (overlayService == null)
                throw new ArgumentNullException("overlayService");
            if (languageSetterService == null)
                throw new ArgumentNullException("languageSetterService");
            ConfigService = configService;
            OverlayService = overlayService;
            LanguageSetterService = languageSetterService;
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
                    LanguageSetterService.SetCurrentLayout(layoutToSet);
                    Thread.Sleep(10);
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
                        LanguageSetterService.SetCurrentLayout(layoutToSet);

                        CultureToLastUsedLayout[currentLayout.Culture.EnglishName] = layoutToSet;
                        Thread.Sleep(10);
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
    }
}
