using System.Text.Json.Nodes;

namespace M56X.Core.Extension
{
    public static class ConvertExtensions
    {
        /// <summary>
        ///  转object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToObject(this object value)
        {
            return value;
        }

        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TObject? ToObject<TObject>(this object? value, IFormatProvider? provider= null)
        {
            var result = Convert.ChangeType(value, typeof(TObject), provider);
            if (result != null)
                return (TObject)result;
            else
                return default;
        }

        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TObject? ToObject<TObject>(this object? value, TObject defaultValue, IFormatProvider? provider = null)
        {
            var result = Convert.ChangeType(value, typeof(TObject), provider);
            if (result != null)
                return (TObject)result;
            else
                return defaultValue;
        }

        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="value"></param>
        /// <param name="firstToString"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TObject? ToObject<TObject>(this object? value, bool firstToString, IFormatProvider? provider = null)
        {
            object? result = default;
            if (firstToString)
            {
                var _value = value?.ToString();
                if(!string.IsNullOrEmpty(_value))
                    result = Convert.ChangeType(_value, typeof(TObject), provider);
            }
            else
            {
                result = Convert.ChangeType(value, typeof(TObject), provider);
            }
                
            if (result != null)
                return (TObject)result;
            else
                return default;
        }

        /// <summary>
        /// 转换数据类型
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="value"></param>
        /// <param name="firstToString"></param>
        /// <param name="defaultValue"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TObject? ToObject<TObject>(this object? value, bool firstToString, TObject defaultValue, IFormatProvider? provider = null)
        {
            object? result = default;
            if (firstToString)
            {
                var _value = value?.ToString();
                if (!string.IsNullOrEmpty(_value))
                    result = Convert.ChangeType(_value, typeof(TObject), provider);
            }
            else
            {
                result = Convert.ChangeType(value, typeof(TObject), provider);
            }

            if (result != null)
                return (TObject)result;
            else
                return defaultValue;
        }
    }
}
