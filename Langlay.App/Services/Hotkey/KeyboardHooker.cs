using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    /// <summary>
    /// A class that manages a global low level keyboard hook
    /// </summary>
    public class KeyboardHooker : IDisposable
    {
        /// <summary>
        /// The collections of keys to watch for
        /// </summary>
        public IList<KeyStroke> HookedKeys = new List<KeyStroke>();
        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        private IntPtr HookHandle = IntPtr.Zero;

        #region Events
        /// <summary>
        /// Occurs when one of the hooked keys is pressed
        /// </summary>
        public KeyEventHandler2 KeyDown;
        /// <summary>
        /// Occurs when one of the hooked keys is released
        /// </summary>
        public KeyEventHandler2 KeyUp;
        #endregion

        /// <summary>
        /// Strong reference to a native callback method.
        /// </summary>
        private Win32.KeyboardHookProc HookProcedureHolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardHooker"/> class and installs the keyboard hook.
        /// </summary>
        public KeyboardHooker(bool doHookImmediately = true)
        {
            // This is a c# hack in order to keep a firm reference to a dynamically created delegate
            // so that it won't be collected by GC.
            HookProcedureHolder = HookProcedure;
            if (doHookImmediately)
                Hook();
        }

        /// <summary>
        /// Installs the global hook
        /// </summary>
        public void Hook()
        {
            var hInstance = Win32.LoadLibrary("User32");
            HookHandle = Win32.SetWindowsHookEx(
                Win32.WH_KEYBOARD_LL, HookProcedureHolder, hInstance, 0);
        }

        /// <summary>
        /// Uninstalls the global hook
        /// </summary>
        public void Unhook()
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
        private int HookProcedure(int code, int wParam, ref Win32.KeyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                var nonModifiers = (Keys) lParam.vkCode;
                var modifiers = KeyUtils.AddModifiers();

                var kea = new KeyEventArgs2(nonModifiers, modifiers);
                if ((wParam == Win32.WM_KEYDOWN || wParam == Win32.WM_SYSKEYDOWN) && (KeyDown != null))
                {
                    if (HookedKeys.Any(x => x.NonModifiers == nonModifiers && x.Modifiers == modifiers))
                    {
                        Trace.WriteLine("Hooked keyDOWN " + modifiers.ToString() + " + " + nonModifiers.ToString());
                        KeyDown(this, kea);
                    }
                }
                else if ((wParam == Win32.WM_KEYUP || wParam == Win32.WM_SYSKEYUP) && (KeyUp != null))
                {
                    if (HookedKeys.Any(x => x.NonModifiers == nonModifiers && x.Modifiers == modifiers || x.Modifiers == modifiers))
                    {
                        Trace.WriteLine("Hooked keyUP " + modifiers.ToString() + " + " + nonModifiers.ToString());
                        KeyUp(this, kea);
                    }
                }
                if (kea.Handled)
                    return 1;
                else
                {
                    Trace.WriteLine("Not handled " + Win32.MessageToString(wParam) + ": " + modifiers.ToString() + " + " + nonModifiers.ToString());
                }
            }
            return Win32.CallNextHookEx(HookHandle, code, wParam, ref lParam);
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

                Unhook();
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
        #endregion
    }
}
