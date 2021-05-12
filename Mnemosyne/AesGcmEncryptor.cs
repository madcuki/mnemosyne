using System.Security.Cryptography;

namespace Mnemosyne
{
    class AesGcmEncryptor
    {
        public (byte[], byte[]) Encrypt(byte[] plaintext, byte[] key, byte[] nonce, byte[] data)
        {
            byte[] tag = new byte[16];
            byte[] ciphertext = new byte[plaintext.Length];

            using (AesGcm gcm = new AesGcm(key))
            {
                gcm.Encrypt(nonce, plaintext, ciphertext, tag, data);
            }

            return (ciphertext, tag);
        }

        public byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] nonce, byte[] tag, byte[] data)
        {
            byte[] plaintext = new byte[ciphertext.Length];

            using (AesGcm gcm = new AesGcm(key))
            {
                gcm.Decrypt(nonce, ciphertext, tag, plaintext, data);
            }

            return plaintext;
        }
    }
}
