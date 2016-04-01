using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;

namespace Product.Common
{
    public class ConfigServiceBase
    {
        protected Configuration Configuration { get; set; }
        public IList<int> LanguageSwitchKeyArray { get; set; }
        public Keys LanguageSwitchKeys { get { return ConvertIntsToKeys(LanguageSwitchKeyArray); } }
        public IList<int> LayoutSwitchKeyArray { get; set; }
        public Keys LayoutSwitchKeys { get { return ConvertIntsToKeys(LayoutSwitchKeyArray); } }
        public bool ShowOverlay { get; set; }
        public long OverlayMilliseconds { get; set; }
        public SwitchMethod SwitchMethod { get; set; }
        public bool DoRunAtWindowsStartup { get; set; }

        public ConfigServiceBase(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            Configuration = configuration;
            LanguageSwitchKeyArray = new int[] { (int) Keys.CapsLock };
            LayoutSwitchKeyArray = new int[] { };
            SwitchMethod = SwitchMethod.InputSimulation;
            ShowOverlay = true;
            OverlayMilliseconds = 500;
            DoRunAtWindowsStartup = true;
        }

        private Keys ConvertIntsToKeys(IList<int> ints)
        {
            var result = (Keys) ints.FirstOrDefault();
            for (var i = 1; i < ints.Count; i++)
            {
                result |= (Keys) ints[i];
            }
            return result;
        }

        private void ReadFromString(string str)
        {
            var arguments = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            arguments.ForEach(x => ReadArgument(x));
        }

        private IList<int> ReadArray(string arrayString)
        {
            return arrayString.Split(new[] { '+' }).Select(x => int.Parse(x)).ToList();
        }

        private void ReadArgument(string name, string value)
        {
            if (name == ArgumentNames.LanguageSwitchKeys)
                LanguageSwitchKeyArray = ReadArray(value);
            else if (name == ArgumentNames.LayoutSwitchKeys)
                LayoutSwitchKeyArray = ReadArray(value);
            else if (name == ArgumentNames.ShowOverlay)
                ShowOverlay = Utils.ParseBool(value, false);
            else if (name == ArgumentNames.OverlayMilliseconds)
                OverlayMilliseconds = long.Parse(value);
            else if (name == ArgumentNames.SwitchMethod)
                SwitchMethod = string.Equals(value, ArgumentNames.SwitchMethod_Message, StringComparison.InvariantCultureIgnoreCase)
                    ? SwitchMethod.Message : SwitchMethod.InputSimulation;
            else if (name == ArgumentNames.RunAtWindowsStartup)
                DoRunAtWindowsStartup = Utils.ParseBool(value, false);
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
