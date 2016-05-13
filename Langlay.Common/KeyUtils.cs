using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Product.Common
{
    public static class KeyUtils
    {
        public static IList<Keys> GetKeysPressed()
        {
            var result = new List<Keys>();

            // Here we get the state of just the keys of our interest - the
            // ones supported by the app. These are all specified in KeyCode enum.
            var keyCodes = ((KeyCode[]) Enum
                .GetValues(typeof(KeyCode)))
                .Select(x => (Keys) x)
                .Skip(1);
            foreach (var code in keyCodes)
            {
                if ((Win32.GetKeyState((int) code) & 0x8000) != 0)
                    result.Add(code);
            }
            return result;
        }

        public static bool GetIsKeyToggled(Keys key)
        {
            return ((Win32.GetKeyState((int) key) & 0x0001) != 0);
        }

        public static bool IsEmpty(this Keys key)
        {
            return key == Keys.None;
        }

        public static bool IsEmpty(this KeyCode key)
        {
            return key == KeyCode.None;
        }

        public static bool AreEqual(this IList<KeyCode> keyCodes1, IList<KeyCode> keyCodes2)
        {
            return keyCodes1.Count == keyCodes2.Count
                && keyCodes1.Contain(keyCodes2);
        }

        public static bool Contain(this IList<KeyCode> keyCodes1, IList<KeyCode> keyCodes2)
        {
            return keyCodes1.Intersect(keyCodes2).Count() == keyCodes2.Count;
        }
    }
}