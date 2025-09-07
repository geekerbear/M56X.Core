using System.Text;

namespace M56X.Core.Extension
{
    public static class StringExtensions
    {
        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding">字符串编码，默认UTF8</param>
        /// <returns></returns>
        public static byte[] StringToBytes(this string value, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetBytes(value);
        }

        /// <summary>
        /// 字节数组转字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding">字符串编码，默认UTF8</param>
        /// <returns></returns>
        public static string BytesToString(this byte[] value, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetString(value);
        }
    }
}
