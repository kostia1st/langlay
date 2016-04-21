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
        public KeyCode LanguageSwitchNonModifiers { get { return KeyUtils.ReduceKeyCodeArray(LanguageSwitchKeyArray, false); } }
        public KeyCode LanguageSwitchModifiers { get { return KeyUtils.ReduceKeyCodeArray(LanguageSwitchKeyArray, true); } }

        public bool DoSwitchLayout { get; set; }
        public IList<KeyCode> LayoutSwitchKeyArray { get; set; }
        public KeyCode LayoutSwitchNonModifiers { get { return KeyUtils.ReduceKeyCodeArray(LayoutSwitchKeyArray, false); } }
        public KeyCode LayoutSwitchModifiers { get { return KeyUtils.ReduceKeyCodeArray(LayoutSwitchKeyArray, true); } }

        public bool DoShowOverlay { get; set; }
        public bool DoShowOverlayOnMainDisplayOnly { get; set; }        
        public long OverlayMilliseconds { get; set; }
        public long OverlayOpacity { get; set; }
        public OverlayLocation OverlayLocation { get; set; }

        public SwitchMethod SwitchMethod { get; set; }
        public bool DoRunAtWindowsStartup { get; set; }
        public bool DoShowSettingsOnce { get; set; }

        public bool DoShowCursorTooltip { get; set; }

        public ConfigServiceBase()
        {
            GlobalConfig = OpenConfiguration(false);
            UserConfig = OpenConfiguration(true);

            LanguageSwitchKeyArray = new KeyCode[] { KeyCode.CapsLock };
            LayoutSwitchKeyArray = new KeyCode[] { };
            SwitchMethod = SwitchMethod.InputSimulation;
            DoShowOverlay = true;
            OverlayMilliseconds = 500;
            OverlayOpacity = 80;
            OverlayLocation = OverlayLocation.BottomCenter;

            DoRunAtWindowsStartup = true;
            DoSwitchLanguage = true;
            DoSwitchLayout = false;
            DoShowSettingsOnce = true;
        }

        private void ReadFromString(string str)
        {
            var arguments = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            arguments.ForEach(x => ReadArgument(x));
        }

        private IList<KeyCode> KeyStringToArray(string arrayString)
        {
            return arrayString.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries).Select(x => (KeyCode) Utils.ParseInt(x, 0)).ToList();
        }

        private void ReadArgument(string name, string value)
        {
            if (name == ArgumentNames.SwitchLanguage)
                DoSwitchLanguage = Utils.ParseBool(value, true);
            else if (name == ArgumentNames.SwitchLayout)
                DoSwitchLayout = Utils.ParseBool(value, false);
            else if (name == ArgumentNames.LanguageSwitchKeys)
                LanguageSwitchKeyArray = KeyStringToArray(value);
            else if (name == ArgumentNames.LayoutSwitchKeys)
                LayoutSwitchKeyArray = KeyStringToArray(value);

            else if (name == ArgumentNames.ShowOverlay)
                DoShowOverlay = Utils.ParseBool(value, false);
            else if (name == ArgumentNames.ShowOverlayOnMainDisplayOnly)
                DoShowOverlayOnMainDisplayOnly = Utils.ParseBool(value, false);
            else if (name == ArgumentNames.OverlayMilliseconds)
                OverlayMilliseconds = Utils.ParseInt(value, 300);
            else if (name == ArgumentNames.OverlayOpacity)
                OverlayOpacity = Utils.ParseInt(value, 80);
            else if (name == ArgumentNames.OverlayLocation)
                OverlayLocation = Utils.ParseEnum(value, OverlayLocation.BottomCenter);
                    
            else if (name == ArgumentNames.SwitchMethod)
                SwitchMethod = Utils.ParseEnum(value, SwitchMethod.InputSimulation);
            else if (name == ArgumentNames.RunAtWindowsStartup)
                DoRunAtWindowsStartup = Utils.ParseBool(value, false);
            else if (name == ArgumentNames.ShowSettingsOnce)
                DoShowSettingsOnce = Utils.ParseBool(value, true);
            else if (name == ArgumentNames.ShowCursorTooltip)
                DoShowCursorTooltip = Utils.ParseBool(value, false);
        }

        private void ReadArgument(string argument)
        {
            if (!argument.StartsWith("--"))
                throw new ArgumentException("Arguments must start with '--'");
            var parts = argument.Substring(2).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            var argumentName = parts[0];
            if (parts.Length > 1)
            {
                ReadArgument(argumentName, parts[1]);
            }
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
                File.WriteAllText((string) configPath, @"<?xml version=""1.0"" encoding=""utf-8"" ?>
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

    }
}
