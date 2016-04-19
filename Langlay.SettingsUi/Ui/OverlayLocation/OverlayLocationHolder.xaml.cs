﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            Loaded += OverlayLocationHolder_Loaded;
        }

        private void OverlayLocationHolder_Loaded(object sender, RoutedEventArgs e)
        {
            //OnLocationChanged(Location);
            //OnIsSelectedChanged(Location == OverlayLocation.BottomCenter);
        }

        private void OnIsSelectedChanged(bool isSelected)
        {
            Background = isSelected ? SystemColors.HighlightBrush : Brushes.Transparent;
            Foreground = isSelected ? SystemColors.HighlightTextBrush : Brushes.Transparent;
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
