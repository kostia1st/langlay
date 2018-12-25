using System.Windows.Forms.Integration;
using Product.SettingsUi;

namespace Product {
    public class SettingsService : ISettingsService, ILifecycled {
        private MainWindow _mainWindow;

        #region Start/Stop

        public bool IsStarted { get; private set; }

        public void Start() {
            if (!IsStarted)
                IsStarted = true;
        }

        public void Stop() {
            if (IsStarted) {
                IsStarted = false;
                if (_mainWindow != null) {
                    if (_mainWindow.IsVisible)
                        _mainWindow.Close();
                    _mainWindow = null;
                }
            }
        }

        #endregion Start/Stop

        public void ShowSettings() {
            if (_mainWindow != null && _mainWindow.IsVisible) {
                _mainWindow.Activate();
            } else {
                var configService = ServiceRegistry.Instance.Get<IConfigService>();
                _mainWindow = new MainWindow(configService) {
                    OnSave = delegate {
                        configService.SaveToFile();
                    },
                    OnApply = delegate {
                        // We need to make sure the settings window is not going to show up
                        // multiple times in a row.
                        configService.DoShowSettingsOnce = false;
                        configService.SaveToFile();

                        var appRunnerService = ServiceRegistry.Instance.Get<IAppRunnerService>();
                        if (!appRunnerService.IsExiting)
                            appRunnerService.ReReadAndRunTheConfig();
                    },
                    ShowActivated = true
                };
                ElementHost.EnableModelessKeyboardInterop(_mainWindow);
                _mainWindow.Closed += _mainWindow_Closed;
                _mainWindow.Show();
                _mainWindow.Activate();
            }
        }

        private void _mainWindow_Closed(object sender, System.EventArgs e) {
            _mainWindow.OnApply = null;
            _mainWindow.OnSave = null;
            _mainWindow.Closed -= _mainWindow_Closed;
            _mainWindow = null;
        }
    }
}