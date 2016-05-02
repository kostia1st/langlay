using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Product.Common
{
    public static class KeyUtils
    {
        public static KeyCode ReduceKeyCodeArray(IList<KeyCode> keyCodes, bool modifiers)
        {
            var keyCodesFiltered = keyCodes.Where(x => IsModifier((Keys) x) ^ !modifiers).ToList();
            var result = keyCodesFiltered.FirstOrDefault();
            for (var i = 1; i < keyCodesFiltered.Count; i++)
            {
                result |= keyCodesFiltered[i];
            }
            return result;
        }

        private static IList<Keys> Modifiers = new[] {
            Keys.LShiftKey,
            Keys.RShiftKey,
            Keys.LControlKey,
            Keys.RControlKey,
            Keys.LMenu,
            Keys.RMenu,
            Keys.LWin,
            Keys.RWin
        };

        private static IList<Keys> Toggles = new[] {
            Keys.CapsLock,
            Keys.Scroll,
            Keys.NumLock,
        };

        public static bool IsModifier(Keys key)
        {
            return Modifiers.Contains(key);
        }

        public static Keys AddToggles(Keys key = Keys.None)
        {
            foreach (var toggle in Toggles)
            {
                if ((Win32.GetKeyState((int) toggle) & 0x0001) != 0) key = key | toggle;
            }
            return key;
        }

        public static Keys AddModifiers(Keys key = Keys.None)
        {
            foreach (var modifier in Modifiers)
            {
                if ((Win32.GetKeyState((int) modifier) & 0x8000) != 0) key = key | modifier;
            }
            return key;
        }

        public static bool IsEmpty(this Keys key)
        {
            return key == Keys.None;
        }

        public static bool IsEmpty(this KeyCode key)
        {
            return key == KeyCode.None;
        }
    }
}
