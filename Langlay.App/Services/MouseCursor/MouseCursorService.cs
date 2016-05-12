using System;
using System.Diagnostics;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class MouseCursorService : IDisposable
    {
        private MouseHooker Hooker { get; set; }
        private IConfigService ConfigService { get; set; }
        private ILanguageService LanguageService { get; set; }
        private ITooltipService TooltipService { get; set; }

        private bool IsLastDownHandled;

        public MouseCursorService(
            IConfigService configService, ILanguageService languageService,
            ITooltipService tooltipService)
        {
            if (configService == null)
                throw new ArgumentNullException("configService");
            if (languageService == null)
                throw new ArgumentNullException("languageService");
            if (tooltipService == null)
                throw new ArgumentNullException("tooltipService");
            ConfigService = configService;
            LanguageService = languageService;
            TooltipService = tooltipService;
        }

        #region Start/Stop

        private bool IsStarted { get; set; }

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                Hooker = new MouseHooker(false, HookProcedureWrapper);
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

        #endregion Start/Stop

        private string GetLanguageName(InputLayout layout)
        {
            return ConfigService.DoShowLanguageNameInNative
                ? layout.LanguageNameThreeLetterNative.ToLower()
                : layout.LanguageNameThreeLetter.ToLower();
        }

        private void ShowTooltip(MouseEventArgs2 e)
        {
            var currentLayout = LanguageService.GetCurrentLayout();
            var text = GetLanguageName(currentLayout);
            TooltipService.Push(text, new System.Drawing.Point(e.Point.X, e.Point.Y), true);
        }

        private void UpdateTooltip(MouseEventArgs2 e)
        {
            if (TooltipService.GetIsVisible())
            {
                TooltipService.Push(TooltipService.GetDisplayString(), new System.Drawing.Point(e.Point.X, e.Point.Y), false);
            }
        }

        #region Hook handling

        private int? HookProcedureWrapper(Func<int?> func)
        {
            var result = (int?) null;
            try
            {
                result = func();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
            return result;
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

        #endregion Hook handling

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

        #endregion IDisposable Support
    }
}