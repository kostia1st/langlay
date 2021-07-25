using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Product.Common;

namespace Product.SettingsUi {
    public enum LanguageOrLayoutSelectorMode {
        LanguageAndLayout,
        LanguageOnly,
        LayoutOnly
    }

    static class LanguageOrLayoutHelper {
        static internal IList<LanguageOrLayoutViewModel> GetList(LanguageOrLayoutSelectorMode mode) {
            var languageService = ServiceRegistry.Instance?.Get<ILanguageService>();
            switch (mode) {
                case LanguageOrLayoutSelectorMode.LanguageAndLayout:
                    return languageService?
                        .GetInputLayouts()
                        .Aggregate(new List<LanguageOrLayoutViewModel>(), (List<LanguageOrLayoutViewModel> acc, InputLayout layout) => {
                            if (acc.FirstOrDefault(x => x.LanguageOrLayoutId == layout.LanguageId) == null) {
                                acc.Add(new LanguageOrLayoutViewModel((InputLanguage) layout));
                            }
                            acc.Add(new LanguageOrLayoutViewModel(layout));
                            return acc;
                        })
                        .ToList();
                case LanguageOrLayoutSelectorMode.LanguageOnly:
                    return languageService?
                        .GetInputLayouts()
                        .Aggregate(new List<LanguageOrLayoutViewModel>(), (List<LanguageOrLayoutViewModel> acc, InputLayout layout) => {
                            if (acc.FirstOrDefault(x => x.LanguageOrLayoutId == layout.LanguageId) == null) {
                                acc.Add(new LanguageOrLayoutViewModel((InputLanguage) layout));
                            }
                            return acc;
                        })
                        .ToList();
                case LanguageOrLayoutSelectorMode.LayoutOnly:
                    return languageService?
                        .GetInputLayouts()
                        .Select(x => new LanguageOrLayoutViewModel(x))
                        .ToList();
                default:
                    throw new Exception($"Unsupported mode: {mode}");
            }
        }
    }
}
