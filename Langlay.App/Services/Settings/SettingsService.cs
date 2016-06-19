using Product.SettingsUi;

namespace Product
{
    public class SettingsService : ISettingsService
    {
        private IConfigService ConfigService { get; set; }
        private IAppRunnerService AppRunnerService { get; set; }
        private MainWindow _mainWindow;

        public SettingsService(IConfigService configService, IAppRunnerService appRunnerService)
        {
            ConfigService = configService;
            AppRunnerService = appRunnerService;
        }

        #region Start/Stop

        private bool IsStarted { get; set; }

        public void Start()
        {
            if (!IsStarted)
                IsStarted = true;
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                if (_mainWindow != null)
                {
                    if (_mainWindow.IsVisible)
                        _mainWindow.Close();
                    _mainWindow = null;
                }
            }
        }

        #endregion Start/Stop

        public void ShowSettings()
        {
            if (_mainWindow != null && _mainWindow.IsVisible)
            {
                _mainWindow.Activate();
            }
            else
            {
                _mainWindow = new MainWindow(ConfigService);
                _mainWindow.HandleSave = delegate
                {
                    ConfigService.SaveToFile();
                };
                _mainWindow.HandleApply = delegate
                {
                    if (!AppRunnerService.IsExiting)
                        AppRunnerService.ReReadAndRunTheConfig();
                };
                _mainWindow.Closed += _mainWindow_Closed;
                _mainWindow.ShowActivated = true;
                _mainWindow.Show();
                _mainWindow.Activate();
            }
        }

        private void _mainWindow_Closed(object sender, System.EventArgs e)
        {
            _mainWindow.HandleApply = null;
            _mainWindow.HandleSave = null;
            _mainWindow.Closed -= _mainWindow_Closed;
            _mainWindow = null;
        }
    }
}