using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class TrayService
    {
        private IConfigService ConfigService { get; set; }
        private ISettingsService SettingsService { get; set; }
        private ContextMenu ContextMenu { get; set; }
        private NotifyIcon Icon { get; set; }
        private bool IsStarted { get; set; }

        public TrayService(IConfigService configService, ISettingsService settingsService)
        {
            ConfigService = configService;
            SettingsService = settingsService;
        }

        private void OpenHomepage()
        {
            var psi = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = "https://github.com/estorski/langlay",
            };
            Process.Start(psi);
        }

        private void OpenIssues()
        {
            var psi = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = "https://github.com/estorski/langlay/issues",
            };
            Process.Start(psi);
        }

        private void RestartAsAdmin()
        {
            ProcessUtils.StartMainApp($"--{ArgumentNames.ForceThisInstance}:true", true);
        }

        private void ExitApplication()
        {
            Application.Exit();
        }

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                var actionOpenSettings = (Action) delegate { SettingsService.ShowSettings(); };

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
                Icon = new NotifyIcon()
                {
                    Text = Application.ProductName,
                    Icon = new Icon(typeof(Program), "Keyboard-Filled-2-16.ico"),
                    Visible = true,
                    ContextMenu = ContextMenu,
                };
                Icon.MouseDoubleClick += delegate { actionOpenSettings(); };
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                if (ContextMenu != null)
                {
                    ContextMenu.Dispose();
                    ContextMenu = null;
                }
                if (Icon != null)
                {
                    Icon.Dispose();
                    Icon = null;
                }
            }
        }
    }
}