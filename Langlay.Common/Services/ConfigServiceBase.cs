using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Product.Common
{
    public class ConfigServiceBase
    {
        protected Configuration GlobalConfig { get; set; }
        protected Configuration UserConfig { get; set; }

        public bool DoSwitchLanguage { get; set; }
        public IList<KeyCode> LanguageSwitchKeyArray { get; set; }

        public bool DoSwitchLayout { get; set; }
        public IList<KeyCode> LayoutSwitchKeyArray { get; set; }
        public bool DoDisableCapsLockToggle { get; set; }

        public bool DoShowOverlay { get; set; }
        public bool DoShowOverlayOnMainDisplayOnly { get; set; }
        public bool DoShowOverlayRoundCorners { get; set; }
        public bool DoShowLanguageNameInNative { get; set; }
        public uint OverlayDuration { get; set; }
        public uint OverlayOpacity { get; set; }
        public uint OverlayScale { get; set; }
        public OverlayLocation OverlayLocation { get; set; }

        public SwitchMethod SwitchMethod { get; set; }
        public bool DoRunAtWindowsStartup { get; set; }
        public bool DoShowSettingsOnce { get; set; }

        public bool DoForceThisInstance { get; set; }

        public bool DoShowCursorTooltip { get; set; }
        private IDictionary<string, Action<string>> ReadStrategies { get; set; }

        public ConfigServiceBase()
        {
            GlobalConfig = OpenConfiguration(false);
            UserConfig = OpenConfiguration(true);

            DoRunAtWindowsStartup = true;
            DoShowSettingsOnce = true;

            DoSwitchLanguage = true;
            DoSwitchLayout = false;
            LanguageSwitchKeyArray = new KeyCode[] { KeyCode.CapsLock };
            LayoutSwitchKeyArray = new KeyCode[] { };

            DoDisableCapsLockToggle = false;
            SwitchMethod = SwitchMethod.InputSimulation;
            DoShowOverlay = true;
            OverlayDuration = 300;
            OverlayOpacity = 80;
            OverlayScale = 100;
            OverlayLocation = OverlayLocation.BottomCenter;

            ReadStrategies = new Dictionary<string, Action<string>>()
            {
                { ArgumentNames.SwitchLanguage, x => DoSwitchLanguage = Utils.ParseBool(x, DoSwitchLanguage) },
                { ArgumentNames.SwitchLayout, x => DoSwitchLayout = Utils.ParseBool(x, DoSwitchLayout) },
                { ArgumentNames.LanguageSwitchKeys, x => LanguageSwitchKeyArray = KeyStringToArray(x) },
                { ArgumentNames.LayoutSwitchKeys, x => LayoutSwitchKeyArray = KeyStringToArray(x) },
                { ArgumentNames.DisableCapsLockToggle, x => DoDisableCapsLockToggle = Utils.ParseBool(x, DoDisableCapsLockToggle) },
                { ArgumentNames.ShowOverlay, x => DoShowOverlay = Utils.ParseBool(x, DoShowOverlay) },
                { ArgumentNames.ShowOverlayOnMainDisplayOnly, x => DoShowOverlayOnMainDisplayOnly = Utils.ParseBool(x, DoShowOverlayOnMainDisplayOnly) },
                { ArgumentNames.ShowOverlayRoundCorners, x => DoShowOverlayRoundCorners = Utils.ParseBool(x, DoShowOverlayRoundCorners) },
                { ArgumentNames.ShowLanguageNameInNative, x => DoShowLanguageNameInNative = Utils.ParseBool(x, DoShowLanguageNameInNative) },
                { ArgumentNames.OverlayDuration, x => OverlayDuration = Utils.ParseUInt(x, OverlayDuration) },
                { ArgumentNames.OverlayOpacity, x => OverlayOpacity = (uint) Utils.ParseUInt(x, OverlayOpacity, 1, 100) },
                { ArgumentNames.OverlayScale, x => OverlayScale = (uint) Utils.ParseUInt(x, OverlayScale, 50, 500) },
                { ArgumentNames.OverlayLocation, x => OverlayLocation = Utils.ParseEnum(x, OverlayLocation) },
                { ArgumentNames.SwitchMethod, x => SwitchMethod = Utils.ParseEnum(x, SwitchMethod) },
                { ArgumentNames.RunAtWindowsStartup, x => DoRunAtWindowsStartup = Utils.ParseBool(x, DoRunAtWindowsStartup) },
                { ArgumentNames.ShowSettingsOnce, x => DoShowSettingsOnce = Utils.ParseBool(x, DoShowSettingsOnce) },
                { ArgumentNames.ShowCursorTooltip, x => DoShowCursorTooltip = Utils.ParseBool(x, DoShowCursorTooltip) },
                { ArgumentNames.ForceThisInstance, x => DoForceThisInstance = Utils.ParseBool(x, DoForceThisInstance) },
            };
        }

        private void ReadArgument(string name, string value)
        {
            if (ReadStrategies.ContainsKey(name))
                ReadStrategies[name](value);
        }

        private void ReadArgument(string argument)
        {
            if (!argument.StartsWith("--"))
                throw new ArgumentException("Arguments must start with '--'");
            var parts = argument.Substring(2).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
                ReadArgument(parts[0], parts[1]);
        }

        private void ReadFromString(string str)
        {
            var arguments = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            arguments.ForEach(x => ReadArgument(x));
        }

        private IList<KeyCode> KeyStringToArray(string arrayString)
        {
            return arrayString.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (KeyCode) Utils.ParseInt(x, 0))
                .Where(x => x != KeyCode.None)
                .ToList();
        }

        public void ReadFromConfigFile(bool isUserLevel)
        {
            if (isUserLevel)
                ReadFromConfigFile(UserConfig);
            else
                ReadFromConfigFile(GlobalConfig);
        }

        protected void ReadFromConfigFile(Configuration configuration)
        {
            var appSettings = configuration.AppSettings.Settings;
            foreach (var key in appSettings.AllKeys)
            {
                if (key.StartsWith("app:"))
                {
                    var settingName = key.Substring(4);
                    if (settingName == "arguments")
                    {
                        var arguments = appSettings[key].GetValueOrDefault(x => x.Value);
                        ReadFromString(arguments);
                    }
                    else
                        ReadArgument(settingName, appSettings[key].GetValueOrDefault(x => x.Value));
                }
            }
        }

        public void ReadFromCommandLineArguments()
        {
            var arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1));
            ReadFromString(arguments);
        }

        protected static Configuration OpenConfiguration(bool isUserSpecific)
        {
            string rootPath;
            if (isUserSpecific)
            {
                rootPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    AppSpecific.MainAppTitle);
            }
            else
            {
                rootPath = PathUtils.GetAppDirectory();
            }

            var configPath = Path.Combine(rootPath, AppSpecific.MainAppConfigFilename);
            return OpenOrCreateFile(configPath);
        }

        protected static Configuration OpenOrCreateFile(string configPath)
        {
            if (string.IsNullOrEmpty(configPath))
                throw new ArgumentNullException("exeFilepath");
            if (!File.Exists(configPath))
            {
                var directoryPath = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                File.WriteAllText(configPath, @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <appSettings>
  </appSettings>
</configuration>");
            }
            return ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap()
            {
                RoamingUserConfigFilename = configPath,
                LocalUserConfigFilename = configPath,
                ExeConfigFilename = configPath
            }, ConfigurationUserLevel.None);
        }

        public bool GetLanguageSwitchConfigured()
        {
            return DoSwitchLanguage
                && LanguageSwitchKeyArray.Count > 0;
        }

        public bool GetLayoutSwitchConfigured()
        {
            return DoSwitchLayout
                && LayoutSwitchKeyArray.Count > 0;
        }
    }
}