using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public partial class OverlayForm : Form
    {
        private Stopwatch PeriodElapsed { get; set; }
        public long MillisecondsToKeepVisible { get; set; }
        public long OpacityWhenVisible { get; set; }
        public OverlayLocation DisplayLocation { get; set; }
        public Screen Screen { get; set; }

        private const long MillisecondsToFadeOut = 200;
        public string LanguageName { get; set; }
        public string LayoutName { get; set; }

        private Font LanguageFont { get; set; }
        private Brush LanguageBrush { get; set; }
        private Font LayoutFont { get; set; }
        private Brush LayoutBrush { get; set; }

        private const int MinWidth = 140;

        public OverlayForm()
        {
            InitializeComponent();
            PeriodElapsed = new Stopwatch();
            LanguageBrush = new SolidBrush(Color.White);
            LayoutBrush = new SolidBrush(Color.Gray);
            LanguageFont = new Font(Font.FontFamily, 28);
            LayoutFont = new Font(Font.FontFamily, 14);
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

            PeriodElapsed.Restart();

            UpdateRegionAndPosition();
            timerOverlay.Start();
            OnTimer();
            Visible = true;

        }
        private const int ScreenMargin = 20;
        private IntPtr RegionHandle { get; set; }
        private void UpdateRegionAndPosition()
        {
            using (var g = this.CreateGraphics())
            {
                var sizeLanguage = g.MeasureString(LanguageName, LanguageFont);
                var sizeLayout = g.MeasureString(LayoutName, LayoutFont);
                this.Size = new Size(
                    Math.Max((int) Math.Max(sizeLanguage.Width, sizeLayout.Width) + 40, MinWidth), 
                    (int) sizeLanguage.Height + (int) sizeLayout.Height + 20);
            }

            var screenBounds = Screen.Bounds;
            switch (DisplayLocation)
            {
                case OverlayLocation.TopLeft:
                case OverlayLocation.MiddleLeft:
                case OverlayLocation.BottomLeft:
                    Left = screenBounds.Left + ScreenMargin;
                    break;
                case OverlayLocation.TopCenter:
                case OverlayLocation.MiddleCenter:
                case OverlayLocation.BottomCenter:
                    Left = screenBounds.Left + ((screenBounds.Width - Width) / 2);
                    break;
                case OverlayLocation.TopRight:
                case OverlayLocation.MiddleRight:
                case OverlayLocation.BottomRight:
                    Left = screenBounds.Left + screenBounds.Width - Width - ScreenMargin;
                    break;
            }

            switch (DisplayLocation)
            {
                case OverlayLocation.TopLeft:
                case OverlayLocation.TopCenter:
                case OverlayLocation.TopRight:
                    Top = screenBounds.Top + ScreenMargin;
                    break;
                case OverlayLocation.MiddleLeft:
                case OverlayLocation.MiddleCenter:
                case OverlayLocation.MiddleRight:
                    Top = screenBounds.Top + (screenBounds.Height - Height) / 2;
                    break;
                case OverlayLocation.BottomLeft:
                case OverlayLocation.BottomCenter:
                case OverlayLocation.BottomRight:
                    Top = screenBounds.Top + screenBounds.Height - Height - ScreenMargin;
                    break;
            }


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
            LanguageName = "";
            LayoutName = "";

            // Make sure the window has redrawn itself empty
            Invalidate();
            Application.DoEvents();
            Visible = false;

            // Put the new content into the window
            LanguageName = languageName;
            LayoutName = layoutName;

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
            Opacity = GetOpacity(PeriodElapsed.ElapsedMilliseconds);
            if (Opacity == 0)
            {
                timerOverlay.Stop();
                PeriodElapsed.Stop();
                Visible = false;
            }
        }

        private void timerOverlay_Tick(object sender, EventArgs e)
        {
            OnTimer();
        }

        private void OverlayForm_Paint(object sender, PaintEventArgs e)
        {
            var sizeLanguage = e.Graphics.MeasureString(LanguageName, LanguageFont);
            var sizeLayout = e.Graphics.MeasureString(LayoutName, LayoutFont);
            var pointLanguage = new PointF((Width - sizeLanguage.Width) / 2, (Height - sizeLanguage.Height - sizeLayout.Height) / 2);
            var pointLayout = new PointF((Width - sizeLayout.Width) / 2, pointLanguage.Y + sizeLanguage.Height);
            e.Graphics.DrawString(this.LanguageName, this.LanguageFont, this.LanguageBrush, pointLanguage);
            e.Graphics.DrawString(this.LayoutName, this.LayoutFont, this.LayoutBrush, pointLayout);
        }
    }
}
