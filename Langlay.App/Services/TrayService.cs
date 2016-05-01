using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Product
{
    public class TrayService
    {
        private IConfigService ConfigService { get; set; }
        private ISettingsService SettingsService { get; set; }
        private ContextMenu ContextMenu { get; set; }
        private NotifyIcon Icon { get; set; }
        private bool IsStarted { get; set; }

        public Action OnExit { get; set; }

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

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Settings", delegate { SettingsService.ShowSettings(); }) { DefaultItem = true },
                    new MenuItem("-"),
                    new MenuItem("Report a bug", delegate { OpenIssues(); }),
                    new MenuItem("Visit homepage", delegate { OpenHomepage(); }),
                    new MenuItem("-"),
                    new MenuItem("Quit", delegate { if (OnExit != null) OnExit(); })
                });
                Icon = new NotifyIcon()
                {
                    Text = Application.ProductName,
                    Icon = new Icon(typeof(Program), "Keyboard-Filled-2-16.ico"),
                    Visible = true,
                    ContextMenu = ContextMenu,
                };
                Icon.MouseDoubleClick += delegate { SettingsService.ShowSettings(); };
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                if (ContextMenu != null)
                    ContextMenu.Dispose();
                if (Icon != null)
                    Icon.Dispose();
            }
        }
    }
}
