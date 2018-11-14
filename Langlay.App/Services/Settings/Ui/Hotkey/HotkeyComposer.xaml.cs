using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Product.Common;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for HotkeyComposer.xaml
    /// </summary>
    public partial class HotkeyComposer : UserControl
    {
        private IList<KeyCodeViewModel> KeyList => (IList<KeyCodeViewModel>) DataContext;
        public event RoutedEventHandler Changed;

        public HotkeyComposer()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            KeyList.Add(new KeyCodeViewModel { KeyCode = KeyCode.None });
        }

        private void HotkeyEditor_RemoveClick(object sender, RoutedEventArgs e)
        {
            var control = (HotkeyEditor) sender;
            KeyList.Remove((KeyCodeViewModel) control.DataContext);
        }

        private void HotkeyEditor_Changed(object sender, RoutedEventArgs e)
        {
            Changed?.Invoke(this, e);
        }
    }
}