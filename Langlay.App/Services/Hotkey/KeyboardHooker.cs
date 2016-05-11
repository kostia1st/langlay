using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
        public Func<Func<int?>, int?> HookProcedureWrapper { get; set; }

        #endregion Events

        /// <summary>
        /// Strong reference to a native callback method.
        /// Thanks to it, we connect its GC lifetime to the lifetime of the hooker itself.
        /// </summary>
        private Win32.KeyboardHookProc HookProcedureHolder;

        public KeyboardHooker(
            bool doHookImmediately = true, Func<Func<int?>, int?> hookProcedureWrapper = null)
        {
            // This is a c# hack in order to keep a firm reference to a dynamically created delegate
            // so that it won't be collected by GC.
            HookProcedureHolder = HookProcedure;
            HookProcedureWrapper = hookProcedureWrapper;
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

        private int? HookInternals(int code, uint wParam, IntPtr lParam)
        {
            var result = (int?) null;
            if (code >= 0)
            {
                var keyInfo = (Win32.KeyboardInfo) Marshal.PtrToStructure(lParam, typeof(Win32.KeyboardInfo));
                var key = (Keys) keyInfo.VirtualKeyCode;
                var keyHeldBefore = KeyUtils.GetKeysPressed();

                var kea = new KeyEventArgs2(key, keyHeldBefore);
#if TRACE
                var keysString = string.Join(", ", kea.KeyStroke.Keys.Select(x => ((KeyCode) x).GetDisplayName()));
                var eventString = $"{ Win32.MessageToString(wParam) }: { keysString }";
#endif
                if (wParam.In(Win32.WM_KEYDOWN, Win32.WM_SYSKEYDOWN) && KeyDown != null)
                {
                    Trace.WriteLine($"Hooked { eventString }");
                    KeyDown(this, kea);
                }
                else if (wParam.In(Win32.WM_KEYUP, Win32.WM_SYSKEYUP) && KeyUp != null)
                {
                    Trace.WriteLine($"Hooked { eventString }");
                    KeyUp(this, kea);
                }

                if (kea.Handled)
                    result = 1;
                else
                    Trace.WriteLine($">> Not handled { eventString }");
            }
            return result;
        }

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        private int HookProcedure(int code, uint wParam, IntPtr lParam)
        {
            var result = (int?) null;
            if (HookProcedureWrapper != null)
                result = HookProcedureWrapper(() => HookInternals(code, wParam, lParam));
            else
                result = HookInternals(code, wParam, lParam);
            if (result == null)
                result = Win32.CallNextHookEx(HookHandle, code, wParam, lParam);
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