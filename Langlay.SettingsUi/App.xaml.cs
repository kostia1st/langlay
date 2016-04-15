using System;
using System.Windows;
using Product.Common;

namespace Product.SettingsUi
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ConfigService ConfigService { get; set; }

        [STAThread]
        public static void Main()
        {
            try
            {
                //var productName = Assembly.GetEntryAssembly()
                //    .GetCustomAttributes(typeof(AssemblyProductAttribute), true)
                //    .OfType<AssemblyProductAttribute>()
                //    .FirstOrDefault().GetValueOrDefault(x => x.Product);
                var uniquenessService = new UniquenessService(System.Windows.Forms.Application.ProductName);
                uniquenessService.RunOrIgnore(delegate
                {
                    ConfigService = new ConfigService();
                    ConfigService.ReadFromConfigFile(false);
                    ConfigService.ReadFromConfigFile(true);

                    var app = new App();
                    app.InitializeComponent();
                    app.Run();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
