using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Product.SettingsUi {
    /// <summary>
    /// Interaction logic for AttachmentComposer.xaml
    /// </summary>
    public partial class AttachmentComposer : UserControl {
        private IList<AttachmentViewModel> AttachmentList => (IList<AttachmentViewModel>) DataContext;
        public event RoutedEventHandler Changed;

        public AttachmentComposer() {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            var attachmentNew = new AttachmentViewModel("", null);
            AttachmentList.Add(attachmentNew);
            SynchronizationContext.Current.Post(state => {
                var contentPresenter = this.ItemsControl_Editors.ItemContainerGenerator.ContainerFromItem(attachmentNew) as ContentPresenter;
                if (contentPresenter != null) {
                    contentPresenter.ApplyTemplate();
                    var editor = contentPresenter.ContentTemplate.LoadContent() as AttachmentEditor;

                    editor?.SetFocus();
                }
            }, null);
        }

        private void AttachmentEditor_RemoveClick(object sender, RoutedEventArgs e) {
            var control = (AttachmentEditor) sender;
            AttachmentList.Remove((AttachmentViewModel) control.DataContext);
        }

        private void AttachmentEditor_Changed(object sender, RoutedEventArgs e) {
            Changed?.Invoke(this, e);
        }
    }
}
