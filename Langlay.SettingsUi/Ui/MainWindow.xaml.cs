using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Product.Common;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConfigViewModel ViewModel { get; set; }

        public MainWindow()
        {
            ViewModel = new ConfigViewModel(App.ConfigService);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            ViewModel.ShowSettingsOnce = false;
            DataContext = ViewModel;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tbrVersion.Text = string.Format("Version {0}", AppSpecific.AppVersion);
            tbrLocation.Text = PathUtils.GetAppDirectory();
            UpdateHotkeyAnalysis();
        }

        private void DoOnViewModelChanged()
        {
            // Save configuration
            App.ConfigService.SaveToFile();
            UpdateHotkeyAnalysis();
        }

        private void UpdateHotkeyAnalysis()
        {
            tbkFeedbackLanguage.Text = GetAnalysisByHotkey(App.ConfigService.LanguageSwitchKeyArray);
            tbkFeedbackLayout.Text = GetAnalysisByHotkey(App.ConfigService.LayoutSwitchKeyArray);
        }

        private string GetAnalysisByHotkey(IList<KeyCode> keys)
        {
            var result = new StringBuilder();
            if (keys.Count == 1)
            {
                if (keys[0] == KeyCode.CapsLock)
                    result.AppendLine("Note, to press the *actual* Caps Lock you can use Shift + Caps Lock.");
            }
            else if (keys.Count > 1)
            {
                if (keys.Contains(KeyCode.FnKey))
                    result.AppendLine("The Fn key will work out only if pressed last in the sequence.");
                if (keys.Contains(KeyCode.LWin) || keys.Contains(KeyCode.RWin))
                    result.AppendLine("The use of the Win key in a combination could probably lead to issues"
                        + " if Win is applied not the last in the sequence.");
            }
            return result.ToString().TrimEnd('\r', '\n');
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
                    Win32.PostThreadMessage(thread.Id, Win32.WM_USER_RESTART, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    process.CloseMainWindow();
                }
                if (!process.WaitForExit(1000))
                {
                    process.Kill();

                    var location = PathUtils.GetAppDirectory();
                    var fullFilename =
                        System.IO.Path.Combine(location, AppSpecific.MainAppFilename);
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
