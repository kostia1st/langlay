﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public partial class TooltipForm : Form
    {
        private const int MillisecondsToKeepVisible = 500;
        private Stopwatch PeriodElapsed { get; set; }
        private string DisplayString { get; set; }
        private const int OpacityWhenVisible = 80;
        private Font TextFont { get; set; }
        private Brush TextBrush { get; set; }
        private Point PivotPosition { get; set; }

        private const int MinWidth = 10;

        public TooltipForm()
        {
            InitializeComponent();

            PeriodElapsed = new Stopwatch();
            TextFont = new Font(Font.FontFamily, 10);
            TextBrush = new SolidBrush(Color.White);
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

        public void Push(string str, Point position, bool resetTimer)
        {
            PivotPosition = position;
            if (resetTimer)
            {
                DisplayString = string.Empty;

                Invalidate();
                Application.DoEvents();
                Visible = false;

                DisplayString = str;
                ResetAndRun();
            }
            else
            {
                PivotPosition = position;
                UpdateRegionAndPosition();
                if (DisplayString != str)
                {
                    DisplayString = str;
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
            timerTooltip.Start();
            OnTimer();

            Visible = true;
        }

        private void UpdateRegionAndPosition()
        {
            using (var g = this.CreateGraphics())
            {
                var sizeOfText = g.MeasureString(DisplayString, TextFont);
                Size = new Size(
                    Math.Max((int) sizeOfText.Width + 8, MinWidth),
                    (int) sizeOfText.Height + 8);
            }
            var screen = Screen.FromPoint(PivotPosition);
            var proposedLeft = PivotPosition.X + 20;
            var proposedTop = PivotPosition.Y + 20;
            if (proposedLeft + Size.Width > screen.Bounds.Right)
                proposedLeft = PivotPosition.X - Size.Width;
            if (proposedTop + Size.Height > screen.Bounds.Bottom)
                proposedTop = PivotPosition.Y - Size.Height;
            Left = proposedLeft;
            Top = proposedTop;
        }

        public bool GetIsVisible()
        {
            return PeriodElapsed.ElapsedMilliseconds < MillisecondsToKeepVisible;
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
            OnTimer();
        }

        private void TooltipForm_Paint(object sender, PaintEventArgs e)
        {
            var sizeOfText = e.Graphics.MeasureString(DisplayString, TextFont);
            var positionOfText = new PointF((Width - sizeOfText.Width - 1) / 2, (Height - sizeOfText.Height - 1) / 2);
            
            // Commented out because small text does not seem to benefit from AA, actually it's vice versa
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.DrawString(this.DisplayString, this.TextFont, this.TextBrush, positionOfText);

        }
    }
}