using System.Collections.Generic;
using System.Windows.Forms;

namespace Product
{
    public class KeyEventArgs2
    {
        public KeyStroke KeyStroke { get; set; }
        public bool Handled { get; set; }
        public KeyEventArgs2(Keys keyTriggered, IList<Keys> keysPressedBefore)
        {
            KeyStroke = new KeyStroke(keyTriggered, keysPressedBefore);
        }
    }
}
