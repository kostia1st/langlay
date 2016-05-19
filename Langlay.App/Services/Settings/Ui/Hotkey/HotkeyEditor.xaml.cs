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
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var viewModel = (KeyCodeViewModel) DataContext;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Changed != null)
                Changed(this, new RoutedEventArgs());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveClick != null)
                RemoveClick(this, e);
        }
    }
}