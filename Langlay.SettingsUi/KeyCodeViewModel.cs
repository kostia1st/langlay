using System.ComponentModel;
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
                    NotifyPropertyChanged("KeyCode");
                }
            }
        }
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
