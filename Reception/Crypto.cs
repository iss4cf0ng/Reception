using System.Security.Cryptography;
using System.Text;
using System;

public class Crypto
{
    public static string[] CreateRSAKey()
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.KeySize = 2048;

        string publicKey = rsa.ToXmlString(false);
        string privateKey = rsa.ToXmlString(true);

        return new string[] { publicKey, privateKey };
    }

    public static byte[] RSAEncrypt(string data, string publicKey)
    {
        RSACryptoServiceProvider rsa_public = new RSACryptoServiceProvider();
        rsa_public.KeySize = 2048;
        rsa_public.FromXmlString(publicKey);

        byte[] encrypted_value = rsa_public.Encrypt(Encoding.UTF8.GetBytes(data), false);
        return encrypted_value;
    }

    public static byte[] RSADecrypt(byte[] data, string privateKey)
    {
        RSACryptoServiceProvider rsa_privae = new RSACryptoServiceProvider();
        rsa_privae.KeySize = 2048;
        rsa_privae.FromXmlString(privateKey);

        byte[] decrypted_value = rsa_privae.Decrypt(data, false);
        return decrypted_value;
    }

    public static string AESEncrypt(string plainText, byte[] secret_key, byte[] iv)
    {
        byte[] encrypted;
        using (Aes aes = Aes.Create())
        {
            aes.Mode = CipherMode.CBC;
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Key = secret_key;
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    encrypted = ms.ToArray();
                }
            }
        }
        return Convert.ToBase64String(encrypted);
    }

    public static string AESDecrypt(byte[] cipher_bytes, byte[] key, byte[] iv)
    {
        string plainText = null;
        using (Aes aes = Aes.Create())
        {
            aes.Mode = CipherMode.CBC;
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(cipher_bytes))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        plainText = sr.ReadToEnd();
                    }
                }
            }
        }
        return plainText;
    }
}