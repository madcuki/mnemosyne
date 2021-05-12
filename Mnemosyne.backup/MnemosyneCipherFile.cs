using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Mnemosyne
{
    class MnemosyneCipherFile
    {
        public const string DEFAULT_FILENAME = "mnemosyne_cipher.txt";
        public const int BYTE_LENGTH = 1024;

        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;
            }
        }
        public string Directory
        {
            get
            {
                return directory;
            }
            set
            {
                directory = value;
            }
        }
        public string FullPath
        {
            get
            {
                return string.Concat(Directory, Filename);
            }
        }
        public List<Credential> Credentials
        {
            get
            {
                return credentials;
            }
            set
            {
                credentials = value;
            }
        }

        private string filename;
        private string directory;

        private FileStream writer;
        private FileStream reader;

        private List<Credential> credentials;

        private AesCryptoServiceProvider provider;

        public MnemosyneCipherFile(string filename = "", string directory = "")
        {
            Filename = (filename == "") ? DEFAULT_FILENAME : filename;
            Directory = directory;
        }

        public bool Exists()
        {
            return File.Exists(FullPath);
        }

        public bool Create()
        {
            if (!Exists())
            {
                FileStream fs =  File.Create(FullPath);
                return fs != null;
            }
            return false;
        }

        public void Save(List<Credential> credentials)
        {
            CryptoStream stream = new CryptoStream(File.OpenWrite(FullPath), new ToBase64Transform(), CryptoStreamMode.Write);
            byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(credentials));
            byte[][] byteChunks = new byte[(int)(bytes.Length / BYTE_LENGTH)][];

            if (Exists())
            {
                writer.SetLength(0);
                writer.Flush();
                writer.Write(bytes, 0, BYTE_LENGTH);
            }
            else
            {
                Create();
                Save(credentials);
            }
        }

        /*public List<Credential> Read()
        {
            reader = File.OpenRead(FullPath);

            return JsonSerializer.Deserialize(reader.Read());
        }*/
    }
}
