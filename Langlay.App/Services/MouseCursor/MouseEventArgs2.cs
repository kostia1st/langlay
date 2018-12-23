using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Product.Common;

namespace Product {
    public class MouseEventArgs2 {
        public MouseButtons Buttons { get; private set; }
        private IntPtr MouseInfoPtr { get; set; }
        private Win32.MouseInfo _mouseInfo;
        private bool _mouseInfoInitialized;

        private Win32.MouseInfo MouseInfo {
            get {
                if (!_mouseInfoInitialized && MouseInfoPtr != IntPtr.Zero) {
                    _mouseInfo = (Win32.MouseInfo) Marshal.PtrToStructure(MouseInfoPtr, typeof(Win32.MouseInfo));
                    _mouseInfoInitialized = true;
                }
                return _mouseInfo;
            }
        }

        public Win32.Point Point => MouseInfo.Point;
        public bool Handled { get; set; }

        public MouseEventArgs2(MouseButtons buttons, IntPtr mouseInfoPtr) {
            Buttons = buttons;
            MouseInfoPtr = mouseInfoPtr;
        }
    }
}