using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class TrayService
    {
        private IConfigService ConfigService { get; set; }
        private ContextMenu ContextMenu { get; set; }
        private NotifyIcon Icon { get; set; }
        private bool IsStarted { get; set; }

        public Action OnExit { get; set; }

        public TrayService(IConfigService configService)
        {
            ConfigService = configService;
        }

        private void LaunchProduct()
        {
            var settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), AppSpecific.SettingsPath);
            Process.Start(settingsPath);
        }

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Settings", delegate { LaunchProduct(); }),
                    new MenuItem("-"),
                    new MenuItem("Quit", delegate { if (OnExit != null) OnExit(); })
                });
                Icon = new NotifyIcon()
                {
                    Text = Application.ProductName,
                    Icon = new Icon(typeof(Program), "Keyboard Filled-16.ico"),
                    Visible = true,
                    ContextMenu = ContextMenu
                };
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
