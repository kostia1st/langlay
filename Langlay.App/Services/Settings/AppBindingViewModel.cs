using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Product.Common;

namespace Product.SettingsUi {
    public class AppBindingViewModel : INotifyPropertyChanged {

        private Regex acceptableMask = new Regex(@"[0-9+\-\(\)\\\/A-Za-z\s]");

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

        private string _languageOrLayoutId;
        public string LanguageOrLayoutId {
            get => _languageOrLayoutId;
            set {
                if (value != _languageOrLayoutId) {
                    _languageOrLayoutId = value;
                    RaisePropertyChanged(x => x.LanguageOrLayoutId);
                }
            }
        }

        public AppBindingViewModel(string mask, string languageOrLayoutId) {
            _appTitleMask = mask;
            _languageOrLayoutId = languageOrLayoutId;
        }

        private void RaisePropertyChanged<T1>(Expression<Func<AppBindingViewModel, T1>> expression) {
            RaisePropertyChanged(ExpressionUtils.GetMemberName(expression));
        }

        private void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
