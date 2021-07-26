using System.Windows;

namespace Mnemosyne
{
    /// <summary>
    /// Interaction logic for CredentialWindow.xaml
    /// </summary>
    public partial class CredentialWindow : Window
    {
        public Credential Draft
        {
            get;
            set;
        }

        private Credential _credential;
        private CipherFile _file;
        private bool _new;

        public CredentialWindow(CipherFile file, Credential credential = null)
        {
            InitializeComponent();

            DataContext = this;

            _file = file;
            _new = credential == null;
            
            _credential = _new ? new Credential() : credential;
            Draft = new Credential()
            { 
                Name = _credential.Name,
                Username = _credential.Username,
                Password = _credential.Password
            };

            _txt_Name.Focus();
        }

        private void _btn_Generator_Click(object sender, RoutedEventArgs e)
        {
            PasswordGeneratorWindow generator = new PasswordGeneratorWindow(_credential.Password);
            if ((bool)generator.ShowDialog())
            {
                Draft.Password = generator.Password;
            }
        }

        private void _btn_Save_Click(object sender, RoutedEventArgs e)
        {
            _credential.Name = Draft.Name;
            _credential.Username = Draft.Username;
            _credential.Password = Draft.Password;

            if (_new)
            {
                _file.Data.Credentials.Add(_credential);
            }
            _file.Save();
            DialogResult = true;
            Close();
        }
    }
}
