using System.Collections.Generic;

namespace Mnemosyne
{
    public class CipherData
    {
        public List<Credential> Credentials
        {
            get;
            set;
        }

        public CipherData()
        {
            Credentials = new List<Credential>();
        }
    }
}
