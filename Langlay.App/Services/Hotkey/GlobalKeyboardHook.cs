using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    /// <summary>
    /// A class that manages a global low level keyboard hook
    /// </summary>
    public class GlobalKeyboardHook : IDisposable
    {
        /// <summary>
        /// The collections of keys to watch for
        /// </summary>
        public List<Keys> HookedKeys = new List<Keys>();
        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        private IntPtr HookHandle = IntPtr.Zero;

        #region Events
        /// <summary>
        /// Occurs when one of the hooked keys is pressed
        /// </summary>
        public KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when one of the hooked keys is released
        /// </summary>
        public KeyEventHandler KeyUp;
        #endregion

        /// <summary>
        /// Strong reference to a native callback method.
        /// </summary>
        private Win32.KeyboardHookProc HookProcedureHolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalKeyboardHook"/> class and installs the keyboard hook.
        /// </summary>
        public GlobalKeyboardHook(bool doHookImmediately = true)
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
            IntPtr hInstance = Win32.LoadLibrary("User32");
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
                Keys key = (Keys) lParam.vkCode;
                key = AddModifiers(key);
                if (HookedKeys.Contains(key))
                {
                    KeyEventArgs kea = new KeyEventArgs((Keys) key);
                    if ((wParam == Win32.WM_KEYDOWN || wParam == Win32.WM_SYSKEYDOWN) && (KeyDown != null))
                    {
                        KeyDown(this, kea);
                    }
                    else if ((wParam == Win32.WM_KEYUP || wParam == Win32.WM_SYSKEYUP) && (KeyUp != null))
                    {
                        KeyUp(this, kea);
                    }
                    if (kea.Handled)
                        return 1;
                }
            }
            return Win32.CallNextHookEx(HookHandle, code, wParam, ref lParam);
        }

        private Keys AddModifiers(Keys key)
        {
            //if ((SafeMethods.GetKeyState((int) Keys.CapsLock) & 0x0001) != 0) key = key | Keys.CapsLock;
            if ((Win32.GetKeyState((int) Keys.LShiftKey) & 0x8000) != 0) key = key | Keys.LShiftKey;
            if ((Win32.GetKeyState((int) Keys.RShiftKey) & 0x8000) != 0) key = key | Keys.RShiftKey;
            if ((Win32.GetKeyState((int) Keys.LControlKey) & 0x8000) != 0) key = key | Keys.LControlKey;
            if ((Win32.GetKeyState((int) Keys.RControlKey) & 0x8000) != 0) key = key | Keys.RControlKey;
            if ((Win32.GetKeyState((int) Keys.LMenu) & 0x8000) != 0) key = key | Keys.LMenu;
            if ((Win32.GetKeyState((int) Keys.RMenu) & 0x8000) != 0) key = key | Keys.RMenu;
            return key;
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

        ~GlobalKeyboardHook()
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
