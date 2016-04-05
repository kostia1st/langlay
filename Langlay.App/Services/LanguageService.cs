using System;
using System.Collections.Generic;
using System.Threading;
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
            IConfigService configService, IOverlayService overlayService)
        {
            ConfigService = configService;
            OverlayService = overlayService;
        }

        private void CheckServicesAreSet()
        {
            if (ConfigService == null)
                throw new NullReferenceException("ConfigService must not be null");
            if (OverlayService == null)
                throw new NullReferenceException("OverlayService");
            if (LanguageSetterService == null)
                throw new NullReferenceException("LanguageSetterService");
        }

        public bool SwitchLanguage(bool restoreSavedLayout)
        {
            try
            {
                CheckServicesAreSet();

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
                CheckServicesAreSet();
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
