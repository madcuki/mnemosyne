using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mnemosyne
{
    public partial class MainWindow : Window
    {
        private MnemosyneCipherFile file;

        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 3; i++)
            {
                allCredsView.Items.Add(new CredentialItem()
                {
                    Name = "lichess",
                    Username = "madcookie",
                    ShowPassword = false,
                    Password = "sjdfhkhglkdhfgklskdjfhg"
                });
            }
        }

        private void showHideAllPasswords(bool show)
        {
            foreach (CredentialItem cred in allCredsView.Items)
            {
                cred.ShowPassword = show;
            }
        }

        private void btn_showAllClick(object sender, RoutedEventArgs e)
        {
            showHideAllPasswords(true);
        }

        private void btn_hideAllClick(object sender, RoutedEventArgs e)
        {
            showHideAllPasswords(false);
        }

        private void btn_newClick(object sender, RoutedEventArgs e)
        {
            PasswordGeneratorWindow window = new PasswordGeneratorWindow();
            window.Show();
        }
    }
}
