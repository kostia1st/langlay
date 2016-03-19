using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Langwitch
{
    public partial class OverlayForm : Form
    {
        private Stopwatch WatchElapsed { get; set; }
        public long MillisecondsToKeepVisible { get; set; }
        private const long MillisecondsToFadeOut = 200;
        private const double VisibleOpacity = 0.8;

        public OverlayForm()
        {
            InitializeComponent();
            WatchElapsed = new Stopwatch();
        }

        protected override bool ShowWithoutActivation 
        {
            get { return true; }
        }

        protected override CreateParams CreateParams 
        {
            get
            {
                CreateParams baseParams = base.CreateParams;
                baseParams.ExStyle |=
                    (SafeMethods.WS_EX_NOACTIVATE | SafeMethods.WS_EX_TOOLWINDOW | SafeMethods.WS_EX_TOPMOST);
                baseParams.ExStyle &= ~SafeMethods.WS_EX_APPWINDOW;
                return baseParams;
            }
        }

        private void ResetAndRun()
        {
            Visible = false;
            if (timerOverlay.Enabled)
                timerOverlay.Stop();

            WatchElapsed.Restart();

            UpdateSizeAndPosition();
            timerOverlay.Start();
            OnTimer();
            Visible = true;

        }

        private void UpdateSizeAndPosition()
        {
            var screenBounds = Screen.PrimaryScreen.Bounds;
            Top = screenBounds.Top + screenBounds.Height - (int) (screenBounds.Height * 0.2);
            Left = screenBounds.Left + ((screenBounds.Width - Width) / 2);
            
            var hrgn = SafeMethods.CreateRoundRectRgn(0, 0, Width, Height, 20, 20);
            try
            {
                Region = System.Drawing.Region.FromHrgn(hrgn);
            }
            finally
            {
                // Make sure we free this unmanaged resource
                SafeMethods.DeleteObject(hrgn);
            }

        }

        public void PushMessage(string message)
        {
            labelOverlay.Text = "";
            Visible = false;
            labelOverlay.Text = message;
            ResetAndRun();
        }

        private double GetOpacity(long elapsed)
        {
            if (elapsed <= MillisecondsToKeepVisible)
            {
                return VisibleOpacity;
            }
            else if (elapsed <= MillisecondsToKeepVisible + MillisecondsToFadeOut)
            {
                return ((double) Math.Max(MillisecondsToKeepVisible + MillisecondsToFadeOut - elapsed, 0))
                    / MillisecondsToFadeOut
                    * VisibleOpacity;
            }
            else
            {
                return 0;
            }
        }

        private void OnTimer()
        {
            Opacity = GetOpacity(WatchElapsed.ElapsedMilliseconds);
            if (Opacity == 0)
            {
                labelOverlay.Text = "";
                timerOverlay.Stop();
                WatchElapsed.Stop();
                Visible = false;
            }
        }

        private void timerOverlay_Tick(object sender, EventArgs e)
        {
            OnTimer();
        }
    }
}
