using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using Product.Common;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConfigService ConfigService { get; set; }
        private ConfigViewModel ViewModel { get; set; }

        public MainWindow()
        {
            ConfigService = new ConfigService();
            ConfigService.ReadFromConfigFile();

            ViewModel = new ConfigViewModel(ConfigService);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            ViewModel.ShowSettingsOnce = false;
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void DoOnViewModelChanged()
        {
            // Save configuration
            ConfigService.SaveToFile();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DoOnViewModelChanged();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Close the running product, and restart it.
            var process = Process.GetProcessesByName(AppSpecific.MainAppProcessName).FirstOrDefault();
#if DEBUG
            if (process == null)
                process = Process.GetProcessesByName(AppSpecific.MainAppProcessName + ".vshost").FirstOrDefault();
#endif

            if (process != null)
            {
                var thread = process.Threads.Cast<ProcessThread>().FirstOrDefault();
                if (thread != null)
                {
                    Win32.PostThreadMessage(thread.Id, Win32.WM_USER_RESTART, 0, 0);
                }
                else
                {
                    process.CloseMainWindow();
                }
                if (!process.WaitForExit(1000))
                {
                    process.Kill();

                    var location = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    var fullFilename =
                        System.IO.Path.Combine(location, AppSpecific.MainAppPath);
                    var psi = new ProcessStartInfo
                    {
                        FileName = fullFilename,
                        WorkingDirectory = location,
                        UseShellExecute = false,
                        LoadUserProfile = true
                    };
                    Process.Start(psi);
                }
            }
            this.Close();
        }

        private void HotkeyComposer_Layout_Changed(object sender, RoutedEventArgs e)
        {
            ViewModel.NotifyLayoutSequenceChanged();
        }

        private void HotkeyComposer_Language_Changed(object sender, RoutedEventArgs e)
        {
            ViewModel.NotifyLanguageSequenceChanged();
        }
    }
}
