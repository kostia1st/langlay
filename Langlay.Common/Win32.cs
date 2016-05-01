using System;
using System.Runtime.InteropServices;

namespace Product.Common
{
    public class Win32
    {
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;

        public const int WH_MOUSE = 7;
        public const int WH_MOUSE_LL = 14;
        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_LBUTTONDBLCLK = 0x203;
        public const int WM_RBUTTONDOWN = 0x204;
        public const int WM_RBUTTONUP = 0x205;
        public const int WM_RBUTTONDBLCLK = 0x206;
        public const int WM_MBUTTONDOWN = 0x207;
        public const int WM_MBUTTONUP = 0x208;
        public const int WM_MBUTTONDBLCLK = 0x209;
        public const int WM_MOUSEWHEEL = 0x20A;
        public const int WM_MOUSEHWHEEL = 0x20E;
        public const int WM_XBUTTONDOWN = 0x20B;
        public const int WM_XBUTTONUP = 0x20C;
        public const int WM_XBUTTONDBLCLK = 0x20D;
        public const int WM_NCXBUTTONDOWN = 0xAB;
        public const int WM_NCXBUTTONUP = 0xAC;
        public const int WM_NCXBUTTONDBLCLK = 0xAD;


        public const int WM_INPUTLANGCHANGEREQUEST = 0x0050;
        public const int WM_CLOSE = 0x0010;
        public const int WM_DESTROY = 0x0002;
        public const int WM_QUIT = 0x0012;
        public const int WM_USER_RESTART = 0x0400 + 20;

        public static string MessageToString(uint message)
        {
            switch (message)
            {
                case WM_KEYDOWN:
                    return "Key Down";
                case WM_KEYUP:
                    return "Key Up";
                case WM_SYSKEYDOWN:
                    return "Key Down (Sys)";
                case WM_SYSKEYUP:
                    return "Key Up (Sys)";
            }
            return null;
        }

        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }

        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int WS_EX_APPWINDOW = 0x00040000;
        public const int WS_EX_TOPMOST = 0x00000008;

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(int threadId);
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr windowHandle, IntPtr id);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr windowHandle,
            uint messageId,
            IntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags flags,
            uint timeout,
            out IntPtr result);

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn
        (
            int topLeftX, // x-coordinate of upper-left corner
            int topLeftY, // y-coordinate of upper-left corner
            int bottomRightX, // x-coordinate of lower-right corner
            int bottomRightY, // y-coordinate of lower-right corner
            int roundingWidth, // height of ellipse
            int roundingHeight // width of ellipse
         );
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr objectHandle);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short GetKeyState(int keyCode);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short GetAsyncKeyState(int keyCode);
        /// <summary>
        /// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="callback">The callback.</param>
        /// <param name="hInstance">The handle you want to attach the event to, can be null</param>
        /// <param name="threadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, MouseHookProc callback, IntPtr hInstance, uint threadId);
        /// <summary>
        /// Unhooks the windows hook.
        /// </summary>
        /// <param name="hookHandle">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr idHook, int nCode, uint wParam, ref KeyboardInfo lParam);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr idHook, int nCode, uint wParam, ref MouseInfo lParam);
        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(int threadId, uint messageId, IntPtr wParam, IntPtr lParam);


        /// <summary>
        /// Defines the callback type for the hook
        /// </summary>
        public delegate int KeyboardHookProc(int code, uint wParam, ref KeyboardInfo lParam);

        public delegate int MouseHookProc(int nCode, uint wParam, ref MouseInfo lParam);

        public struct KeyboardInfo
        {
            public int VirtualKeyCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            private IntPtr _extraInfo;

            public IntPtr ExtraInfo { get { return _extraInfo; } }
        }

        public struct MouseInfo
        {
            public Point Point;
            public int MouseData;
            public int Flags;
            public int Time;
            private IntPtr _extraInfo;

            public IntPtr ExtraInfo { get { return _extraInfo; } }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CursorInfo
        {
            public int Size;
            public int Flags;
            private IntPtr _cursorHandle; 
            public Point ScreenPosition;

            public IntPtr Handle { get { return _cursorHandle; } }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorInfo(out CursorInfo pci);
    }
}
