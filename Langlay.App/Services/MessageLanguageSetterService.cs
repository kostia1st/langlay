using System;
using Product.Common;

namespace Product {
    public class MessageLanguageSetterService : ILanguageSetterService {
        public bool SetCurrentLayout(IntPtr handle) {
            var foregroundWindowHandle = Win32.GetForegroundWindow();

            // TODO: this does not work with Skype, for some reason.
            IntPtr result;
            Win32.SendMessageTimeout(
                foregroundWindowHandle, Win32.WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, handle,
                Win32.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 500, out result);
            return true;
        }
    }
}