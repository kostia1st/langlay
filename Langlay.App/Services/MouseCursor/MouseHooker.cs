using System;
using System.Windows.Forms;
using Product.Common;

#if TRACE
using System.Diagnostics;
#endif

namespace Product {

    public class MouseHooker : IDisposable {

        private IntPtr HookHandle = IntPtr.Zero;

        #region Events

        public MouseEventHandler2 ButtonDown;
        public MouseEventHandler2 ButtonUp;
        public MouseEventHandler2 MouseMove;
        public Func<Func<int?>, int?> HookProcedureWrapper { get; set; }

        #endregion Events

        /// <summary>
        /// Strong reference to a native callback method.
        /// </summary>
        private Win32.MouseHookProc HookProcedureHolder;

        public MouseHooker(
            bool doHookImmediately = true, Func<Func<int?>, int?> hookProcedureWrapper = null) {
            // This is a c# hack in order to keep a firm reference to a
            // dynamically created delegate so that it won't be collected by GC.
            HookProcedureHolder = HookProcedure;
            HookProcedureWrapper = hookProcedureWrapper;
            if (doHookImmediately)
                SetHook();
        }

        /// <summary>
        /// Installs the global hook
        /// </summary>
        public void SetHook() {
            var hInstance = Win32.LoadLibrary("User32");
            HookHandle = Win32.SetWindowsHookEx(
                Win32.WH_MOUSE_LL, HookProcedureHolder, hInstance, 0);
        }

        /// <summary>
        /// Uninstalls the global hook
        /// </summary>
        public void UnsetHook() {
            Win32.UnhookWindowsHookEx(HookHandle);
        }

        private int? HookInternals(int code, uint wParam, IntPtr lParam) {
            int? result = null;
            if (code >= 0) {
                MouseEventArgs2 args = null;
                if (ButtonDown != null) {
                    if (wParam == Win32.WM_LBUTTONDOWN)
                        args = new MouseEventArgs2(MouseButtons.Left, lParam);
                    if (wParam == Win32.WM_RBUTTONDOWN)
                        args = new MouseEventArgs2(MouseButtons.Right, lParam);
                    if (wParam == Win32.WM_MBUTTONDOWN)
                        args = new MouseEventArgs2(MouseButtons.Middle, lParam);
                    if (args != null)
                        ButtonDown(this, args);
                }
                if (args == null && ButtonUp != null) {
                    if (wParam == Win32.WM_LBUTTONUP)
                        args = new MouseEventArgs2(MouseButtons.Left, lParam);
                    if (wParam == Win32.WM_RBUTTONUP)
                        args = new MouseEventArgs2(MouseButtons.Right, lParam);
                    if (wParam == Win32.WM_MBUTTONUP)
                        args = new MouseEventArgs2(MouseButtons.Middle, lParam);

                    if (args != null)
                        ButtonUp(this, args);
                }

                if (args == null && MouseMove != null) {
                    if (wParam == Win32.WM_MOUSEMOVE) {
                        args = new MouseEventArgs2(MouseButtons.None, lParam);
                        MouseMove(this, args);
                    }
                }
                if (args != null && args.Handled)
                    result = 1;
                else {
#if TRACE
                    if (wParam != Win32.WM_MOUSEMOVE)
                        Trace.WriteLine(">> Not handled " + Win32.MessageToString(wParam));
#endif
                }
            }
            return result;
        }

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">
        /// The hook code, if it isn't &gt;= 0, the function shouldn't do anyting
        /// </param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The mousehook event information</param>
        /// <returns></returns>
        private int HookProcedure(int code, uint wParam, IntPtr lParam) {
            var result = (int?) null;
            try {
                if (HookProcedureWrapper != null)
                    result = HookProcedureWrapper(() => HookInternals(code, wParam, lParam));
                else
                    result = HookInternals(code, wParam, lParam);
            } catch (Exception ex) {
#if TRACE
                Trace.TraceError(ex.ToString());
#endif
            }
            if (result == null)
                result = Win32.CallNextHookEx(HookHandle, code, wParam, lParam);
            return result.Value;
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                UnsetHook();
                disposedValue = true;
            }
        }

        ~MouseHooker() {
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}