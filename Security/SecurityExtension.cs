using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace M56X.Core.Security
{
    public static class SecurityExtension
    {
        #region MD5
        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="data"></param>
        /// <param name="toUpper">是否大写输出</param>
        /// <returns></returns>
        public static string MD5(this byte[] data, bool toUpper = false)
        {
            byte[] hash = System.Security.Cryptography.MD5.HashData(data);
            var result = Convert.ToHexStringLower(hash);
            if (toUpper) return result.ToUpper();
            return result;
        }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="data"></param>
        /// <param name="toUpper">是否大写输出</param>
        /// <param name="encoding">默认: UTF8</param>
        /// <returns></returns>
        public static string MD5(this string data, bool toUpper = false, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            byte[] hash = System.Security.Cryptography.MD5.HashData(encoding.GetBytes(data));
            var result = Convert.ToHexStringLower(hash);
            if (toUpper) return result.ToUpper();
            return result;
        }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="toUpper">是否大写输出</param>
        /// <returns></returns>
        public static string MD5(this Stream stream, bool toUpper = false)
        {
            byte[] hash = System.Security.Cryptography.MD5.HashData(stream);
            var result = Convert.ToHexStringLower(hash);
            if (toUpper) return result.ToUpper();
            return result;
        }

        /// <summary>
        /// 文件哈希计算
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string MD5(this FileInfo file, bool toUpper = false)
        {
            using var fs = file.OpenRead();
            byte[] hash = System.Security.Cryptography.MD5.HashData(fs);
            var result = Convert.ToHexStringLower(hash);
            if (toUpper) return result.ToUpper();
            return result;
        }
        #endregion

        #region Crc16、Crc32
        /// <summary>
        /// Crc
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static uint Crc(this byte[] data) => new Crc32().Update(data).Value;

        /// <summary>
        /// Crc16
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ushort Crc16(this byte[] data) => new Crc16().Update(data).Value;
        #endregion

        #region SHA128、SHA256、SHA384、SHA512
        /// <summary>
        /// SHA128
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] SHA128(this byte[] data, byte[] key) => new HMACSHA1(key).ComputeHash(data);

        /// <summary>
        /// SHA256
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] SHA256(this byte[] data, byte[] key) => new HMACSHA256(key).ComputeHash(data);

        /// <summary>
        /// SHA384
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] SHA384(this byte[] data, byte[] key) => new HMACSHA384(key).ComputeHash(data);

        /// <summary>
        /// SHA512
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] SHA512(this byte[] data, byte[] key) => new HMACSHA512(key).ComputeHash(data);
        #endregion

        /// <summary>
        /// Aes加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] AesEncrypt(this byte[] plaintext, byte[] key, byte[] iv) => Aes.Encrypt(plaintext, key, iv);

        /// <summary>
        /// Aes解密
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] AesDecrypt(this byte[] ciphertext, byte[] key, byte[] iv) => Aes.Decrypt(ciphertext, key, iv);

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static string Base64Encrypt(this byte[] plaintext)
        {
            return Convert.ToBase64String(plaintext);
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="plaintext"></param>
        /// <returns></returns>
        public static string Base64Encrypt(this string plaintext)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext));
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <returns></returns>
        public static byte[] Base64Decrypt(this string ciphertext)
        {
            return Convert.FromBase64String(ciphertext);
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Base64Decrypt(this string ciphertext, Encoding? encoding = null)
        {
            var byteArr = Convert.FromBase64String(ciphertext);
            encoding ??= Encoding.UTF8;
            return encoding.GetString(byteArr);
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UrlEncode(this string url, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return HttpUtility.UrlEncode(url, encoding);
        }

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UrlDecode(this string url, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return HttpUtility.UrlDecode(url, encoding);
        }
    }
}
