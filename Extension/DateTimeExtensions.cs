using System.ComponentModel;

namespace M56X.Core.Extension
{
    /// <summary>
    /// 时间戳类型
    /// </summary>
    public enum TimestampType
    {
        /// <summary>
        /// 秒
        /// </summary>
        [Description("秒")]
        Second = 10,

        /// <summary>
        /// 毫秒
        /// </summary>
        [Description("毫秒")]
        Millisecond = 13,

        /// <summary>
        /// 微秒
        /// </summary>
        [Description("微秒")]
        Microsecond = 16
    }

    /// <summary>
    /// 时间扩展
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 计算指定时间到1970-1-1 0:0:0的总秒数
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double TotalSeconds(this DateTime value)
        {
            TimeSpan ts = value.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalSeconds;
        }

        /// <summary>
        /// 时间转为时间戳(默认13位时间戳)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static long ToTimestamp(this DateTime value, TimestampType type = TimestampType.Millisecond)
        {
            if (type == TimestampType.Second)
                return Convert.ToInt64(TotalSeconds(value));
            else if (type == TimestampType.Microsecond)
                return Convert.ToInt64(TotalSeconds(value) * 1000000);
            else
                return Convert.ToInt64(TotalSeconds(value) * 1000);
        }

        /// <summary>
        /// 时间戳转时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long value)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            var len = value.ToString().Length;

            if (len == 16)
                return dt.AddSeconds(value / 1000000);
            else if (len == 13)
                return dt.AddSeconds(value / 1000);
            else
                return dt.AddSeconds(value);
        }

        /// <summary>
        /// 转为时间字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this long value, string format = "yyyy-MM-dd HH:mm:ss")
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            var len = value.ToString().Length;

            DateTime ts;
            if (len == 16)
                ts = dt.AddSeconds(value / 1000000);
            else if (len == 13)
                ts = dt.AddSeconds(value / 1000);
            else
                ts = dt.AddSeconds(value);

            return ts.ToString(format);
        }
    }
}
