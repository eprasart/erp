using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace ERP
{
    public static class Security
    {
        public static byte[] salt = Encoding.ASCII.GetBytes("eI%3SOD2l^eL/.&f$");
        public static string encryptionPassword = "Ie;jaeofLie$q9SUC-3$%293";

        public static string EncryptString(string textToEncrypt)
        {
            var algorithm = GetAlgorithm(encryptionPassword);
            byte[] encryptedBytes;

            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV))
            {
                byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(textToEncrypt);
                encryptedBytes = InMemoryCrypt(bytesToEncrypt, encryptor);
            }
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string DecryptString(string encryptedText)
        {
            if (String.IsNullOrEmpty(encryptedText)) return "";

            var algorithm = GetAlgorithm(encryptionPassword);
            byte[] descryptedBytes;

            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                descryptedBytes = InMemoryCrypt(encryptedBytes, decryptor);
            }
            return Encoding.UTF8.GetString(descryptedBytes);
        }

        // Performs an in-memory encrypt/decrypt transformation on a byte array.
        private static byte[] InMemoryCrypt(byte[] data, ICryptoTransform transform)
        {
            MemoryStream memory = new MemoryStream();
            using (Stream stream = new CryptoStream(memory, transform, CryptoStreamMode.Write))
            {
                stream.Write(data, 0, data.Length);
            }
            return memory.ToArray();
        }

        // Defines a RijndaelManaged algorithm and sets its key and Initialization Vector (IV) 
        // values based on the encryptionPassword received.
        private static RijndaelManaged GetAlgorithm(string encryptionPassword)
        {
            // Create an encryption key from the encryptionPassword and salt.
            var key = new Rfc2898DeriveBytes(encryptionPassword, salt);

            // Declare that we are going to use the Rijndael algorithm with the key that we've just got.
            var algorithm = new RijndaelManaged();

            int bytesForKey = algorithm.KeySize / 8;
            int bytesForIV = algorithm.BlockSize / 8;

            algorithm.Key = key.GetBytes(bytesForKey);
            algorithm.IV = key.GetBytes(bytesForIV);

            return algorithm;
        }
    }
}
