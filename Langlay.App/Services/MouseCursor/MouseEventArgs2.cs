using System.Windows.Forms;
using Product.Common;

namespace Product
{
    public class MouseEventArgs2
    {
        public MouseButtons Buttons { get; set; }
        public Win32.Point Point { get; set; }
        public bool Handled { get; set; }

        public MouseEventArgs2(MouseButtons buttons, Win32.Point point)
        {
            Buttons = buttons;
            Point = point;
        }
    }
}