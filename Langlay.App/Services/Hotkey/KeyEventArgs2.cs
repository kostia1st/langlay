using System.Windows.Forms;

namespace Product
{
    public class KeyEventArgs2
    {
        public Keys NonModifiers { get; set; }
        public Keys Modifiers { get; set; }
        public bool Handled { get; set; }
        public KeyEventArgs2(Keys nonModifiers, Keys modifiers)
        {
            NonModifiers = nonModifiers;
            Modifiers = modifiers;
        }
    }
}
