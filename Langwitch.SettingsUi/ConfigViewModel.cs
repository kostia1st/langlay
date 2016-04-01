using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product.Common;
using Product.SettingsUi;

namespace Product.SettingsUi
{
    public class ConfigViewModel : INotifyPropertyChanged
    {
        private ConfigService ConfigService { get; set; }

        public ConfigViewModel(ConfigService configService)
        {
            ConfigService = configService;
        }

        public bool ShowOverlay
        {
            get { return ConfigService.ShowOverlay; }
            set
            {
                if (ConfigService.ShowOverlay != value)
                {
                    ConfigService.ShowOverlay = value;
                    NotifyPropertyChanged("ShowOverlay");
                }
            }
        }

        public bool RunAtWindowsStartup
        {
            get { return ConfigService.DoRunAtWindowsStartup; }
            set
            {
                if (value != ConfigService.DoRunAtWindowsStartup)
                {
                    ConfigService.DoRunAtWindowsStartup = value;
                    NotifyPropertyChanged("RunAtWindowsStartup");
                }
            }
        }

        public long OverlayMilliseconds
        {
            get { return ConfigService.OverlayMilliseconds; }
            set
            {
                if (ConfigService.OverlayMilliseconds != value)
                {
                    ConfigService.OverlayMilliseconds = value;
                    NotifyPropertyChanged("OverlayMilliseconds");
                }
            }
        }

        public SwitchMethod SwitchMethod
        {
            get { return ConfigService.SwitchMethod; }
            set
            {
                if (ConfigService.SwitchMethod != value)
                {
                    ConfigService.SwitchMethod = value;
                    NotifyPropertyChanged("SwitchMethod");
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
