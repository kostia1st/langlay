using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;

namespace Langwitch
{
    public class ConfigService: IConfigService
    {
        public IList<int> LanguageSwitchKeyArray { get; private set; }
        public Keys LanguageSwitchKeys { get { return ConvertIntsToKeys(LanguageSwitchKeyArray); } }
        public IList<int> LayoutSwitchKeyArray { get; private set; }
        public Keys LayoutSwitchKeys { get { return ConvertIntsToKeys(LayoutSwitchKeyArray); } }
        public bool ShowOverlay { get; private set; }
        public long OverlayMilliseconds { get; private set; }
        public SwitchMethod SwitchMethod { get; private set; }

        public ConfigService()
        {
            LanguageSwitchKeyArray = new int[] { (int) Keys.CapsLock };
            LayoutSwitchKeyArray = new int[] { };
            SwitchMethod = SwitchMethod.Message;
            ShowOverlay = true;
            OverlayMilliseconds = 1000;
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

        private void ReadArgument(string argument)
        {
            if (!argument.StartsWith("--"))
                throw new ArgumentException("Arguments must start with '--'");
            var parts = argument.Substring(2).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            var argumentName = parts[0];
            if (parts.Length > 1)
            {
                var argumentValue = parts[1];
                if (argumentName == ArgumentNames.LanguageSwitchKeys)
                    LanguageSwitchKeyArray = ReadArray(argumentValue);
                else if (argumentName == ArgumentNames.LayoutSwitchKeys)
                    LayoutSwitchKeyArray = ReadArray(argumentValue);
                else if (argumentName == ArgumentNames.ShowOverlay)
                    ShowOverlay = Utils.ParseBool(argumentValue, false);
                else if (argumentName == ArgumentNames.OverlayMilliseconds)
                    OverlayMilliseconds = long.Parse(argumentValue);
                else if (argumentName == ArgumentNames.SwitchMethod)
                    SwitchMethod = string.Equals(argumentValue, ArgumentNames.SwitchMethod_Message, StringComparison.InvariantCultureIgnoreCase) 
                        ? SwitchMethod.Message : SwitchMethod.InputSimulation;
            }
        }

        public void ReadFromConfigFile()
        {
            var arguments = ConfigurationManager.AppSettings["app:arguments"];
            ReadFromString(arguments);
        }

        public void ReadFromCommandLineArguments()
        {
            var arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1));
            ReadFromString(arguments);
        }
    }
}
