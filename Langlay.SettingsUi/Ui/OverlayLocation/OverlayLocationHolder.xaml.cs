using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Product.Common;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for OverlayLocationHolder.xaml
    /// </summary>
    public partial class OverlayLocationHolder : UserControl
    {
        public static readonly DependencyProperty LocationProperty
            = DependencyProperty.Register(
                "Location", typeof(OverlayLocation), typeof(OverlayLocationHolder),
                new PropertyMetadata(OverlayLocation.None, OnLocationChanged));

        public OverlayLocation Location
        {
            get { return (OverlayLocation) GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty
            = DependencyProperty.Register(
                "IsSelected", typeof(bool), typeof(OverlayLocationHolder),
                new PropertyMetadata(false, OnIsSelectedChanged));

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public OverlayLocationHolder()
        {
            InitializeComponent();
        }

        private void OnIsSelectedChanged(bool isSelected)
        {
            //Background = isSelected ? SystemColors.HighlightBrush : Brushes.Transparent;
            Foreground = isSelected ? SystemColors.HighlightBrush : Brushes.Transparent;
        }

        protected static void OnIsSelectedChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var control = (OverlayLocationHolder) d;
            control.OnIsSelectedChanged((bool) args.NewValue);
        }

        private void OnLocationChanged(OverlayLocation location)
        {
            tbkText.Text = location.ToDisplayString();
        }

        protected static void OnLocationChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var control = (OverlayLocationHolder) d;
            control.OnLocationChanged((OverlayLocation) args.NewValue);
        }
    }
}