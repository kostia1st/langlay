using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for LayoutColorSetComposer.xaml
    /// </summary>
    public partial class LayoutColorSetComposer : UserControl {
        private IList<LayoutColorSetViewModel> LayoutColorSetList => (IList<LayoutColorSetViewModel>) DataContext;
        public event RoutedEventHandler Changed;

        public LayoutColorSetComposer() {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            var layoutColorSetNew = new LayoutColorSetViewModel(null, Colors.Black, Colors.Gray);
            LayoutColorSetList.Add(layoutColorSetNew);
            SynchronizationContext.Current.Post(state => {
                var contentPresenter = this.ItemsControl_Editors.ItemContainerGenerator.ContainerFromItem(layoutColorSetNew) as ContentPresenter;
                if (contentPresenter != null) {
                    contentPresenter.ApplyTemplate();
                    var editor = contentPresenter.ContentTemplate.LoadContent() as LayoutColorSetEditor;

                    editor?.SetFocus();
                }
            }, null);
        }

        private void LayoutColorSetEditor_RemoveClick(object sender, RoutedEventArgs e) {
            var control = (LayoutColorSetEditor) sender;
            LayoutColorSetList.Remove((LayoutColorSetViewModel) control.DataContext);
        }

        private void LayoutColorSetEditor_Changed(object sender, RoutedEventArgs e) {
            Changed?.Invoke(this, e);
        }
    }
}
