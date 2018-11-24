using System;
using System.Drawing;

namespace Product
{
    public class TooltipService : ITooltipService
    {
        public IConfigService ConfigService { get; private set; }
        private TooltipForm TooltipForm { get; set; }

        public TooltipService(IConfigService configService)
        {
            ConfigService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        private TooltipForm CreateTooltipForm()
        {
            return new TooltipForm();
        }

        #region Start/Stop

        private bool IsStarted { get; set; }

        public void Start()
        {
            if (!IsStarted)
            {
                if (ConfigService.DoShowCursorTooltip)
                {
                    IsStarted = true;
                    TooltipForm = CreateTooltipForm();
                }
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                TooltipForm.Dispose();
                TooltipForm = null;
            }
        }

        #endregion Start/Stop

        public string GetDisplayString()
        {
            return TooltipForm.DisplayString;
        }

        public void Push(string displayString, Point position, bool resetTimer)
        {
            if (IsStarted)
                TooltipForm.Push(displayString, position, resetTimer);
        }

        public bool GetIsVisible()
        {
            if (IsStarted)
                return TooltipForm.GetIsVisible();
            return false;
        }
    }
}