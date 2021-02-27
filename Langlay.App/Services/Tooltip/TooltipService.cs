using System.Drawing;
using System.Linq;
using Product.Common;

namespace Product {
    public class TooltipService : ITooltipService, ILifecycled {
        private TooltipForm TooltipForm { get; set; }

        public TooltipService() {
        }

        private TooltipForm CreateTooltipForm() {
            return new TooltipForm();
        }

        #region Start/Stop

        public bool IsStarted { get; private set; }

        public void Start() {
            if (!IsStarted) {
                var configService = ServiceRegistry.Instance.Get<IConfigService>();
                if (configService.DoShowCursorTooltip) {
                    IsStarted = true;
                    TooltipForm = CreateTooltipForm();
                }
            }
        }

        public void Stop() {
            if (IsStarted) {
                IsStarted = false;
                TooltipForm?.Dispose();
                TooltipForm = null;
            }
        }

        #endregion Start/Stop

        public string GetDisplayString() {
            return TooltipForm?.DisplayString;
        }

        public void Push(string displayString, Point position, bool resetTimer, ColorSet colorSet) {
            if (IsStarted) {
                if (colorSet != null) {
                    TooltipForm.BackColor = colorSet.BackgroundColor.ToWinForms();
                    TooltipForm.TextBrush.Color = colorSet.ForegroundColor.ToWinForms();
                } else {
                    TooltipForm.BackColor = Color.FromKnownColor(KnownColor.Black);
                    TooltipForm.TextBrush.Color = Color.FromKnownColor(KnownColor.White);
                }
                TooltipForm.Push(displayString, position, resetTimer);
            }
        }

        public bool GetIsVisible() {
            if (IsStarted && TooltipForm != null)
                return TooltipForm.GetIsVisible();
            return false;
        }
    }
}