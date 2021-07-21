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
        public const string FILE_EXTENSION = ".mnem";

        public CipherData   Data
        {
            get;
            private set;
        }
        public string       Path
        {
            get
            {
                return _new_path;
            }
            set
            {
                _new_path = value;

                if (_new_path == "")
                {
                    _new_path = UserSettings.Default.DefaultPath;
                }

                _new_path = Environment.ExpandEnvironmentVariables(_new_path);

                if (System.IO.Path.GetExtension(_new_path) == "")
                {
                    _new_path = System.IO.Path.GetDirectoryName(_new_path) + "\\";
                }

                _new_path += System.IO.Path.GetExtension(_new_path) == FILE_EXTENSION ? "" : FILE_EXTENSION;
                _new_path = System.IO.Path.IsPathFullyQualified(_new_path) ? _new_path : System.IO.Path.GetFullPath(_new_path);
            }
        }
        public string       Key
        {
            private get;
            set;
        }
        public bool         Active
        {
            get
            {
                return _active_path != "";
            }
        }

        private Cryptor     _cryptor;
        private string      _active_path;
        private string      _new_path;

        public CipherFile()
        {
            _cryptor    = new Cryptor();
            Data        = new CipherData();
            
            _active_path    = "";
            Path            = "";
            Key             = "";
        }

        public CipherFile(string path) : this()
        {
            Path = path;
        }

        public CipherFile(string path, string key) : this(path)
        {
            Key = key;
        }

        public bool Exists()
        {
            return File.Exists(Path);
        }

        public CipherFile Activate()
        {
            if (Active)
            {
                File.Delete(_active_path);
            }

            if (Exists())
            {
                _Read();
            }
            else
            {
                _Create();
            }

            return this;
        }

        public CipherFile Save()
        {
            if (!Active)
            {
                Activate();
            }

            MnemRandom random = new MnemRandom();

            byte[] ciphertext, tag,
                salt = random.GetNextBytes(8),
                nonce = random.GetNextBytes(12);

            (ciphertext, tag) = _cryptor.Encrypt(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Data)),
                new Rfc2898DeriveBytes(Key, salt).GetBytes(32),
                nonce,
                Encoding.UTF8.GetBytes(System.IO.Path.GetFileName(_active_path))
            );

            Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>()
            {
                { "ciphertext", ciphertext },
                { "tag", tag },
                { "nonce", nonce },
                { "salt", salt }
            };

            byte[] base64 = Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dictionary))));

            FileStream writer = File.OpenWrite(_active_path);
            writer.SetLength(0);
            writer.Flush();
            writer.Write(base64, 0, base64.Length);
            writer.Close();

            //_Read();

            return this;
        }

        private void _Create()
        {
            string directory = System.IO.Path.GetDirectoryName(Path);
            if (directory != "")
            {
                Directory.CreateDirectory(directory);
            }

            File.Create(Path).Close();

            _active_path = Path;

            Save();
        }

        private void _Read()
        {
            Dictionary<string, byte[]> dictionary = JsonSerializer.Deserialize<Dictionary<string, byte[]>>(Convert.FromBase64String(File.ReadAllText(Path, Encoding.UTF8)));

            Data = JsonSerializer.Deserialize<CipherData>(_cryptor.Decrypt(
                dictionary["ciphertext"],
                new Rfc2898DeriveBytes(Key, dictionary["salt"]).GetBytes(32),
                dictionary["nonce"],
                dictionary["tag"],
                Encoding.UTF8.GetBytes(System.IO.Path.GetFileName(Path))
            ));

            _active_path = Path;
        }
    }
}
