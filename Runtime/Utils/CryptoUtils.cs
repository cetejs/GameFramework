using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace GameFramework
{
    public static class CryptoUtils
    {
        public static class Aes
        {
            private static readonly byte[] DefaultIV = new byte[16];

            public static byte[] EncryptBytesToBytes(byte[] plainBytes, string password)
            {
                if (plainBytes == null || plainBytes.Length == 0)
                {
                    return null;
                }

                byte[] cipherBytes = null;
                try
                {
                    using (RijndaelManaged rijAlg = new RijndaelManaged())
                    {
                        rijAlg.Mode = CipherMode.CBC;
                        rijAlg.Padding = PaddingMode.PKCS7;
                        rijAlg.IV = DefaultIV;

                        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, rijAlg.IV, 32);
                        rijAlg.Key = key.GetBytes(16);

                        ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                        cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                return cipherBytes;
            }

            public static byte[] DecryptBytesFromBytes(byte[] cipherBytes, string password)
            {
                if (cipherBytes == null || cipherBytes.Length == 0)
                {
                    return null;
                }

                byte[] plainBytes = null;
                try
                {
                    using (RijndaelManaged rijAlg = new RijndaelManaged())
                    {
                        rijAlg.Mode = CipherMode.CBC;
                        rijAlg.Padding = PaddingMode.PKCS7;
                        rijAlg.IV = DefaultIV;

                        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, rijAlg.IV, 32);
                        rijAlg.Key = key.GetBytes(16);
                        ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                        plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                return plainBytes;
            }

            public static byte[] EncryptStringToBytes(string plainText, string password)
            {
                if (string.IsNullOrEmpty(plainText))
                {
                    return null;
                }

                byte[] cipherBytes = null;
                try
                {
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
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                return cipherBytes;
            }

            public static string DecryptStringFromBytes(byte[] cipherBytes, string password)
            {
                if (cipherBytes == null || cipherBytes.Length == 0)
                {
                    return null;
                }

                string plainText = null;
                try
                {
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
                                    plainText = srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

                return plainText;
            }
        }
    }

    public enum CryptoType
    {
        None,
        AES
    }
}