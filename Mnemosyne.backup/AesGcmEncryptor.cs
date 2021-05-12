using System;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Mnemosyne
{
    class AesGcmEncryptor
    {
        public enum BitSizes
        {
            NonceBitSize = 128,
            MacBitSize = 128,
            KeyBitSize = 256
        }

        private static readonly SecureRandom random = new SecureRandom();
    }
}
