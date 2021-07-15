using System.Windows;

namespace Mnemosyne
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            UserSettings.Default.Upgrade();
            UserSettings.Default.Save();

            if (e.Args.Length == 1)
            {
                new CipherWindow(new CipherFile(e.Args[0])).ShowDialog();
            }
            else
            {
                new CipherWindow().ShowDialog();
            }
        }
    }
}
