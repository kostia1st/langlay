using System;
using System.ComponentModel;
using System.Windows;
using Product.Common;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConfigViewModel ViewModel
        {
            get => (ConfigViewModel) DataContext;
            set => DataContext = value;
        }

        public Action OnSave { get; set; }
        public Action OnApply { get; set; }

        public MainWindow(IConfigService configService)
        {
            ViewModel = new ConfigViewModel(configService);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            ViewModel.ShowSettingsOnce = false;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            OnSave = null;
            OnApply = null;
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            ViewModel = null;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tbrVersion.Text = $"Version {AppSpecific.AppVersion}";
            tbrLocation.Text = PathUtils.GetAppDirectory();
            spMain.ViewModel = ViewModel;
        }

        private void RaiseSaveConfig()
        {
            OnSave?.Invoke();
        }

        private void RaiseApplyConfig()
        {
            OnApply?.Invoke();
        }

        private void DoOnViewModelChanged()
        {
            RaiseSaveConfig();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DoOnViewModelChanged();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            RaiseApplyConfig();
            base.OnClosed(e);
        }

    }
}