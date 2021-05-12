using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;

namespace Mnemosyne
{
    /// <summary>
    /// Interaction logic for DecryptWindow.xaml
    /// </summary>
    public partial class DecryptWindow : Window
    {
        public DecryptWindow()
        {
            InitializeComponent();
            initializeDecryptButton();
        }

        private void initializeDecryptButton()
        {
            if (txt_filename.Text != "")
            {
                if (File.Exists(txt_filename.Text))
                {
                    btn_decrypt.Content = "Decrypt";
                }
                else
                {
                    btn_decrypt.Content = "New File";
                }

                btn_decrypt.IsEnabled = true;
            }
            else
            {
                btn_decrypt.IsEnabled = false;
            }
        }

        private void btn_browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            dialog.Filter = "Text files (*.txt)|*.txt";
            dialog.Filter = "Mnemosyne Files|*.mnem";
            dialog.ShowDialog();
            txt_filename.Text = dialog.FileName;
        }

        private void btn_decrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pwd_key.Password.Length < 16)
                {
                    MessageBox.Show("Your key must be at least 16 characters.");
                    return;
                }
                MainWindow window = new MainWindow(new CipherFile(txt_filename.Text, pwd_key.Password));
                window.Show();
                Close();
            }
            catch (IOException)
            {
                MessageBox.Show("The file name or path is invalid.");
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Decryption failed.");
            }
        }

        private void btn_generator_Click(object sender, RoutedEventArgs e)
        {
            PasswordGeneratorWindow window = new PasswordGeneratorWindow();
            if (window.ShowDialog() == true)
            {
                pwd_key.Password = window.Password;
            }
        }

        private void txt_filename_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            initializeDecryptButton();
        }
    }
}
