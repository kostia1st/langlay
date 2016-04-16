using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        };

        public static bool IsModifier(Keys key)
        {
            return Modifiers.Contains(key);
        }

        public static Keys AddModifiers(Keys key)
        {
            //if ((SafeMethods.GetKeyState((int) Keys.CapsLock) & 0x0001) != 0) key = key | Keys.CapsLock;
            foreach (var modifier in Modifiers)
            {
                if ((Win32.GetAsyncKeyState((int) modifier) & 0x8000) != 0) key = key | modifier;
            }
            return key;
        }
    }
}
