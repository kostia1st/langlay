using System.Windows.Forms;

namespace Product
{
    public class KeyEventArgs2
    {
        public KeyStroke KeyStroke { get; set; }
        public bool Handled { get; set; }
        public KeyEventArgs2(Keys nonModifiers, Keys modifiers)
        {
            KeyStroke = new KeyStroke(nonModifiers, modifiers);
        }
    }
}
