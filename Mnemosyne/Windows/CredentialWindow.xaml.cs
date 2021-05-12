using System.Windows;

namespace Mnemosyne
{
    /// <summary>
    /// Interaction logic for CredentialWindow.xaml
    /// </summary>
    public partial class CredentialWindow : Window
    {
        public Credential Credential
        {
            get
            {
                return credential;
            }
            set
            {
                credential = value;
            }
        }
        public Credential Draft
        {
            get
            {
                return draft;
            }
            set
            {
                draft = value;
            }
        }

        private Credential credential;
        private Credential draft;
        private CipherFile file;
        private bool isNew;

        public CredentialWindow(CipherFile file, Credential credential = null)
        {
            InitializeComponent();

            DataContext = this;

            this.file = file;
            isNew = credential == null;
            
            Credential = isNew ? new Credential() : credential;
            Draft = new Credential()
            { 
                Name = Credential.Name,
                Email = Credential.Email,
                Username = Credential.Username,
                Password = Credential.Password
            };
        }

        private void btn_generator_Click(object sender, RoutedEventArgs e)
        {
            PasswordGeneratorWindow window = new PasswordGeneratorWindow(Credential.Password);
            if ((bool)window.ShowDialog())
            {
                Draft.Password = window.Password;
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            Credential.Name = Draft.Name;
            Credential.Email = Draft.Email;
            Credential.Username = Draft.Username;
            Credential.Password = Draft.Password;

            if (isNew)
            {
                file.Credentials.Add(Credential);
            }
            file.Save();
            DialogResult = true;
            Close();
        }
    }
}
