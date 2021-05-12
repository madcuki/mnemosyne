using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return credential;
            }
            set
            {
                credential = value;
            }
        }
        public string Password {
            get
            {
                if (ShowPassword)
                {
                    return credential.Password;
                }
                else
                {
                    return MASK;
                }
            }
            set
            {
                if (credential.Password == value)
                {
                    return;
                }
                credential.Password = value;
                RaisePropertyChanged("Password");
            }
        }
        public bool ShowPassword {
            get
            {
                return showPassword;
            }
            set
            {
                if (showPassword == value)
                {
                    return;
                }
                showPassword = value;
                RaisePropertyChanged("ShowPassword");
                RaisePropertyChanged("Password");
            }
        }

        private Credential credential;
        private bool showPassword;

        public CredentialItem(Credential credential = null)
        {
            Credential = (credential == null ? new Credential() : credential);
        }

        private void RaisePropertyChanged(string propName)
        {
            PropertyChangedEventHandler eh = PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
