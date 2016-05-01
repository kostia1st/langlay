﻿using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Product.Common;

namespace Product.SettingsUi
{
    public class ConfigService : ConfigServiceBase
    {
        public ConfigService()
            : base()
        {
        }

        public void SaveToFile()
        {
            WriteArgument(ArgumentNames.RunAtWindowsStartup, DoRunAtWindowsStartup.ToString());
            WriteArgument(ArgumentNames.ShowOverlay, DoShowOverlay.ToString());
            WriteArgument(ArgumentNames.ShowOverlayOnMainDisplayOnly, DoShowOverlayOnMainDisplayOnly.ToString());
            WriteArgument(ArgumentNames.ShowOverlayRoundCorners, DoShowOverlayRoundCorners.ToString());
            WriteArgument(ArgumentNames.OverlayMilliseconds, OverlayMilliseconds.ToString());
            WriteArgument(ArgumentNames.OverlayOpacity, OverlayOpacity.ToString());
            WriteArgument(ArgumentNames.OverlayScale, OverlayScale.ToString());
            WriteArgument(ArgumentNames.OverlayLocation, OverlayLocation.ToString());
            WriteArgument(ArgumentNames.SwitchMethod, SwitchMethod.ToString());
            WriteArgument(ArgumentNames.SwitchLanguage, DoSwitchLanguage.ToString());
            WriteArgument(ArgumentNames.LanguageSwitchKeys, ArrayToKeyString(LanguageSwitchKeyArray));
            WriteArgument(ArgumentNames.SwitchLayout, DoSwitchLayout.ToString());
            WriteArgument(ArgumentNames.LayoutSwitchKeys, ArrayToKeyString(LayoutSwitchKeyArray));
            WriteArgument(ArgumentNames.ShowSettingsOnce, DoShowSettingsOnce.ToString());
            WriteArgument(ArgumentNames.ShowCursorTooltip, DoShowCursorTooltip.ToString());
            UserConfig.Save(ConfigurationSaveMode.Minimal, false);
        }

        public void WriteArgument(string name, string value)
        {
            var appSettings = UserConfig.AppSettings.Settings;
            var key = "app:" + name;
            if (!appSettings.AllKeys.Contains(key))
                appSettings.Add(key, value);
            else
                appSettings[key].Value = value;
        }

        private string ArrayToKeyString(IList<KeyCode> keys)
        {
            return string.Join("+", keys.Select(x => ((int) x).ToString()));
        }
    }
}
