using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for AppBindingComposer.xaml
    /// </summary>
    public partial class AppBindingComposer : UserControl {
        private IList<AppBindingViewModel> AppBindingList => (IList<AppBindingViewModel>) DataContext;
        public event RoutedEventHandler Changed;

        public AppBindingComposer() {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            var appBindingNew = new AppBindingViewModel("", null);
            AppBindingList.Add(appBindingNew);
            SynchronizationContext.Current.Post(state => {
                var contentPresenter = this.ItemsControl_Editors.ItemContainerGenerator.ContainerFromItem(appBindingNew) as ContentPresenter;
                if (contentPresenter != null) {
                    contentPresenter.ApplyTemplate();
                    var editor = contentPresenter.ContentTemplate.LoadContent() as AppBindingEditor;

                    editor?.SetFocus();
                }
            }, null);
        }

        private void AppBindingEditor_RemoveClick(object sender, RoutedEventArgs e) {
            var control = (AppBindingEditor) sender;
            AppBindingList.Remove((AppBindingViewModel) control.DataContext);
        }

        private void AppBindingEditor_Changed(object sender, RoutedEventArgs e) {
            Changed?.Invoke(this, e);
        }
    }
}
