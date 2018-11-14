using System;
using System.Diagnostics;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class MouseCursorService : IDisposable
    {
        private MouseHooker Hooker { get; set; }

        public IConfigService ConfigService { get; private set; }
        public ILanguageService LanguageService { get; private set; }
        public ITooltipService TooltipService { get; private set; }
        public IEventService EventService { get; private set; }

        private bool IsLastDownHandled;

        public MouseCursorService(
            IConfigService configService, ILanguageService languageService,
            ITooltipService tooltipService, IEventService eventService)
        {
            ConfigService = configService ?? throw new ArgumentNullException(nameof(configService));
            LanguageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
            TooltipService = tooltipService ?? throw new ArgumentNullException(nameof(tooltipService));
            EventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
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
                IsLastDownHandled = false;
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
            if (currentLayout != null)
            {
                var text = GetLanguageName(currentLayout);
                TooltipService.Push(text, new System.Drawing.Point(e.Point.X, e.Point.Y), true);
            }
        }

        private void UpdateTooltip(MouseEventArgs2 e)
        {
            if (TooltipService.GetIsVisible())
            {
                TooltipService.Push(
                    TooltipService.GetDisplayString(),
                    new System.Drawing.Point(e.Point.X, e.Point.Y), false);
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
            EventService.RaiseMouseInput();
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
            IsLastDownHandled = false;
            EventService.RaiseMouseInput();
        }

        protected void Hooker_MouseMove(object sender, MouseEventArgs2 e)
        {
            UpdateTooltip(e);
            EventService.RaiseMouseInput();
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