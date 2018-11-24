using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Product.Common;

namespace Product.SettingsUi
{
    public class KeyCodeViewModel : INotifyPropertyChanged
    {
        private KeyCode _keyCode;

        public KeyCode KeyCode
        {
            get => _keyCode;
            set
            {
                if (_keyCode != value)
                {
                    _keyCode = value;
                    RaisePropertyChanged(x => x.KeyCode);
                }
            }
        }

        private void RaisePropertyChanged<T1>(Expression<Func<KeyCodeViewModel, T1>> expression)
        {
            RaisePropertyChanged(ExpressionUtils.GetMemberName(expression));
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}