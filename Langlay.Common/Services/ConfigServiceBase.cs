using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Product.Common
{
    public class ConfigServiceBase
    {
        protected Configuration Configuration { get; set; }
        public bool DoSwitchLanguage { get; set; }
        public IList<KeyCode> LanguageSwitchKeyArray { get; set; }
        public KeyCode LanguageSwitchKeys { get { return ReduceKeyCodeArray(LanguageSwitchKeyArray); } }
        public bool DoSwitchLayout { get; set; }
        public IList<KeyCode> LayoutSwitchKeyArray { get; set; }
        public KeyCode LayoutSwitchKeys { get { return ReduceKeyCodeArray(LayoutSwitchKeyArray); } }
        public bool ShowOverlay { get; set; }
        public long OverlayMilliseconds { get; set; }
        public SwitchMethod SwitchMethod { get; set; }
        public bool DoRunAtWindowsStartup { get; set; }
        public bool DoShowSettingsOnce { get; set; }

        public ConfigServiceBase(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            Configuration = configuration;
            LanguageSwitchKeyArray = new KeyCode[] { KeyCode.CapsLock };
            LayoutSwitchKeyArray = new KeyCode[] { };
            SwitchMethod = SwitchMethod.InputSimulation;
            ShowOverlay = true;
            OverlayMilliseconds = 500;
            DoRunAtWindowsStartup = true;
            DoSwitchLanguage = true;
            DoSwitchLayout = false;
            DoShowSettingsOnce = true;
        }

        private KeyCode ReduceKeyCodeArray(IList<KeyCode> keyCodes)
        {
            var result = keyCodes.FirstOrDefault();
            for (var i = 1; i < keyCodes.Count; i++)
            {
                result |= keyCodes[i];
            }
            return result;
        }

        private void ReadFromString(string str)
        {
            var arguments = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            arguments.ForEach(x => ReadArgument(x));
        }

        private IList<KeyCode> KeyStringToArray(string arrayString)
        {
            return arrayString.Split(new[] { '+' }).Select(x => (KeyCode) int.Parse(x)).ToList();
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
                ShowOverlay = Utils.ParseBool(value, false);
            else if (name == ArgumentNames.OverlayMilliseconds)
                OverlayMilliseconds = long.Parse(value);
            else if (name == ArgumentNames.SwitchMethod)
                SwitchMethod = string.Equals(value, SwitchMethod.Message.ToString(), StringComparison.InvariantCultureIgnoreCase)
                    ? SwitchMethod.Message : SwitchMethod.InputSimulation;
            else if (name == ArgumentNames.RunAtWindowsStartup)
                DoRunAtWindowsStartup = Utils.ParseBool(value, false);
            else if (name == ArgumentNames.ShowSettingsOnce)
                DoShowSettingsOnce = Utils.ParseBool(value, true);
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

        public void ReadFromConfigFile()
        {
            var appSettings = Configuration.AppSettings.Settings;
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
    }
}
