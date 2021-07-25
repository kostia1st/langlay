using Microsoft.Win32;

namespace Product.Common {
    public static class SystemSettings {
        private const string RegistryPath_KeyboardLayoutKey = "HKEY_CURRENT_USER\\Keyboard Layout";
        private const string RegistryPath_ToggleKey = RegistryPath_KeyboardLayoutKey + "\\Toggle";
        private const int ShiftFlag = 0x00010000;
        private const string RegistryPath_Theme = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        public static bool GetIsShiftToDisableCapsLock() {
            var value = Utils.ParseInt(Registry.GetValue(
                RegistryPath_KeyboardLayoutKey, "Attributes", null), 0);

            if ((value & ShiftFlag) == ShiftFlag)
                return true;
            return false;
        }

        public static WindowsSequenceCode? GetLanguageSwitchSequence() {
            var result =
                (WindowsSequenceCode?) Utils.ParseInt(Registry.GetValue(
                    RegistryPath_ToggleKey,
                    "Language Hotkey", null));

            // Fallback to perhaps "old"-Windows-version key for the language sequence
            if (result == null) {
                result = (WindowsSequenceCode?) Utils.ParseInt(Registry.GetValue(
                    RegistryPath_ToggleKey,
                    "Hotkey", null));
                if (result == null) {
                    // this is by default (on Win10 at least)
                    result = WindowsSequenceCode.AltShift;
                }
            }
            return result;
        }

        public static WindowsSequenceCode? GetLayoutSwitchSequence() {
            var result = (WindowsSequenceCode?) Utils.ParseInt(Registry.GetValue(
                RegistryPath_ToggleKey,
                "Layout Hotkey", null));
            if (result == null) {
                // this is by default (on Win10 at least)
                result = WindowsSequenceCode.CtrlShift;
            }
            return result;
        }

        public static bool IsDarkMode() {
            var currentWindowsTheme = (WindowsTheme?) Registry.GetValue(RegistryPath_Theme, "AppsUseLightTheme", null);
            return currentWindowsTheme == WindowsTheme.Dark;
        }

    }
}