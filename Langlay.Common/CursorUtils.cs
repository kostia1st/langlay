using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Product.Common
{
    public static class CursorUtils
    {
        public static bool GetIsCurrentCursorBeam()
        {
            Win32.CursorInfo pci;
            pci.Size = Marshal.SizeOf(typeof(Win32.CursorInfo));
            Win32.GetCursorInfo(out pci);

            return pci.Handle == Cursors.IBeam.Handle;
        }
    }
}
