using System;
using System.Collections.Generic;
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
        private IConfigService ConfigService { get; set; }
        public ObservableCollection<KeyCodeViewModel> LanguageSequence { get; private set; }
        public ObservableCollection<KeyCodeViewModel> LayoutSequence { get; private set; }
        public ObservableCollection<KeyCodeViewModel> PasteSequence { get; private set; }

        public ConfigViewModel(IConfigService configService)
        {
            ConfigService = configService;

            LanguageSequence = new ObservableCollection<KeyCodeViewModel>(
                ConfigService.LanguageSwitchKeyArray.Select(x => new KeyCodeViewModel { KeyCode = x }));
            LanguageSequence.CollectionChanged += LanguageSequence_CollectionChanged;

            LayoutSequence = new ObservableCollection<KeyCodeViewModel>(
                ConfigService.LayoutSwitchKeyArray.Select(x => new KeyCodeViewModel { KeyCode = x }));
            LayoutSequence.CollectionChanged += LayoutSequence_CollectionChanged;

            PasteSequence = new ObservableCollection<KeyCodeViewModel>(
                ConfigService.PasteKeyArray.Select(x => new KeyCodeViewModel { KeyCode = x }));
            PasteSequence.CollectionChanged += PasteSequence_CollectionChanged;
        }

        private void PasteSequence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyLayoutSequenceChanged();
        }

        public void NotifyLayoutSequenceChanged()
        {
            ConfigService.LayoutSwitchKeyArray = LayoutSequence.Select(x => x.KeyCode).ToList();
            RaisePropertyChanged(x => x.LayoutSequence);
        }

        private void LayoutSequence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyLayoutSequenceChanged();
        }

        public void RaiseLanguageSequenceChanged()
        {
            ConfigService.LanguageSwitchKeyArray = LanguageSequence.Select(x => x.KeyCode).ToList();
            RaisePropertyChanged(x => x.LanguageSequence);
        }

        public void RaisePasteSequenceChanged()
        {
            ConfigService.PasteKeyArray = PasteSequence.Select(x => x.KeyCode).ToList();
            RaisePropertyChanged(x => x.PasteSequence);
        }

        private void LanguageSequence_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaiseLanguageSequenceChanged();
        }

        public bool RunAtWindowsStartup
        {
            get { return ConfigService.DoRunAtWindowsStartup; }
            set { SetPropertyValue(x => x.RunAtWindowsStartup, x => x.DoRunAtWindowsStartup, value); }
        }

        public bool ShowOverlay
        {
            get { return ConfigService.DoShowOverlay; }
            set { SetPropertyValue(x => x.ShowOverlay, x => x.DoShowOverlay, value); }
        }

        public bool ShowOverlayOnMainDisplayOnly
        {
            get { return ConfigService.DoShowOverlayOnMainDisplayOnly; }
            set { SetPropertyValue(x => x.ShowOverlayOnMainDisplayOnly, x => x.DoShowOverlayOnMainDisplayOnly, value); }
        }

        public bool ShowOverlayRoundCorners
        {
            get { return ConfigService.DoShowOverlayRoundCorners; }
            set { SetPropertyValue(x => x.ShowOverlayRoundCorners, x => x.DoShowOverlayRoundCorners, value); }
        }

        public bool ShowLanguageNameInNative
        {
            get { return ConfigService.DoShowLanguageNameInNative; }
            set { SetPropertyValue(x => x.ShowLanguageNameInNative, x => x.DoShowLanguageNameInNative, value); }
        }

        public bool ShowCursorTooltip
        {
            get { return ConfigService.DoShowCursorTooltip; }
            set { SetPropertyValue(x => x.ShowCursorTooltip, x => x.DoShowCursorTooltip, value); }
        }

        public uint OverlayMilliseconds
        {
            get { return ConfigService.OverlayDuration; }
            set { SetPropertyValue(x => x.OverlayMilliseconds, x => x.OverlayDuration, value); }
        }

        public uint OverlayOpacity
        {
            get { return ConfigService.OverlayOpacity; }
            set { SetPropertyValue(x => x.OverlayOpacity, x => x.OverlayOpacity, value); }
        }

        public uint OverlayScale
        {
            get { return ConfigService.OverlayScale; }
            set { SetPropertyValue(x => x.OverlayScale, x => x.OverlayScale, value); }
        }

        public OverlayLocation OverlayLocation
        {
            get { return ConfigService.OverlayLocation; }
            set { SetPropertyValue(x => x.OverlayLocation, x => x.OverlayLocation, value); }
        }

        public SwitchMethod SwitchMethod
        {
            get { return ConfigService.SwitchMethod; }
            set { SetPropertyValue(x => x.SwitchMethod, x => x.SwitchMethod, value); }
        }

        public ObservableCollection<KeyCodeViewModel> LanguageSwitchSequence
        {
            get { return LanguageSequence; }
        }

        public ObservableCollection<KeyCodeViewModel> LayoutSwitchSequence
        {
            get { return LayoutSequence; }
        }

        public bool DisableCapsLockToggle
        {
            get { return ConfigService.DoDisableCapsLockToggle; }
            set { SetPropertyValue(x => x.DisableCapsLockToggle, x => x.DoDisableCapsLockToggle, value); }
        }

        public bool SwitchLanguage
        {
            get { return ConfigService.DoSwitchLanguage; }
            set { SetPropertyValue(x => x.SwitchLanguage, x => x.DoSwitchLanguage, value); }
        }

        public bool SwitchLayout
        {
            get { return ConfigService.DoSwitchLayout; }
            set { SetPropertyValue(x => x.SwitchLayout, x => x.DoSwitchLayout, value); }
        }

        public bool PasteWithoutFormatting
        {
            get { return ConfigService.DoPasteWithoutFormatting; }
            set { SetPropertyValue(x => x.PasteWithoutFormatting, x => x.DoPasteWithoutFormatting, value); }
        }

        public bool ShowSettingsOnce
        {
            get { return ConfigService.DoShowSettingsOnce; }
            set { SetPropertyValue(x => x.ShowSettingsOnce, x => x.DoShowSettingsOnce, value); }
        }

        private void SetPropertyValue<T1>(
            Expression<Func<ConfigViewModel, T1>> expression1,
            Expression<Func<IConfigService, T1>> expression2, T1 value)
        {
            var valueOld = expression2.Compile()(ConfigService);
            if (!EqualityComparer<T1>.Default.Equals(valueOld, value))
            {
                var propertyName = ExpressionUtils.GetMemberName(expression2);
                ConfigService.GetType().GetProperty(propertyName).SetValue(ConfigService, value, null);

                RaisePropertyChanged(expression1);
            }
        }

        private void RaisePropertyChanged<T1>(
            Expression<Func<ConfigViewModel, T1>> expression)
        {
            RaisePropertyChanged(ExpressionUtils.GetMemberName(expression));
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}