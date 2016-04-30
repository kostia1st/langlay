using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Product.Common;

namespace Product.SettingsUi
{
    public class ConfigViewModel : INotifyPropertyChanged
    {
        private ConfigService ConfigService { get; set; }
        private ObservableCollection<KeyCodeViewModel> LanguageSequence { get; set; }
        private ObservableCollection<KeyCodeViewModel> LayoutSequence { get; set; }

        public ConfigViewModel(ConfigService configService)
        {
            ConfigService = configService;
            LanguageSequence = new ObservableCollection<KeyCodeViewModel>(
                ConfigService.LanguageSwitchKeyArray.Select(x => new KeyCodeViewModel { KeyCode = x }));
            LanguageSequence.CollectionChanged += LanguageSequence_CollectionChanged;
            LayoutSequence = new ObservableCollection<KeyCodeViewModel>(
                ConfigService.LayoutSwitchKeyArray.Select(x => new KeyCodeViewModel { KeyCode = x }));
            LayoutSequence.CollectionChanged += LayoutSequence_CollectionChanged;
        }

        public void NotifyLayoutSequenceChanged()
        {
            ConfigService.LayoutSwitchKeyArray = LayoutSequence.Select(x => x.KeyCode).ToList();
            NotifyPropertyChanged("LayoutSequence");
        }
        private void LayoutSequence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyLayoutSequenceChanged();
        }

        public void NotifyLanguageSequenceChanged()
        {
            ConfigService.LanguageSwitchKeyArray = LanguageSequence.Select(x => x.KeyCode).ToList();
            NotifyPropertyChanged("LanguageSequence");
        }

        private void LanguageSequence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyLanguageSequenceChanged();
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

        public bool ShowOverlay
        {
            get { return ConfigService.DoShowOverlay; }
            set
            {
                if (ConfigService.DoShowOverlay != value)
                {
                    ConfigService.DoShowOverlay = value;
                    NotifyPropertyChanged("ShowOverlay");
                }
            }
        }

        public bool ShowOverlayOnMainDisplayOnly
        {
            get { return ConfigService.DoShowOverlayOnMainDisplayOnly; }
            set
            {
                if (ConfigService.DoShowOverlayOnMainDisplayOnly != value)
                {
                    ConfigService.DoShowOverlayOnMainDisplayOnly = value;
                    NotifyPropertyChanged("ShowOverlayOnMainDisplayOnly");
                }
            }
        }

        public bool ShowOverlayRoundCorners
        {
            get { return ConfigService.DoShowOverlayRoundCorners; }
            set
            {
                if (ConfigService.DoShowOverlayRoundCorners != value)
                {
                    ConfigService.DoShowOverlayRoundCorners = value;
                    NotifyPropertyChanged("ShowOverlayRoundCorners");
                }
            }
        }

        public bool ShowCursorTooltip
        {
            get { return ConfigService.DoShowCursorTooltip; }
            set
            {
                if (ConfigService.DoShowCursorTooltip != value)
                {
                    ConfigService.DoShowCursorTooltip = value;
                    NotifyPropertyChanged("ShowCursorTooltip");
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

        public long OverlayOpacity
        {
            get { return ConfigService.OverlayOpacity; }
            set
            {
                if (ConfigService.OverlayOpacity != value)
                {
                    ConfigService.OverlayOpacity = value;
                    NotifyPropertyChanged("OverlayOpacity");
                }
            }
        }

        public OverlayLocation OverlayLocation
        {
            get { return ConfigService.OverlayLocation; }
            set
            {
                if (ConfigService.OverlayLocation != value)
                {
                    ConfigService.OverlayLocation = value;
                    NotifyPropertyChanged("OverlayLocation");
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

        public ObservableCollection<KeyCodeViewModel> LanguageSwitchSequence
        {
            get { return LanguageSequence; }
        }

        public ObservableCollection<KeyCodeViewModel> LayoutSwitchSequence
        {
            get { return LayoutSequence; }
        }

        public bool SwitchLanguage
        {
            get { return ConfigService.DoSwitchLanguage; }
            set
            {
                if (ConfigService.DoSwitchLanguage != value)
                {
                    ConfigService.DoSwitchLanguage = value;
                    NotifyPropertyChanged("SwitchLanguage");
                }
            }
        }

        public bool SwitchLayout
        {
            get { return ConfigService.DoSwitchLayout; }
            set
            {
                if (ConfigService.DoSwitchLayout != value)
                {
                    ConfigService.DoSwitchLayout = value;
                    NotifyPropertyChanged("SwitchLayout");
                }
            }
        }

        public bool ShowSettingsOnce
        {
            get { return ConfigService.DoShowSettingsOnce; }
            set
            {
                if (ConfigService.DoShowSettingsOnce != value)
                {
                    ConfigService.DoShowSettingsOnce = value;
                    NotifyPropertyChanged("ShowSettingsOnce");
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
