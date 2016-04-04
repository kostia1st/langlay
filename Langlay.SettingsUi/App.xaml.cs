using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Product.Common;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            var productName = Assembly.GetEntryAssembly()
                .GetCustomAttributes(typeof(AssemblyProductAttribute), true)
                .OfType<AssemblyProductAttribute>()
                .FirstOrDefault().GetValueOrDefault(x => x.Product);
            var uniquenessService = new UniquenessService(productName);
            uniquenessService.RunOrIgnore(delegate
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            });
        }
    }
}
