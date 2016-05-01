using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
            NotifyPropertyChanged(x => x.LayoutSequence);
        }
        private void LayoutSequence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyLayoutSequenceChanged();
        }

        public void NotifyLanguageSequenceChanged()
        {
            ConfigService.LanguageSwitchKeyArray = LanguageSequence.Select(x => x.KeyCode).ToList();
            NotifyPropertyChanged(x => x.LanguageSequence);
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
                    NotifyPropertyChanged(x => x.RunAtWindowsStartup);
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
                    NotifyPropertyChanged(x => x.ShowOverlay);
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
                    NotifyPropertyChanged(x => x.ShowOverlayOnMainDisplayOnly);
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
                    NotifyPropertyChanged(x => x.ShowOverlayRoundCorners);
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
                    NotifyPropertyChanged(x => x.ShowCursorTooltip);
                }
            }
        }

        public uint OverlayMilliseconds
        {
            get { return ConfigService.OverlayMilliseconds; }
            set
            {
                if (ConfigService.OverlayMilliseconds != value)
                {
                    ConfigService.OverlayMilliseconds = value;
                    NotifyPropertyChanged(x => x.OverlayMilliseconds);
                }
            }
        }

        public uint OverlayOpacity
        {
            get { return ConfigService.OverlayOpacity; }
            set
            {
                if (ConfigService.OverlayOpacity != value)
                {
                    ConfigService.OverlayOpacity = value;
                    NotifyPropertyChanged(x => x.OverlayOpacity);
                }
            }
        }

        public uint OverlayScale
        {
            get { return ConfigService.OverlayScale; }
            set
            {
                if (ConfigService.OverlayScale != value)
                {
                    ConfigService.OverlayScale = value;
                    NotifyPropertyChanged(x => x.OverlayScale);
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
                    NotifyPropertyChanged(x => x.OverlayLocation);
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
                    NotifyPropertyChanged(x => x.SwitchMethod);
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
                    NotifyPropertyChanged(x => x.SwitchLanguage);
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
                    NotifyPropertyChanged(x => x.SwitchLayout);
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
                    NotifyPropertyChanged(x => x.ShowSettingsOnce);
                }
            }
        }

        private void NotifyPropertyChanged<T1>(Expression<Func<ConfigViewModel, T1>> expression)
        {
            NotifyPropertyChanged(ExpressionUtils.GetMemberName(expression));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
