using System.Security.Cryptography;
using System.Text;

namespace M56X.Core.Security
{
    public class Aes
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] bytes, byte[] key, byte[] iv)
        {
            using System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using ICryptoTransform encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="key">长度16</param>
        /// <param name="iv">长度16</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] bytes, byte[] key, byte[] iv)
        {
            using System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
        }
    }
}
