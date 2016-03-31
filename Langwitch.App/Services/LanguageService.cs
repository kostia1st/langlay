using System;
using System.Collections.Generic;
using System.Threading;

namespace Product
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
                var currentLanguage = InputLayoutHelper.GetCurrentLayout();
                if (currentLanguage != null)
                {
                    // Here we save the layout last used within the language, so that it could be restored later.
                    CultureToLastUsedLayout[currentLanguage.LanguageName] = currentLanguage.Handle;

                    var nextLanguageName = InputLayoutHelper.GetNextInputLanguageName(
                        currentLanguage.LanguageName);
                    IntPtr layoutToSet;
                    if (restoreSavedLayout && CultureToLastUsedLayout.ContainsKey(nextLanguageName))
                        layoutToSet = CultureToLastUsedLayout[nextLanguageName];
                    else
                        layoutToSet = InputLayoutHelper.GetDefaultLayoutForLanguage(nextLanguageName);
                    LanguageSetterService.SetCurrentLayout(layoutToSet);
                    Thread.Sleep(10);
                    currentLanguage = InputLayoutHelper.GetCurrentLayout();
                    if (currentLanguage != null)
                    {
                        OverlayService.PushMessage(
                            currentLanguage.LanguageNameTwoLetter.CapitalizeFirst(),
                            currentLanguage.Name);
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
                var currentLayout = InputLayoutHelper.GetCurrentLayout();
                if (currentLayout != null)
                {
                    var nextLayoutName = InputLayoutHelper.GetNextInputLayoutName(
                        currentLayout.LanguageName, currentLayout.Name, doWrap);

                    if (!string.IsNullOrEmpty(nextLayoutName))
                    {
                        IntPtr layoutToSet = InputLayoutHelper.GetLayoutByLanguageAndLayoutName(
                            currentLayout.LanguageName, nextLayoutName).Handle;
                        LanguageSetterService.SetCurrentLayout(layoutToSet);

                        CultureToLastUsedLayout[currentLayout.LanguageName] = layoutToSet;
                        currentLayout = InputLayoutHelper.GetCurrentLayout();
                        if (currentLayout != null)
                        {
                            OverlayService.PushMessage(
                                currentLayout.LanguageNameTwoLetter.CapitalizeFirst(),
                                currentLayout.Name);
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
