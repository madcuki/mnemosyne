using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Mnemosyne
{
    public class CipherFile
    {
        public const string DEFAULT_FILENAME = "vault";
        public const string FILE_EXTENSION   = ".mnem";

        public string FilePath;

        public List<Credential> Credentials;

        private FileStream writer;
        private AesGcmEncryptor gcm = new AesGcmEncryptor();
        private MnemRandom random = new MnemRandom();

        private string key;
        private byte[] nonce;

        public CipherFile(string filepath, string key)
        {
            FilePath = filepath;
            this.key = key;

            if (Exists())
            {
                Read();
            }
            else
            {
                Create();
                Credentials = new List<Credential>();
                nonce = new byte[12];
                Save();
                Read();
            }
        }

        public bool Exists()
        {
            return File.Exists(FilePath);
        }

        public void Create()
        {
            if (Path.GetExtension(FilePath) != FILE_EXTENSION)
            {
                FilePath += FILE_EXTENSION;
            }

            File.Create(FilePath).Close();
        }

        public void Read()
        {
            byte[] data = Convert.FromBase64String(File.ReadAllText(FilePath, Encoding.UTF8));

            Dictionary<string, byte[]> dictionary = JsonSerializer.Deserialize<Dictionary<string, byte[]>>(data);
            Credentials = JsonSerializer.Deserialize<List<Credential>>(gcm.Decrypt(
                dictionary["ciphertext"],
                new Rfc2898DeriveBytes(key, dictionary["salt"]).GetBytes(32),
                dictionary["nonce"],
                dictionary["tag"],
                dictionary["meta"]
            ));

            nonce = dictionary["nonce"];
        }

        public void Save()
        {
            byte[] ciphertext, tag,
                bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Credentials)),
                nonce = increment(this.nonce),
                meta = random.GetNextBytes(random.Next(0, 32)),
                salt = random.GetNextBytes(8);

            (ciphertext, tag) = gcm.Encrypt(bytes, new Rfc2898DeriveBytes(key, salt).GetBytes(32), nonce, meta);

            Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>()
            {
                { "ciphertext", ciphertext },
                { "tag", tag },
                { "nonce", nonce },
                { "meta", meta },
                { "salt", salt }
            };

            byte[] data = Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dictionary))));

            writer = File.OpenWrite(FilePath);
            writer.SetLength(0);
            writer.Flush();
            writer.Write(data, 0, data.Length);
            writer.Close();
        }

        private byte[] increment(byte[] counter)
        {
            int j = counter.Length;
            while (--j >= 0 && ++counter[j] == 0) { }
            return counter;
        }
    }
}
