using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Product.Common;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [System.STAThreadAttribute()]
        public static void Main()
        {
            var productName = Assembly.GetEntryAssembly()
                .GetCustomAttributes(typeof(AssemblyProductAttribute))
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
