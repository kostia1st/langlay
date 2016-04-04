using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class MouseCursorService
    {
        public bool IsBeam()
        {
            Win32.CURSORINFO pci;
            pci.cbSize = Marshal.SizeOf(typeof(Win32.CURSORINFO));
            Win32.GetCursorInfo(out pci);

            return pci.hCursor == Cursors.IBeam.Handle;
        }
    }
}
