using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Product.Common;

namespace Product {
    public class ConfigPersister {
        protected Configuration GlobalConfig { get; set; }
        protected Configuration UserConfig { get; set; }

        private IDictionary<string, Action<string>> ReadStrategies { get; set; }
        private IDictionary<string, Func<string>> WriteStrategies { get; set; }

        public ConfigPersister(IConfigService config) {
            GlobalConfig = OpenConfiguration(false);
            UserConfig = OpenConfiguration(true);

            ReadStrategies = new Dictionary<string, Action<string>>()
            {
                { ArgumentNames.SwitchLanguage, x => config.DoSwitchLanguage = Utils.ParseBool(x, config.DoSwitchLanguage) },
                { ArgumentNames.SwitchLayout, x => config.DoSwitchLayout = Utils.ParseBool(x, config.DoSwitchLayout) },
                { ArgumentNames.PasteWithoutFormatting, x => config.DoPasteWithoutFormatting = Utils.ParseBool(x, config.DoPasteWithoutFormatting) },

                { ArgumentNames.LanguageSwitchKeys, x => config.LanguageSwitchKeyArray = KeyStringToArray(x) },
                { ArgumentNames.LayoutSwitchKeys, x => config.LayoutSwitchKeyArray = KeyStringToArray(x) },
                { ArgumentNames.PasteKeys, x => config.PasteKeyArray = KeyStringToArray(x) },
                { ArgumentNames.AppAttachments, x => config.AppAttachmentArray = StringToAppAttachments(x) },

                { ArgumentNames.DisableCapsLockToggle, x => config.DoDisableCapsLockToggle = Utils.ParseBool(x, config.DoDisableCapsLockToggle) },
                { ArgumentNames.ShowOverlay, x => config.DoShowOverlay = Utils.ParseBool(x, config.DoShowOverlay) },
                { ArgumentNames.ShowOverlayOnMainDisplayOnly, x => config.DoShowOverlayOnMainDisplayOnly = Utils.ParseBool(x, config.DoShowOverlayOnMainDisplayOnly) },
                { ArgumentNames.ShowOverlayRoundCorners, x => config.DoShowOverlayRoundCorners = Utils.ParseBool(x, config.DoShowOverlayRoundCorners) },
                { ArgumentNames.ShowLanguageNameInNative, x => config.DoShowLanguageNameInNative = Utils.ParseBool(x, config.DoShowLanguageNameInNative) },
                { ArgumentNames.OverlayDuration, x => config.OverlayDuration = Utils.ParseUInt(x, config.OverlayDuration) },
                { ArgumentNames.OverlayOpacity, x => config.OverlayOpacity = (uint) Utils.ParseUInt(x, config.OverlayOpacity, 1, 100) },
                { ArgumentNames.OverlayScale, x => config.OverlayScale = (uint) Utils.ParseUInt(x, config.OverlayScale, 50, 500) },
                { ArgumentNames.OverlayLocation, x => config.OverlayLocation = Utils.ParseEnum(x, config.OverlayLocation) },
                { ArgumentNames.SwitchMethod, x => config.SwitchMethod = Utils.ParseEnum(x, config.SwitchMethod) },
                { ArgumentNames.RunAtWindowsStartup, x => config.DoRunAtWindowsStartup = Utils.ParseBool(x, config.DoRunAtWindowsStartup) },
                { ArgumentNames.ShowSettingsOnce, x => config.DoShowSettingsOnce = Utils.ParseBool(x, config.DoShowSettingsOnce) },
                { ArgumentNames.ShowCursorTooltip, x => config.DoShowCursorTooltip = Utils.ParseBool(x, config.DoShowCursorTooltip) },
                { ArgumentNames.ShowCursorTooltip_WhenFocusNotChanged, x => config.DoShowCursorTooltip_WhenFocusNotChanged = Utils.ParseBool(x, config.DoShowCursorTooltip_WhenFocusNotChanged) },
                { ArgumentNames.ForceThisInstance, x => config.DoForceThisInstance = Utils.ParseBool(x, config.DoForceThisInstance) },
            };

            WriteStrategies = new Dictionary<string, Func<string>> {
                { ArgumentNames.RunAtWindowsStartup, () => config.DoRunAtWindowsStartup.ToString() },
                { ArgumentNames.ShowOverlay, () => config.DoShowOverlay.ToString() },
                { ArgumentNames.ShowOverlayOnMainDisplayOnly, () => config.DoShowOverlayOnMainDisplayOnly.ToString() },
                { ArgumentNames.ShowOverlayRoundCorners, () => config.DoShowOverlayRoundCorners.ToString() },
                { ArgumentNames.ShowLanguageNameInNative, () => config.DoShowLanguageNameInNative.ToString() },
                { ArgumentNames.OverlayDuration, () => config.OverlayDuration.ToString() },
                { ArgumentNames.OverlayOpacity, () => config.OverlayOpacity.ToString() },
                { ArgumentNames.OverlayScale, () => config.OverlayScale.ToString() },
                { ArgumentNames.OverlayLocation, () => config.OverlayLocation.ToString() },
                { ArgumentNames.SwitchMethod, () => config.SwitchMethod.ToString() },

                { ArgumentNames.SwitchLanguage, () => config.DoSwitchLanguage.ToString() },
                { ArgumentNames.LanguageSwitchKeys, () => ArrayToKeyString(config.LanguageSwitchKeyArray) },

                { ArgumentNames.SwitchLayout, () => config.DoSwitchLayout.ToString() },
                { ArgumentNames.LayoutSwitchKeys, () => ArrayToKeyString(config.LayoutSwitchKeyArray) },

                { ArgumentNames.PasteWithoutFormatting, () => config.DoPasteWithoutFormatting.ToString() },
                { ArgumentNames.PasteKeys, () => ArrayToKeyString(config.PasteKeyArray) },

                { ArgumentNames.AppAttachments, () => AppAttachmentsToString(config.AppAttachmentArray) },

                { ArgumentNames.DisableCapsLockToggle, () => config.DoDisableCapsLockToggle.ToString() },
                { ArgumentNames.ShowSettingsOnce, () => config.DoShowSettingsOnce.ToString() },
                { ArgumentNames.ShowCursorTooltip, () => config.DoShowCursorTooltip.ToString() },
                { ArgumentNames.ShowCursorTooltip_WhenFocusNotChanged, () => config.DoShowCursorTooltip_WhenFocusNotChanged.ToString() },
            };
        }

