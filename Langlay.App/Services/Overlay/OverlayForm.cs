using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public partial class OverlayForm : Form
    {
        private Stopwatch WatchElapsed { get; set; }
        public long MillisecondsToKeepVisible { get; set; }
        public long OpacityWhenVisible { get; set; }
        private const long MillisecondsToFadeOut = 200;
        private Brush BrushLanguage { get; set; }
        private Brush BrushLayout { get; set; }
        public string LanguageName { get; set; }
        public string LayoutName { get; set; }
        private Font FontLanguage { get; set; }
        private Font FontLayout { get; set; }
        private const int MinWidth = 140;

        public OverlayForm()
        {
            InitializeComponent();
            WatchElapsed = new Stopwatch();
            BrushLanguage = new SolidBrush(Color.White);
            BrushLayout = new SolidBrush(Color.Gray);
            FontLanguage = new Font(Font.FontFamily, 28);
            FontLayout = new Font(Font.FontFamily, 14);
        }

        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;
                baseParams.ExStyle |=
                    (Win32.WS_EX_NOACTIVATE | Win32.WS_EX_TOOLWINDOW | Win32.WS_EX_TOPMOST);
                baseParams.ExStyle &= ~Win32.WS_EX_APPWINDOW;
                return baseParams;
            }
        }

        private void ResetAndRun()
        {
            Visible = false;
            if (timerOverlay.Enabled)
                timerOverlay.Stop();

            WatchElapsed.Restart();

            UpdateRegionAndPosition();
            timerOverlay.Start();
            OnTimer();
            Visible = true;

        }

        private IntPtr RegionHandle { get; set; }
        private void UpdateRegionAndPosition()
        {
            using (var g = this.CreateGraphics())
            {
                var sizeLanguage = g.MeasureString(LanguageName, FontLanguage);
                var sizeLayout = g.MeasureString(LayoutName, FontLayout);
                this.Size = new Size(
                    Math.Max((int) Math.Max(sizeLanguage.Width, sizeLayout.Width) + 40, MinWidth), 
                    (int) sizeLanguage.Height + (int) sizeLayout.Height + 20);
            }

            var screenBounds = Screen.PrimaryScreen.Bounds;
            Top = screenBounds.Top + screenBounds.Height - (int) (screenBounds.Height * 0.2);
            Left = screenBounds.Left + ((screenBounds.Width - Width) / 2);

            SetRoundedRegion();
        }

        private void SetRoundedRegion()
        {
            if (Region != null)
            {
                Region.Dispose();
                Region = null;
            }
            if (RegionHandle != IntPtr.Zero)
            {
                // Make sure we free this unmanaged resource whenever we don't use it anymore
                Win32.DeleteObject(RegionHandle);
                RegionHandle = IntPtr.Zero;
            }
            RegionHandle = Win32.CreateRoundRectRgn(0, 0, Width, Height, 20, 20);
            Region = System.Drawing.Region.FromHrgn(RegionHandle);
            UpdateBounds();
        }

        public void PushMessage(string languageName, string layoutName)
        {
            this.LanguageName = "";
            this.LayoutName = "";
            this.Invalidate();
            Application.DoEvents();
            Visible = false;
            this.LanguageName = languageName;
            this.LayoutName = layoutName;
            ResetAndRun();
        }

        private double GetOpacity(long elapsed)
        {
            if (elapsed <= MillisecondsToKeepVisible)
            {
                return (double) OpacityWhenVisible / 100;
            }
            else if (elapsed <= MillisecondsToKeepVisible + MillisecondsToFadeOut)
            {
                return ((double) Math.Max(MillisecondsToKeepVisible + MillisecondsToFadeOut - elapsed, 0))
                    / MillisecondsToFadeOut
                    * OpacityWhenVisible / 100;
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
                timerOverlay.Stop();
                WatchElapsed.Stop();
                Visible = false;
            }
        }

        private void timerOverlay_Tick(object sender, EventArgs e)
        {
            OnTimer();
        }

        private void OverlayForm_Paint(object sender, PaintEventArgs e)
        {
            var sizeLanguage = e.Graphics.MeasureString(LanguageName, FontLanguage);
            var sizeLayout = e.Graphics.MeasureString(LayoutName, FontLayout);
            var pointLanguage = new PointF((Width - sizeLanguage.Width) / 2, (Height - sizeLanguage.Height - sizeLayout.Height) / 2);
            var pointLayout = new PointF((Width - sizeLayout.Width) / 2, pointLanguage.Y + sizeLanguage.Height);
            e.Graphics.DrawString(this.LanguageName, this.FontLanguage, this.BrushLanguage, pointLanguage);
            e.Graphics.DrawString(this.LayoutName, this.FontLayout, this.BrushLayout, pointLayout);
        }
    }
}
