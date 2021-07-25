using System;
using System.ComponentModel;
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

        public LayoutColorSetViewModel(string layoutId, Color backgroundColor, Color foregroundColor) {
            _layoutId = layoutId;
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
