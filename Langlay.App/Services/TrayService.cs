using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Product.Common;

namespace Product {
    public class TrayService : ITrayService, ILifecycled {
        private ContextMenu ContextMenu { get; set; }
        private NotifyIcon Icon { get; set; }

        private void OpenHomepage() {
            var psi = new ProcessStartInfo() {
                UseShellExecute = true,
                FileName = "https://github.com/estorski/langlay",
            };
            Process.Start(psi);
        }

        private void OpenIssues() {
            var psi = new ProcessStartInfo() {
                UseShellExecute = true,
                FileName = "https://github.com/estorski/langlay/issues",
            };
            Process.Start(psi);
        }

        private void RestartAsAdmin() {
            ProcessUtils.StartMainApp($"--{ArgumentNames.ForceThisInstance}:true", true);
        }

        private void ExitApplication() {
            var appRunnerService = ServiceRegistry.Instance.Get<IAppRunnerService>();
            appRunnerService?.ExitApplication();
        }

        #region Start/Stop
        public bool IsStarted { get; private set; }

        public void Start() {
            if (!IsStarted) {
                IsStarted = true;
                void actionOpenSettings() {
                    var settingsService = ServiceRegistry.Instance.Get<ISettingsService>();
                    settingsService.ShowSettings();
                }

                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Settings", delegate { actionOpenSettings(); }) { DefaultItem = true },
                    new MenuItem("-"),
                    new MenuItem("Report a bug", delegate { OpenIssues(); }),
                    new MenuItem("Visit homepage", delegate { OpenHomepage(); }),
                    new MenuItem("-"),
                    new MenuItem("Restart as admin", delegate { RestartAsAdmin(); }),
                    new MenuItem("Quit", delegate { ExitApplication(); })
                });
                Icon = new NotifyIcon() {
                    Text = Application.ProductName,
                    Icon = new Icon(typeof(Program), "Keyboard-Filled-2-16.ico"),
                    Visible = true,
                    ContextMenu = ContextMenu,
                };
                Icon.MouseDoubleClick += delegate { actionOpenSettings(); };
            }
        }

        public void Stop() {
            if (IsStarted) {
                IsStarted = false;
                if (ContextMenu != null) {
                    ContextMenu.Dispose();
                    ContextMenu = null;
                }
                if (Icon != null) {
                    Icon.Dispose();
                    Icon = null;
                }
            }
        }
        #endregion Start/Stop
    }
}