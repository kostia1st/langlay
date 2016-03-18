using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Langwitch
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

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Quit", delegate { if (OnExit != null) OnExit(); })
                });
                Icon = new NotifyIcon()
                {
                    Text = "Langwitch",
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
