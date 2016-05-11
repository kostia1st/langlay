using System;
using System.Diagnostics;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    /// <summary>
    /// A class that manages a global low level keyboard hook
    /// </summary>
    public class KeyboardHooker : IDisposable
    {
        private IntPtr HookHandle = IntPtr.Zero;

        #region Events

        public KeyEventHandler2 KeyDown;
        public KeyEventHandler2 KeyUp;

        #endregion Events

        /// <summary>
        /// Strong reference to a native callback method.
        /// </summary>
        private Win32.KeyboardHookProc HookProcedureHolder;

        public KeyboardHooker(bool doHookImmediately = true)
        {
            // This is a c# hack in order to keep a firm reference to a dynamically created delegate
            // so that it won't be collected by GC.
            HookProcedureHolder = HookProcedure;
            if (doHookImmediately)
                SetHook();
        }

        /// <summary>
        /// Installs the global hook
        /// </summary>
        public void SetHook()
        {
            var hInstance = Win32.LoadLibrary("User32");
            HookHandle = Win32.SetWindowsHookEx(
                Win32.WH_KEYBOARD_LL, HookProcedureHolder, hInstance, 0);
        }

        /// <summary>
        /// Uninstalls the global hook
        /// </summary>
        public void UnsetHook()
        {
            Win32.UnhookWindowsHookEx(HookHandle);
        }

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        private int HookProcedure(int code, uint wParam, ref Win32.KeyboardInfo lParam)
        {
            var result = (int?) null;
            if (code >= 0)
            {
                try
                {
                    var key = (Keys) lParam.VirtualKeyCode;
                    var keys = KeyUtils.GetKeysPressed();

                    var kea = new KeyEventArgs2(key, keys);

                    if (wParam.In(Win32.WM_KEYDOWN, Win32.WM_SYSKEYDOWN) && KeyDown != null)
                    {
                        Trace.WriteLine(string.Format(
                            "Hooked keyDOWN {0}",
                            string.Join(", ", keys)));
                        KeyDown(this, kea);
                    }
                    else if (wParam.In(Win32.WM_KEYUP, Win32.WM_SYSKEYUP) && KeyUp != null)
                    {
                        Trace.WriteLine(string.Format(
                            "Hooked keyUP {0}",
                            string.Join(", ", keys)));
                        KeyUp(this, kea);
                    }

                    if (kea.Handled)
                    {
                        result = 1;
                    }
                    else
                    {
                        Trace.WriteLine(string.Format(
                            ">> Not handled {0}: {1}",
                            Win32.MessageToString(wParam),
                            string.Join(", ", keys)));
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    throw;
                }
            }
            if (result == null)
                result = Win32.CallNextHookEx(HookHandle, code, wParam, ref lParam);
            return result.Value;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                UnsetHook();
                disposedValue = true;
            }
        }

        ~KeyboardHooker()
        {
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}