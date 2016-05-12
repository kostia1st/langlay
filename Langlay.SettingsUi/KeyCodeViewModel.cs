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
            get { return _keyCode; }
            set
            {
                if (_keyCode != value)
                {
                    _keyCode = value;
                    NotifyPropertyChanged(x => x.KeyCode);
                }
            }
        }

        private void NotifyPropertyChanged<T1>(Expression<Func<KeyCodeViewModel, T1>> expression)
        {
            NotifyPropertyChanged(ExpressionUtils.GetMemberName(expression));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}