        public void SaveToFile() {
            // Execute all writing strategies available
            foreach (var argumentName in WriteStrategies.Keys) {
                WriteArgument(argumentName, WriteStrategies[argumentName]());
            }

            UserConfig.Save(ConfigurationSaveMode.Minimal, false);
        }

        public void WriteArgument(string name, string value) {
            var appSettings = UserConfig.AppSettings.Settings;
            var key = "app:" + name;
            if (!appSettings.AllKeys.Contains(key))
                appSettings.Add(key, value);
            else
                appSettings[key].Value = value;
        }

        private void ReadArgument(string name, string value) {
            if (ReadStrategies.ContainsKey(name))
                ReadStrategies[name](value);
        }

        private void ReadArgument(string argument) {
            if (!argument.StartsWith("--"))
                throw new ArgumentException("Arguments must start with '--'");
            var parts = argument.Substring(2).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
                ReadArgument(parts[0], parts[1]);
        }

        private void ReadFromString(string str) {
            var arguments = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            arguments.ForEach(x => ReadArgument(x));
        }

        private IList<KeyCode> KeyStringToArray(string arrayString) {
            return arrayString.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (KeyCode) Utils.ParseInt(x, 0))
                .Where(x => x != KeyCode.None)
                .ToList();
        }

        private string AppAttachmentToString(AppAttachment attachment) {
            return attachment.AppMask + "," + attachment.LanguageOrLayoutId;
        }

        private string AppAttachmentsToString(IList<AppAttachment> attachments) {
            return string.Join("|", attachments.Select(x => AppAttachmentToString(x)).ToList());
        }

        private AppAttachment StringToAppAttachment(string str) {
            var parts = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
                return new AppAttachment { AppMask = parts[0], LanguageOrLayoutId = parts[1] };
            return null;
        }

        private IList<AppAttachment> StringToAppAttachments(string str) {
            var itemStrings = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return itemStrings
                .Select(x => StringToAppAttachment(x))
                .Where(x => x != null)
                .ToList();
        }

        private string ArrayToKeyString(IList<KeyCode> keys) {
            return string.Join("+", keys.Select(x => ((int) x).ToString()));
        }

        protected static Configuration OpenConfiguration(bool isUserSpecific) {
            string rootPath;
            if (isUserSpecific) {
                rootPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    AppSpecific.MainAppTitle);
            } else {
                rootPath = PathUtils.GetAppDirectory();
            }

            var configPath = Path.Combine(rootPath, AppSpecific.MainAppConfigFilename);
            return OpenOrCreateFile(configPath);
        }

        protected static Configuration OpenOrCreateFile(string configPath) {
            if (string.IsNullOrEmpty(configPath))
                throw new ArgumentNullException(nameof(configPath));
            if (!File.Exists(configPath)) {
                var directoryPath = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(directoryPath)) {
                    Directory.CreateDirectory(directoryPath);
                }
                File.WriteAllText(configPath, @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <appSettings>
  </appSettings>
</configuration>");
            }
            return ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap() {
                RoamingUserConfigFilename = configPath,
                LocalUserConfigFilename = configPath,
                ExeConfigFilename = configPath
            }, ConfigurationUserLevel.None);
        }

        public void ReadFromConfigFile(bool isUserLevel) {
            if (isUserLevel)
                ReadFromConfigFile(UserConfig);
            else
                ReadFromConfigFile(GlobalConfig);
        }

        protected void ReadFromConfigFile(Configuration configuration) {
            var appSettings = configuration.AppSettings.Settings;
            foreach (var key in appSettings.AllKeys) {
                if (key.StartsWith("app:")) {
                    var settingName = key.Substring(4);
                    if (settingName == "arguments") {
                        var arguments = appSettings[key].GetValueOrDefault(x => x.Value);
                        ReadFromString(arguments);
                    } else
                        ReadArgument(settingName, appSettings[key].GetValueOrDefault(x => x.Value));
                }
            }
        }

        public void ReadFromCommandLineArguments() {
            var arguments = Environment.GetCommandLineArgs().Skip(1).ToList();
            arguments.ForEach(x => ReadArgument(x));
        }

    }
}
