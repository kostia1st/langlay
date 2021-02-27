using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Media;
using Product.Common;

namespace Product.SettingsUi {
    public class LayoutColorSetViewModel : INotifyPropertyChanged {
        private Color _backgroundColor;
        public Color BackgroundColor {
            get => _backgroundColor;
            set {
                if (value != _backgroundColor) {
                    _backgroundColor = value;
                    RaisePropertyChanged(x => x.BackgroundColor);
                }
            }
        }

        private Color _foregroundColor;
        public Color ForegroundColor {
            get => _foregroundColor;
            set {
                if (value != _foregroundColor) {
                    _foregroundColor = value;
                    RaisePropertyChanged(x => x.ForegroundColor);
                }
            }
        }

        private string _layoutId;
        public string LayoutId {
            get => _layoutId;
            set {
                if (value != _layoutId) {
                    _layoutId = value;
                    RaisePropertyChanged(x => x.LayoutId);
                }
            }
        }

        private IList<LanguageOrLayoutViewModel> _layoutList;
        public IList<LanguageOrLayoutViewModel> LayoutList {
            get => _layoutList;
            set {
                if (value != _layoutList) {
                    _layoutList = value;
                    RaisePropertyChanged(x => x.LayoutList);
                }
            }
        }

        public LayoutColorSetViewModel(string layoutId, Color backgroundColor, Color foregroundColor) {
            _layoutId = layoutId;
            var languageService = ServiceRegistry.Instance.Get<ILanguageService>();
            _layoutList = languageService
                .GetInputLayouts()
                .Select(x => new LanguageOrLayoutViewModel(x))
                .ToList();

            _backgroundColor = backgroundColor;
            _foregroundColor = foregroundColor;
        }

        private void RaisePropertyChanged<T1>(Expression<Func<LayoutColorSetViewModel, T1>> expression) {
            RaisePropertyChanged(ExpressionUtils.GetMemberName(expression));
        }

        private void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
