using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public partial class TooltipForm : Form
    {
        private const int MillisecondsToKeepVisible = 500;
        private Stopwatch PeriodElapsed { get; set; }
        public string DisplayString { get; set; }
        private const int OpacityWhenVisible = 80;
        private Font TextFont { get; set; }
        private Brush TextBrush { get; set; }
        private Point PivotPosition { get; set; }

        private const int MinWidth = 10;

        public TooltipForm()
        {
            InitializeComponent();

            PeriodElapsed = new Stopwatch();
            TextBrush = new SolidBrush(Color.White);
            TextFont = new Font(Font.FontFamily, 10);
        }

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

        public void Push(string displayString, Point position, bool resetTimer)
        {
            PivotPosition = position;
            if (resetTimer)
            {
                DisplayString = string.Empty;

                Invalidate();
                Application.DoEvents();
                Visible = false;

                DisplayString = displayString;
                ResetAndRun();
            }
            else
            {
                PivotPosition = position;
                UpdateRegionAndPosition();
                if (!string.Equals(DisplayString, displayString))
                {
                    DisplayString = displayString;
                    Invalidate();
                }
                Application.DoEvents();
            }
        }

        private void ResetAndRun()
        {
            Visible = false;
            if (timerTooltip.Enabled)
                timerTooltip.Stop();

            PeriodElapsed.Restart();

            UpdateRegionAndPosition();

            Visible = true;
            OnTimer();
            timerTooltip.Start();
        }

        private Graphics _graphicsForMeasuring;

        private void UpdateRegionAndPosition()
        {
            if (_graphicsForMeasuring == null)
                _graphicsForMeasuring = CreateGraphics();
            var sizeOfText = _graphicsForMeasuring.MeasureString(DisplayString, TextFont);
            var size = new Size(
                Math.Max((int) sizeOfText.Width + 8, MinWidth),
                (int) sizeOfText.Height + 8);

            var screen = Screen.FromPoint(PivotPosition);

            var position = new Point();
            position.X = PivotPosition.X + 20;
            if (position.X + Size.Width > screen.Bounds.Right)
                position.X = PivotPosition.X - size.Width;
            position.Y = PivotPosition.Y + 20;
            if (position.Y + Size.Height > screen.Bounds.Bottom)
                position.Y = PivotPosition.Y - size.Height;

            Bounds = new Rectangle(position, size);
        }

        public bool GetIsVisible()
        {
            return PeriodElapsed.IsRunning
                && PeriodElapsed.ElapsedMilliseconds < MillisecondsToKeepVisible;
        }

        private double GetOpacity(long elapsed)
        {
            if (elapsed <= MillisecondsToKeepVisible)
            {
                return (double) OpacityWhenVisible / 100;
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
                timerTooltip.Stop();
                PeriodElapsed.Stop();
                Visible = false;
            }
        }

        private void timerTooltip_Tick(object sender, EventArgs e)
        {
            try
            {
                OnTimer();
            }
            catch (Exception ex)
            {
#if TRACE
                Trace.TraceError(ex.ToString());
#endif
            }
        }

        private void TooltipForm_Paint(object sender, PaintEventArgs e)
        {
            var sizeOfText = e.Graphics.MeasureString(DisplayString, TextFont);
            var positionOfText = new PointF((Width - sizeOfText.Width - 1) / 2, (Height - sizeOfText.Height - 1) / 2);

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.DrawString(DisplayString, TextFont, TextBrush, positionOfText);
        }
    }
}