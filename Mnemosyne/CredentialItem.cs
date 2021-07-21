using System.ComponentModel;

namespace Mnemosyne
{
    public sealed class CredentialItem : INotifyPropertyChanged
    {
        public const string MASK = "********************";

        public event PropertyChangedEventHandler PropertyChanged;

        public Credential Credential
        {
            get
            {
                return _credential;
            }
            set
            {
                _credential = value;
            }
        }
        public string Password {
            get
            {
                if (ShowPassword)
                {
                    return _credential.Password;
                }
                else
                {
                    return MASK;
                }
            }
            set
            {
                if (_credential.Password == value)
                {
                    return;
                }
                _credential.Password = value;
                RaisePropertyChanged("Password");
            }
        }
        public bool ShowPassword {
            get
            {
                return _show_password;
            }
            set
            {
                if (_show_password == value)
                {
                    return;
                }
                _show_password = value;
                RaisePropertyChanged("ShowPassword");
                RaisePropertyChanged("Password");
            }
        }

        private Credential _credential;
        private bool _show_password;

        public CredentialItem(Credential credential = null)
        {
            Credential = credential == null ? new Credential() : credential;
        }

        private void RaisePropertyChanged(string property_name)
        {
            PropertyChangedEventHandler eh = PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(property_name));
            }
        }
    }
}
