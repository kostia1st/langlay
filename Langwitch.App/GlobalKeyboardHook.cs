using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
        private SafeMethods.KeyboardHookProc HookProcedureHolder;

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
            IntPtr hInstance = SafeMethods.LoadLibrary("User32");
            HookHandle = SafeMethods.SetWindowsHookEx(
                SafeMethods.WH_KEYBOARD_LL, HookProcedureHolder, hInstance, 0);
        }

        /// <summary>
        /// Uninstalls the global hook
        /// </summary>
        public void Unhook()
        {
            SafeMethods.UnhookWindowsHookEx(HookHandle);
        }

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        private int HookProcedure(int code, int wParam, ref SafeMethods.KeyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                Keys key = (Keys) lParam.vkCode;
                key = AddModifiers(key);
                if (HookedKeys.Contains(key))
                {
                    KeyEventArgs kea = new KeyEventArgs(key);
                    if ((wParam == SafeMethods.WM_KEYDOWN || wParam == SafeMethods.WM_SYSKEYDOWN) && (KeyDown != null))
                    {
                        KeyDown(this, kea);
                    }
                    else if ((wParam == SafeMethods.WM_KEYUP || wParam == SafeMethods.WM_SYSKEYUP) && (KeyUp != null))
                    {
                        KeyUp(this, kea);
                    }
                    if (kea.Handled)
                        return 1;
                }
            }
            return SafeMethods.CallNextHookEx(HookHandle, code, wParam, ref lParam);
        }

        private Keys AddModifiers(Keys key)
        {
            //if ((SafeMethods.GetKeyState((int) Keys.CapsLock) & 0x0001) != 0) key = key | Keys.CapsLock;
            if ((SafeMethods.GetKeyState((int) Keys.LShiftKey) & 0x8000) != 0) key = key | Keys.LShiftKey;
            if ((SafeMethods.GetKeyState((int) Keys.RShiftKey) & 0x8000) != 0) key = key | Keys.RShiftKey;
            if ((SafeMethods.GetKeyState((int) Keys.LControlKey) & 0x8000) != 0) key = key | Keys.LControlKey;
            if ((SafeMethods.GetKeyState((int) Keys.RControlKey) & 0x8000) != 0) key = key | Keys.RControlKey;
            if ((SafeMethods.GetKeyState((int) Keys.LMenu) & 0x8000) != 0) key = key | Keys.LMenu;
            if ((SafeMethods.GetKeyState((int) Keys.RMenu) & 0x8000) != 0) key = key | Keys.RMenu;
            return key;
        }

        private Keys RemoveModifiers(Keys keys)
        {
            return (Keys) ((int) keys & ~(int) Keys.Shift & ~(int) Keys.Control & ~(int) Keys.Alt);
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
