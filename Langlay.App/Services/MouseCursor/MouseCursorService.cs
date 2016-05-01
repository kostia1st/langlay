using System;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class MouseCursorService : IDisposable
    {
        private MouseHooker Hooker { get; set; }
        private IConfigService ConfigService { get; set; }
        private ITooltipService TooltipService { get; set; }

        private bool IsStarted { get; set; }
        private bool IsLastDownHandled;

        public MouseCursorService(
            IConfigService configService, ITooltipService tooltipService)
        {
            if (configService == null)
                throw new ArgumentNullException("configService");
            if (tooltipService == null)
                throw new ArgumentNullException("tooltipService");
            ConfigService = configService;
            TooltipService = tooltipService;
        }

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                Hooker = new MouseHooker(false);
                Hooker.ButtonDown = Hooker_ButtonDown;
                Hooker.ButtonUp = Hooker_ButtonUp;
                Hooker.MouseMove = Hooker_MouseMove;
                Hooker.SetHook();
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                if (Hooker != null)
                {
                    Hooker.Dispose();
                    Hooker = null;
                }
            }
        }

        private void ShowTooltip(MouseEventArgs2 e)
        {
            var currentLayout = InputLayoutHelper.GetCurrentLayout();
            var text = currentLayout.LanguageNameTwoLetter;
            TooltipService.Push(text, new System.Drawing.Point(e.Point.X, e.Point.Y), true);
        }

        private void UpdateTooltip(MouseEventArgs2 e)
        {
            if (TooltipService.GetIsVisible())
            {
                var currentLayout = InputLayoutHelper.GetCurrentLayout();
                var text = currentLayout.LanguageNameTwoLetter;
                TooltipService.Push(text, new System.Drawing.Point(e.Point.X, e.Point.Y), false);
            }
        }

        protected void Hooker_ButtonDown(object sender, MouseEventArgs2 e)
        {
            if (e.Buttons == MouseButtons.Left
                && CursorUtils.GetIsCurrentCursorBeam())
            {
                ShowTooltip(e);
                IsLastDownHandled = true;
            }
            else
                IsLastDownHandled = false;
        }

        protected void Hooker_ButtonUp(object sender, MouseEventArgs2 e)
        {
            if (e.Buttons == MouseButtons.Left
                && !IsLastDownHandled
                && !TooltipService.GetIsVisible()
                && CursorUtils.GetIsCurrentCursorBeam())
            {
                ShowTooltip(e);
            }
        }

        protected void Hooker_MouseMove(object sender, MouseEventArgs2 e)
        {
            UpdateTooltip(e);
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                Stop();

                disposedValue = true;
            }
        }

        ~MouseCursorService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
