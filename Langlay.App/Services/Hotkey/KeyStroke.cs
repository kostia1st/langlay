using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Product
{
    public class KeyStroke
    {
        public Keys NonModifiers { get; set; }
        public Keys Modifiers { get; set; }

        public KeyStroke(Keys nonModifiers, Keys modifiers)
        {
            NonModifiers = nonModifiers;
            Modifiers = modifiers;
        }
    }
}
