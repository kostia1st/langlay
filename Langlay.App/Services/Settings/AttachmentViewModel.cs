using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Product.Common;

namespace Product.SettingsUi {

    public class AttachmentViewModel : INotifyPropertyChanged {

        private Regex acceptableMask = new Regex(@"[0-9+\-\(\)A-Za-z\s]");

        private string _appTitleMask;
        public string AppTitleMask {
            get => _appTitleMask;
            set {
                if (value != _appTitleMask) {
                    var matches = acceptableMask.Matches(value);
                    var cleanedValue = string.Join("", matches.Cast<Match>().Select(x => x.Value));
                    _appTitleMask = cleanedValue;
                    RaisePropertyChanged(x => x.AppTitleMask);
                }
            }
        }

        private int? _layoutId;
        public int? LayoutId {
            get => _layoutId;
            set {
                if (value != _layoutId) {
                    _layoutId = value;
                    RaisePropertyChanged(x => x.LayoutId);
                }
            }
        }

        private IList<LayoutViewModel> _layoutList;
        public IList<LayoutViewModel> LayoutList {
            get => _layoutList;
            set {
                if (value != _layoutList) {
                    _layoutList = value;
                    RaisePropertyChanged(x => x.LayoutList);
                }
            }
        }

        public AttachmentViewModel(string mask, int? layoutId) {
            _appTitleMask = mask;
            _layoutId = layoutId;
            var languageService = ServiceRegistry.Instance.Get<ILanguageService>();
            _layoutList = languageService
                .GetInputLayouts()
                .Select(x => new LayoutViewModel(x))
                .ToList();
        }

        private void RaisePropertyChanged<T1>(Expression<Func<AttachmentViewModel, T1>> expression) {
            RaisePropertyChanged(ExpressionUtils.GetMemberName(expression));
        }

        private void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
