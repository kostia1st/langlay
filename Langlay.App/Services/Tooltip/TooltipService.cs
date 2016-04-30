﻿using System.Drawing;

namespace Product
{
    public class TooltipService : ITooltipService
    {
        private IConfigService ConfigService { get; set; }
        private TooltipForm TooltipForm { get; set; }
        private bool IsStarted { get; set; }

        public TooltipService(IConfigService configService)
        {
            ConfigService = configService;
        }

        private TooltipForm CreateTooltip()
        {
            var form = new TooltipForm();

            return form;
        }

        public void Start()
        {
            if (!IsStarted)
            {
                if (ConfigService.DoShowCursorTooltip)
                {
                    IsStarted = true;
                    TooltipForm = CreateTooltip();
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

        public void Push(string str, Point position, bool resetTimer)
        {
            if (IsStarted)
                TooltipForm.Push(str, position, resetTimer);
        }

        public bool GetIsVisible()
        {
            if (IsStarted)
                return TooltipForm.GetIsVisible();
            return false;
        }
    }
}