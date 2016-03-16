using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Langwitch
{
    static class Program
    {
        private static bool IsExiting
        {
            get; set;
        }
        private static IDictionary<string, IntPtr> CultureToLastUsedLayout = new Dictionary<string, IntPtr>();
        private static KeyEventHandler OnKeyDown = Hooker_KeyDown;

        [STAThread]
        static void Main()
        {
            var configManager = new ConfigManager();
            configManager.ReadFromConfigFile();
            configManager.ReadFromCommandLineArguments();

            var contextMenu = new ContextMenu(new[]
            {
                new MenuItem("Quit", delegate { IsExiting = true; })
            });
            var trayIcon = new NotifyIcon()
            {
                Text = "Langwitch",
                Icon = new Icon(typeof(Program), "Keyboard Filled-16.ico"),
                Visible = true,
                ContextMenu = contextMenu
            };
            try
            {
                using (var hooker = new GlobalKeyboardHook())
                {
                    hooker.HookedKeys.Add(configManager.LanguageSwitchKeys);

                    hooker.KeyDown = OnKeyDown;
                    while (true)
                    {
                        Thread.Sleep(1);
                        try
                        {
                            Application.DoEvents();
                            if (IsExiting)
                                // Quitting the loop must be just enough
                                break;
                        }
                        catch (Exception ex)
                        {
                            // Do nothing O_o as of yet.
                            break;
                        }
                    }

                }
            }
            finally
            {
                trayIcon.Dispose();
                contextMenu.Dispose();
            }
        }

        private static void Hooker_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var languages = InputLanguage.InstalledInputLanguages;

                var currentLanguage = InputLanguageHelper.GetGlobalCurrentInputLanguage();
                if (currentLanguage != null)
                {
                    CultureToLastUsedLayout[currentLanguage.Culture.EnglishName] = currentLanguage.Handle;
                    var nextLanguageName = InputLanguageHelper.GetNextInputLanguageName(currentLanguage.Culture.EnglishName);
                    IntPtr layoutToSet;
                    if (CultureToLastUsedLayout.ContainsKey(nextLanguageName))
                        layoutToSet = CultureToLastUsedLayout[nextLanguageName];
                    else
                        layoutToSet = InputLanguageHelper.GetDefaultLayoutForLanguage(nextLanguageName);
                    InputLanguageHelper.SetCurrentLayout(layoutToSet);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
