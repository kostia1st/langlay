using System;
using System.Diagnostics;
using System.Windows.Forms;
using Product.Common;

namespace Product {
    public class AppMessageFilter : IMessageFilter {
        public Action OnClose;

        [Conditional("DEBUG")]
        private void DebugMessage(Message m) {
            if (m.Msg != 275) { // Filter out timer-related messages
                Debug.WriteLine($"Received external message code: {m.Msg}");
            }
        }

        public bool PreFilterMessage(ref Message m) {
            var result = false;
            DebugMessage(m);
            if (m.Msg.In(Win32.WM_DESTROY, Win32.WM_QUIT, Win32.WM_USER_QUIT)) {
                OnClose?.Invoke();
                result = true;
            }
            return result;
        }
    }
}