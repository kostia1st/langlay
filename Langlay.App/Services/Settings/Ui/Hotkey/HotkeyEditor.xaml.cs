using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for HotkeyEditor.xaml
    /// </summary>
    public partial class HotkeyEditor : UserControl
    {
        public event RoutedEventHandler RemoveClick;
        public event RoutedEventHandler Changed;

        public HotkeyEditor()
        {
            InitializeComponent();
            Loaded += HotkeyEditor_Loaded;
            Unloaded += HotkeyEditor_Unloaded;
        }

        private void HotkeyEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (KeyCodeViewModel) DataContext;
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void HotkeyEditor_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (KeyCodeViewModel) DataContext;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Changed?.Invoke(this, new RoutedEventArgs());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RemoveClick?.Invoke(this, e);
        }
    }
}