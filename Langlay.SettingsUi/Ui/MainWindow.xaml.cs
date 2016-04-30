﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Product.Common;
using WinformsApp = System.Windows.Forms.Application;

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
            ViewModel = new ConfigViewModel(App.ConfigService);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            ViewModel.ShowSettingsOnce = false;
            DataContext = ViewModel;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tbrVersion.Text = string.Format("Version {0}", WinformsApp.ProductVersion);
            tbrLocation.Text = WinformsApp.StartupPath;
        }

        private void DoOnViewModelChanged()
        {
            // Save configuration
            App.ConfigService.SaveToFile();
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