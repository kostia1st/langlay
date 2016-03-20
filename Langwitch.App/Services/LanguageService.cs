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

        public bool SwitchLanguage()
        {
            try
            {
                var currentLanguage = InputLanguageHelper.GetGlobalCurrentInputLanguage();
                if (currentLanguage != null)
                {
                    CultureToLastUsedLayout[currentLanguage.Culture.EnglishName] = currentLanguage.Handle;
                    var nextLanguageName = InputLanguageHelper.GetNextInputLanguageName(currentLanguage.Culture.EnglishName);
                    IntPtr layoutToSet;
                    if (CultureToLastUsedLayout.ContainsKey(nextLanguageName))
                        layoutToSet = CultureToLastUsedLayout[nextLanguageName];
                    else
                        layoutToSet = InputLanguageHelper.GetDefaultLayoutForLanguage(nextLanguageName);
                    InputLanguageHelper.SetCurrentLayout(layoutToSet);

                    currentLanguage = InputLanguageHelper.GetGlobalCurrentInputLanguage();
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
    }
}
