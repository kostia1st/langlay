using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace Langlay.Installer.Actions
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = Context.Parameters["target"],
                    UseShellExecute = true
                });
        }
    }
}