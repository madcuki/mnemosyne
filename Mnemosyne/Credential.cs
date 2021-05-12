using System.ComponentModel;

namespace Mnemosyne
{
    public sealed class Credential : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        private string email;
        private string username;
        private string password;
        
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == value)
                {
                    return;
                }
                name = value;
                raisePropertyChanged("Name");
            }
        }
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                if (email == value)
                {
                    return;
                }
                email = value;
                raisePropertyChanged("Email");
            }
        }
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                if (username == value)
                {
                    return;
                }
                username = value;
                raisePropertyChanged("Username");
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (password == value)
                {
                    return;
                }
                password = value;
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
    }
}
