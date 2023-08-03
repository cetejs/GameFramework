using System;
using System.IO;
using System.Security.Cryptography;

namespace GameFramework
{
    public static class EncryptionUtils
    {
        public static class AES
        {
            private static readonly byte[] DefaultIV = new byte[16];

            public static string EncryptToString(string plainText, string password)
            {
                if (string.IsNullOrEmpty(plainText))
                {
                    return plainText;
                }

                byte[] cipherBytes = EncryptToBytes(plainText, password);
                return Convert.ToBase64String(cipherBytes);
            }

            public static string DecryptFromString(string cipherText, string password)
            {
                if (string.IsNullOrEmpty(cipherText))
                {
                    return cipherText;
                }

                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                return DecryptFromBytes(cipherBytes, password);
            }

            public static byte[] EncryptToBytes(string plainText, string password)
            {
                if (string.IsNullOrEmpty(plainText))
                {
                    return null;
                }

                byte[] cipherBytes;
                using (RijndaelManaged rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.IV = DefaultIV;

                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, rijAlg.IV, 32);
                    rijAlg.Key = key.GetBytes(16);

                    ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }

                            cipherBytes = msEncrypt.ToArray();
                        }
                    }
                }

                return cipherBytes;
            }

            public static string DecryptFromBytes(byte[] cipherBytes, string password)
            {
                if (cipherBytes == null || cipherBytes.Length == 0)
                {
                    return null;
                }

                string plaintext;
                using (RijndaelManaged rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.IV = DefaultIV;

                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, rijAlg.IV, 32);
                    rijAlg.Key = key.GetBytes(16);
                    ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return plaintext;
            }
        }
    }

    public enum EncryptionType
    {
        None,
        AES
    }
}