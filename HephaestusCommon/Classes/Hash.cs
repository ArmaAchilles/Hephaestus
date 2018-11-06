using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HephaestusCommon.Classes
{
    public class Hash
    {
        public Hash()
        {
            Sha1 = "";
        }
        
        public Hash(string directoryName)
        {
            Sha1 = HashDirectory(directoryName);
        }

        public string Sha1 { get; set; }

        private static string HashDirectory(string directory)
        {
            string[] files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                .OrderBy(file => file).ToArray();

            using (SHA1 sha1 = SHA1.Create())
            {
                foreach (string file in files)
                {
                    byte[] pathBytes = Encoding.UTF8.GetBytes(file);
                    sha1.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                    byte[] contentBytes = File.ReadAllBytes(file);
                    sha1.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                }

                sha1.TransformFinalBlock(new byte[0], 0, 0);

                return BitConverter.ToString(sha1.Hash).Replace("-", "").ToLower();
            }
        }
    }
}