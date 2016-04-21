using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class MouseCursorService : IDisposable
    {
        private MouseHooker Hooker { get; set; }
        private IConfigService ConfigService { get; set; }
        private ITooltipService TooltipService { get; set; }

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

        private bool IsStarted { get; set; }
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

        protected void Hooker_ButtonDown(object sender, MouseEventArgs2 e)
        {
            if (GetIsCurrentCursorBeam())
            {
                var currentLayout = InputLayoutHelper.GetCurrentLayout();
                var text = currentLayout.LanguageNameTwoLetter;
                TooltipService.Push(text, new System.Drawing.Point(e.Point.x, e.Point.y), true);
            }

        }

        protected void Hooker_ButtonUp(object sender, MouseEventArgs2 e)
        {

        }

        protected void Hooker_MouseMove(object sender, MouseEventArgs2 e)
        {
            if (TooltipService.GetIsVisible())
            {
                var currentLayout = InputLayoutHelper.GetCurrentLayout();
                var text = currentLayout.LanguageNameTwoLetter;
                TooltipService.Push(text, new System.Drawing.Point(e.Point.x, e.Point.y), false);
            }
        }

        public bool GetIsCurrentCursorBeam()
        {
            Win32.CursorInfo pci;
            pci.cbSize = Marshal.SizeOf(typeof(Win32.CursorInfo));
            Win32.GetCursorInfo(out pci);

            return pci.hCursor == Cursors.IBeam.Handle;
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
