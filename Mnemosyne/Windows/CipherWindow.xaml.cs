using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using WindowsForms = System.Windows.Forms;

namespace Mnemosyne
{
    public partial class CipherWindow : Window
    {
        private const int _KEY_MIN_LENGTH = PasswordGenerator.MIN_LENGTH;

        private CipherFile _file;
        private bool _updating_path_controls;

        public CipherWindow() : this(new CipherFile()) { }

        public CipherWindow(CipherFile file)
        {
            _file = file == null ? new CipherFile() : file;

            InitializeComponent();
            _UpdatePathControls();

            if (_file.Active)
            {
                _txt_File.Focus();
            }
            else
            {
                _pwd_Key.Focus();
            }
        }

        private void _UpdatePathControls()
        {
            _updating_path_controls = true;
            _txt_Directory.Text = Path.GetDirectoryName(_file.Path) + "\\";
            _txt_File.Text = Path.GetFileName(_file.Path);
            _updating_path_controls = false;

            _UpdateNonPathControls();
        }

        private void _UpdateNonPathControls()
        {
            if (!_updating_path_controls)
            {
                _file.Path = Path.Combine(_txt_Directory.Text, _txt_File.Text);
            }

            if (_file.Active)
            {
                _btn_File.Visibility = Visibility.Hidden;
                
                _btn_Cipher.Content = "Rekey";
                _btn_Cipher.IsEnabled = _pwd_Key.Password.Length >= _KEY_MIN_LENGTH && _pwd_Confirm.Password.Length >= _KEY_MIN_LENGTH;
            }
            else
            {
                if (_file.Exists())
                {
                    _btn_Cipher.Content = "Decrypt";
                    _pwd_Confirm.IsEnabled = _lbl_Confirm.IsEnabled = false;
                }
                else
                {
                    _btn_Cipher.Content = "Create";
                    _pwd_Confirm.IsEnabled = _lbl_Confirm.IsEnabled = true;
                }

                _btn_Cipher.IsEnabled = _pwd_Key.Password.Length >= _KEY_MIN_LENGTH && (_file.Exists() || _pwd_Confirm.Password.Length >= _KEY_MIN_LENGTH);
            }
        }

        private void _btn_Directory_Click(object sender, RoutedEventArgs e)
        {
            using (WindowsForms.FolderBrowserDialog dialog = new WindowsForms.FolderBrowserDialog()
            {
                SelectedPath = _txt_Directory.Text
            })
            {
                if (dialog.ShowDialog() == WindowsForms.DialogResult.OK)
                {
                    _txt_Directory.Text = Path.GetFullPath(dialog.SelectedPath);
                }
            }

            _UpdatePathControls();
        }

        private void _btn_File_Click(object sender, RoutedEventArgs e)
        {
            using (WindowsForms.OpenFileDialog dialog = new WindowsForms.OpenFileDialog())
            {
                dialog.InitialDirectory = _txt_Directory.Text;
                dialog.Filter = "Mnemosyne Cipher Files|*.mnem";
                if (dialog.ShowDialog() == WindowsForms.DialogResult.OK)
                {
                    _txt_Directory.Text = Path.GetFullPath(Path.GetDirectoryName(dialog.FileName));
                    _txt_File.Text = Path.GetFileName(dialog.FileName);
                }
            }
        }

        private void _btn_Default_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.Default.DefaultPath = _file.Path;
            UserSettings.Default.Save();
            MessageBox.Show(string.Format("Default file path set to \"{0}\"", _file.Path));
        }

        private void _btn_Generator_Click(object sender, RoutedEventArgs e)
        {
            PasswordGeneratorWindow window = new PasswordGeneratorWindow();
            if ((bool)window.ShowDialog())
            {
                _pwd_Key.Password = window.Password;
            }
        }

        private void _btn_Cipher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_file.Exists() || (!_file.Exists() && _pwd_Key.Password == _pwd_Confirm.Password))
                {
                    _file.Key = _pwd_Key.Password;
                    new MainWindow(_file.Activate()).Show();
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Passwords do not match.");
                }
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Decryption Failed."); // minimalistic message to avoid crypto oracle attacks
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void _txt_File_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _UpdateNonPathControls();
        }

        private void _txt_Directory_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _UpdateNonPathControls();
        }

        private void _pwd_Key_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _UpdatePathControls();
        }

        private void _pwd_Confirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _UpdatePathControls();
        }

        private void _txt_Directory_LostFocus(object sender, RoutedEventArgs e)
        {
            _UpdatePathControls();
        }

        private void _txt_File_LostFocus(object sender, RoutedEventArgs e)
        {
            _UpdatePathControls();
        }
    }
}
