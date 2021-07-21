using System.ComponentModel;

namespace Mnemosyne
{
    public sealed class Credential : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                {
                    return;
                }
                _name = value;
                raisePropertyChanged("Name");
            }
        }
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                if (_username == value)
                {
                    return;
                }
                _username = value;
                raisePropertyChanged("Username");
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password == value)
                {
                    return;
                }
                _password = value;
                raisePropertyChanged("Password");
            }
        }

        private void raisePropertyChanged(string propName)
        {
            PropertyChangedEventHandler eh = PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(propName));
            }
        }

        private string _name;
        private string _username;
        private string _password;
    }
}
