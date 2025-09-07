using System.Text;

namespace M56X.Core.Helper
{
    /// <summary>
    /// 位和字节操作扩展
    /// </summary>
    public static class BitByteHelper
    {
        /// <summary>
        /// 系统是否为小端字节序
        /// </summary>
        /// <returns></returns>
        public static bool IsLittleEndian()
        {
            return BitConverter.IsLittleEndian;
        }

        /// <summary>
        /// 反转字节数组顺序
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns>
        /// <para>如果输入为小端,转换后为大端</para>
        /// <para>如果输入为大端,转换后为小端</para>
        /// </returns>
        public static byte[] Reverse(this byte[] bytes)
        {
            Array.Reverse(bytes);
            return bytes;
        }

        #region 字节转二进制字符串
        /// <summary>
        /// 无符号字节转二进制字符串
        /// <para>例如: 3 转换后-> 00000011</para>
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="zeroFill">是否反转顺序</param>
        /// <param name="zeroFill">是否左边补0</param>
        /// <returns></returns>
        public static string ToBinary(this byte value, bool reverse‌ = false, bool zeroFill = true)
        {
            string result;
            if (zeroFill)
                result = Convert.ToString(value, 2).PadLeft(8, '0');
            else
                result = Convert.ToString(value, 2);

            if (reverse)
            {
                char[] charArray = result.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }
            return result;
        }

        /// <summary>
        /// 有符号字节转二进制字符串
        /// <para>例如: -1 转换后-> 10000001 左边第一位是符号为</para>
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="zeroFill">是否反转顺序</param>
        /// <param name="zeroFill">是否左边补0</param>
        /// <returns></returns>
        public static string ToBinary(this sbyte value, bool reverse‌ = false, bool zeroFill = true)
        {
            byte byteValue = (byte)value;
            return byteValue.ToBinary(reverse, zeroFill);
        }

        /// <summary>
        /// 字节数组转二进制字符串
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string ToBinary(this byte[] bytes, string separator = " ")
        {
            List<string> result = [];
            foreach (byte b in bytes)
            {
                result.Add(b.ToBinary());
            }
            return string.Join(separator, result);
        }

        public static string ToBinary(this ushort value, string separator = " ")
        {
            List<string> result = [];
            var bytes = value.ToBytes();
            foreach (byte b in bytes)
            {
                result.Add(b.ToBinary());
            }
            return string.Join(separator, result);
        }
        #endregion

        #region 复制字节数组
        /// <summary>
        /// 获取实际长度
        /// <para>源长度-偏移量 < 要获取的长度 = 源长度-偏移量</para>
        /// <para>源长度-偏移量 >= 要获取的长度 = 要获取的长度</para>
        /// </summary>
        /// <param name="buffer">字节数据</param>
        /// <param name="offset">偏移数</param>
        /// <param name="count">要获取的长度</param>
        /// <returns></returns>
        private static long GetRealLength(byte[] buffer, long offset, long count)
        {
            if (buffer.Length - offset < count)
                return buffer.Length - offset;
            else
                return count;
        }

        /// <summary>
        /// 复制字节数组
        /// <para>适用场景: 超大数据(GB级)复制, 高频调用且性能瓶颈明确的场景</para>
        /// <para>直接操作内存指针，‌性能最高但要注意安全性</para>
        /// </summary>
        /// <param name="source">原数据</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">要获取的长度</param>
        /// <returns></returns>
        public static byte[] ReadBytesByUnsafe(this byte[] source, long offset, long count)
        {
            unsafe
            {
                long len = GetRealLength(source, offset, count);
                byte[] destination = new byte[len];

                fixed (byte* srcPtr = source, destPtr = destination)
                {
                    Buffer.MemoryCopy(
                        srcPtr + offset,  // 源指针偏移
                        destPtr,          // 目标起始指针
                        destination.Length * sizeof(byte), // 目标内存总大小
                        len         // 需复制的字节数
                    );
                }
                return destination;
            }
        }

        /// <summary>
        /// 复制字节数组
        /// <para>适用场景: 适合高性能或低GC压力场景</para>
        /// <para>实现‌无额外内存分配‌的复制，支持对数组子集的直接复制</para>
        /// </summary>
        /// <param name="source">原数据</param>
        /// <param name="offset">偏移量</param>
        /// <param name="count">要获取的长度</param>
        /// <returns></returns>
        public static byte[] ReadBytesBySpan(this byte[] source, int offset, int count)
        {
            int len = (int)GetRealLength(source, offset, count);
            byte[] destination = new byte[len];

            source.AsSpan(offset, len).CopyTo(destination.AsSpan());
            return destination;
        }

        /// <summary>
        /// 复制字节数组
        /// <para>适用场景: 适合大数组或高频操作场景</para>
        /// <para>直接操作字节块，避开类型安全检查，性能极优</para>
        /// </summary>
        /// <param name="source">原数据</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">要获取的长度</param>
        /// <returns></returns>
        public static byte[] ReadBytes(this byte[] source, int offset, int count)
        {
            if (count == 0) return [];
            int len = (int)GetRealLength(source, offset, count);
            byte[] destination = new byte[len];
            Buffer.BlockCopy(source, offset, destination, 0, destination.Length);
            return destination;
        }
        #endregion

        /// <summary>
        /// 两两字节交换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] SwapBytes(this byte[] value)
        {
            if (value.Length % 2 != 0)
                throw new ArgumentException("Input length must be even");

            byte[] output = new byte[value.Length];

            for (int i = 0; i < value.Length; i += 2)
            {
                output[i] = value[i + 1];
                output[i + 1] = value[i];
            }
            return output;
        }

        #region 输出Hex字符串
        /// <summary>
        /// 输出Hex
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="toUpper"></param>
        /// <returns></returns>
        public static string ToHex(this byte value, bool toUpper = true)
        {
            if (toUpper) return value.ToString("X2");
            return value.ToString("x2");
        }

        /// <summary>
        /// 格式化输出Hex字符串
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="separator">分隔符, 默认: 空格</param>
        /// <param name="prefix">前缀, 默认: 空</param>
        /// <param name="toUpper">是否为大写</param>
        /// <returns>格式化后的字符串<para>默认输出格式: 0x12, 0x34, 0x56, 0x78</para></returns>
        public static string ToHex(this byte[] bytes, int offset = 0, int count = -1, string separator = " ", string prefix = "", bool toUpper = true)
        {
            if (bytes == null || bytes.Length == 0) return string.Empty;
            if (count < 0) count = bytes.Length - offset;
            if (offset < 0 || offset + count > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取数量大于可获取数量");

            string format = toUpper ? "X2" : "x2";
            var hex = new StringBuilder(count * (prefix.Length + 2 + separator.Length));

            for (int i = offset; i < offset + count; i++)
            {
                hex.Append(prefix);
                hex.Append(bytes[i].ToString(format));
                if (i < offset + count - 1 && !string.IsNullOrEmpty(separator))
                    hex.Append(separator);
            }
            return hex.ToString();
        }
        #endregion

        #region 值转为字节数组
        /// <summary>
        /// 有符号16位整型转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this short value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 有符号16位整型数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转数据集合中的每个元素</param>
        /// <returns></returns>
        public static byte[] ToBytes(this short[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * sizeof(short)];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * sizeof(short), temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 无符号16位整型转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this ushort value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 无符号16位整型数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this ushort[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * sizeof(ushort)];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * sizeof(ushort), temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 有符号32位整型转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this int value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 有符号32位整型数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this int[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * sizeof(int)];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * sizeof(int), temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 无符号32位整型转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this uint value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 无符号32位整型数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this uint[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * sizeof(uint)];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * sizeof(uint), temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 有符号64位整型转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this long value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 有符号64位整型数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this long[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * sizeof(long)];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * sizeof(long), temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 无符号64位整型转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this ulong value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 无符号64位整型数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this ulong[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * sizeof(ulong)];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * sizeof(ulong), temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 有符号128位整型转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this Int128 value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 有符号128位整型数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this Int128[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * 16];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * 16, temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 无符号128位整型转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this UInt128 value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 无符号128位整型数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this UInt128[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * 16];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * 16, temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 半精度浮点数转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this Half value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 半精度浮点数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this Half[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * 2];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * 2, temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 单精度浮点数转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this float value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 单精度浮点数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns>根据系统字节序输出</returns>
        public static byte[] ToBytes(this float[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * sizeof(float)];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * sizeof(float), temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 双精度浮点数转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this double value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 双精度浮点数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns>根据系统字节序输出</returns>
        public static byte[] ToBytes(this double[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * sizeof(double)];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * sizeof(double), temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// bool转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns></returns>
        public static byte[] ToBytes(this bool value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// bool数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>根据系统字节序输出</returns>
        public static byte[] ToBytes(this bool[] value)
        {
            byte[] bytes = new byte[value.Length * sizeof(bool)];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes();
                Buffer.BlockCopy(temp, 0, bytes, i * sizeof(bool), temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 字符转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns></returns>
        public static byte[] ToBytes(this char value, bool reverse = false)
        {
            var result = BitConverter.GetBytes(value);
            if (reverse) Array.Reverse(result);
            return result;
        }

        /// <summary>
        /// 字符数组转字节数组
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="reverse">是否反转</param>
        /// <returns>根据系统字节序输出</returns>
        public static byte[] ToBytes(this char[] value, bool reverse = false)
        {
            byte[] bytes = new byte[value.Length * sizeof(char)];
            for (int i = 0; i < value.Length; i++)
            {
                byte[] temp = value[i].ToBytes(reverse);
                Buffer.BlockCopy(temp, 0, bytes, i * sizeof(char), temp.Length);
            }
            return bytes;
        }

        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value)
        {
            return ToBytes(value, Encoding.UTF8);
        }


        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value, Encoding encoding)
        {
            return encoding.GetBytes(value);
        }
        #endregion

        #region 从字节数组中读取值
        /// <summary>
        /// 从指定位置读取一个有符号16位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static short ReadInt16(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = sizeof(short);
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToInt16(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组有符号16位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static short[] ReadInt16(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = sizeof(short);
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            short[] result = new short[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToInt16(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个无符号16位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static ushort ReadUInt16(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = sizeof(ushort);
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToUInt16(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组无符号16位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ushort[] ReadUInt16(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = sizeof(ushort);
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            ushort[] result = new ushort[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToUInt16(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个有符号32位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static int ReadInt32(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = sizeof(int);
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToInt32(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组有符号32位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int[] ReadInt32(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = sizeof(int);
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            int[] result = new int[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToInt32(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个无符号32位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static uint ReadUInt32(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = sizeof(uint);
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToUInt32(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组无符号32位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static uint[] ReadUInt32(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = sizeof(uint);
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            uint[] result = new uint[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToUInt32(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个有符号64位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static long ReadInt64(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = sizeof(long);
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToInt64(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组有符号64位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static long[] ReadInt64(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = sizeof(long);
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            long[] result = new long[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToInt64(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个无符号64位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static ulong ReadUInt64(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = sizeof(ulong);
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToUInt64(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组无符号64位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static ulong[] ReadUInt64(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = sizeof(ulong);
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            ulong[] result = new ulong[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToUInt64(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个有符号128位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static Int128 ReadInt128(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = 16;
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToInt128(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组有符号128位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Int128[] ReadInt128(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = 16;
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            Int128[] result = new Int128[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToInt128(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个无符号128位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static UInt128 ReadUInt128(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = 16;
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToUInt128(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组无符号128位整数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static UInt128[] ReadUInt128(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = 16;
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            UInt128[] result = new UInt128[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToUInt128(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个半精度浮点数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static Half ReadHalf(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = 2;
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToHalf(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组半精度浮点数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Half[] ReadHalf(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = 2;
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            Half[] result = new Half[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToHalf(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个单精度浮点数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static float ReadFloat(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = sizeof(float);
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToSingle(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组单精度浮点数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static float[] ReadFloat(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = sizeof(float);
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            float[] result = new float[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToSingle(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个双精度浮点数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static double ReadDouble(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = sizeof(double);
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToDouble(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组双精度浮点数
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static double[] ReadDouble(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = sizeof(double);
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            double[] result = new double[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToDouble(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个布尔值
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <returns></returns>
        public static bool ReadBoolean(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToBoolean(bytes, startIndex);
        }

        /// <summary>
        /// 从指定位置读取一组布尔值
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool[] ReadBoolean(this byte[] bytes, int startIndex = 0, int count = 1)
        {
            int size = sizeof(bool);
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            bool[] result = new bool[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                result[i] = BitConverter.ToBoolean(bytes, byteOffset);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取一个字符
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        public static char ReadChar(this byte[] bytes, int startIndex = 0, bool reverse = false)
        {
            int size = sizeof(char);
            byte[] tmp = new byte[size];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, size);
            if (reverse)
                Array.Reverse(tmp);
            return BitConverter.ToChar(tmp, 0);
        }

        /// <summary>
        /// 从指定位置读取一组字符
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="count">获取个数</param>
        /// <param name="reverse">是否反转元素</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static char[] ReadChar(this byte[] bytes, int startIndex = 0, int count = 1, bool reverse = false)
        {
            int size = sizeof(char);
            // 参数校验（确保起始位置和长度有效）
            if (startIndex + count * size > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "获取的长度大于可获取的长度");

            char[] result = new char[count];

            for (int i = 0; i < count; i++)
            {
                int byteOffset = startIndex + i * size;
                byte[] tmp = new byte[size];
                Buffer.BlockCopy(bytes, byteOffset, tmp, 0, size);
                if (reverse)
                    Array.Reverse(tmp);
                result[i] = BitConverter.ToChar(tmp, 0);
            }
            return result;
        }

        /// <summary>
        /// 从指定位置读取字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ReadString(this byte[] bytes, int startIndex, int length)
        {
            return ReadString(bytes, startIndex, length, Encoding.UTF8);
        }

        /// <summary>
        /// 字节数组转字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ReadString(this byte[] bytes, int startIndex, int length, Encoding encoding)
        {
            return encoding.GetString(bytes, startIndex, length);
        }
        #endregion

        #region 按位设置或获取值
        /// <summary>
        /// 设置字节位的值
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="bitPosition">第几位</param>
        /// <param name="isSet"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static byte SetBitValue(this byte data, int bitPosition, bool value)
        {
            if (bitPosition < 0 || bitPosition > 7)
                throw new ArgumentOutOfRangeException(nameof(bitPosition), "位索引必须为 0~7");

            byte mask = (byte)(1u << bitPosition);
            return value ? (byte)(data | mask) : (byte)(data & ~mask);
        }

        /// <summary>
        /// 设置字节位的值
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="bitPosition">第几位</param>
        /// <param name="isSet"></param>
        /// <returns></returns>
        public static ushort SetBitValue(this ushort data, int bitPosition, bool value)
        {
            if (bitPosition < 0 || bitPosition > 15)
                throw new ArgumentOutOfRangeException(nameof(bitPosition), "位索引必须为 0~15");

            ushort mask = (ushort)(1u << bitPosition);
            return value ? (ushort)(data | mask) : (ushort)(data & ~mask);
        }

        /// <summary>
        /// 设置字节位的值
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="bitPosition">第几位</param>
        /// <param name="isSet"></param>
        /// <returns></returns>
        public static uint SetBitValue(this uint data, int bitPosition, bool value)
        {
            if (bitPosition < 0 || bitPosition > 31)
                throw new ArgumentOutOfRangeException(nameof(bitPosition), "位索引必须为 0~31");

            uint mask = (uint)(1u << bitPosition);
            return value ? (uint)(data | mask) : (uint)(data & ~mask);
        }

        /// <summary>
        /// 获取字节位的值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bitPosition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool GetBitValue(this byte data, int bitPosition)
        {
            if (bitPosition < 0 || bitPosition > 7)
                throw new ArgumentOutOfRangeException(nameof(bitPosition), "位索引必须为 0~7");

            byte mask = (byte)(1u << bitPosition);
            return (data & mask) != 0;
        }

        /// <summary>
        /// 获取字节位的值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bitPosition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool GetBitValue(this ushort data, int bitPosition)
        {
            if (bitPosition < 0 || bitPosition > 15)
                throw new ArgumentOutOfRangeException(nameof(bitPosition), "位索引必须为 0~15");

            ushort mask = (ushort)(1u << bitPosition);
            return (data & mask) != 0;
        }

        /// <summary>
        /// 获取字节位的值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bitPosition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool GetBitValue(this uint data, int bitPosition)
        {
            if (bitPosition < 0 || bitPosition > 31)
                throw new ArgumentOutOfRangeException(nameof(bitPosition), "位索引必须为 0~31");

            uint mask = (uint)(1u << bitPosition);
            return (data & mask) != 0;
        }

        /// <summary>
        /// ushort转Span<bool>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Span<bool> ToBoolSpan(this ushort value)
        {
            Span<bool> result = new bool[16];
            for (int i = 0; i < 16; i++)
            {
                result[i] = (value & (1 << i)) != 0;
            }
            return result;
        }

        /// <summary>
        /// uint转Span<bool>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Span<bool> ToBoolSpan(this uint value)
        {
            Span<bool> result = new bool[32]; // uint有32位
            for (int i = 0; i < 32; i++)
            {
                result[i] = (value & (1 << i)) != 0; // 检查每一位是否为1
            }
            return result;
        }
        #endregion

        #region 异或计算
        /// <summary>
        /// 对两个长度相等的字节数组进行逐字节异或运算
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static unsafe byte[] Xor(byte[] a, byte[] b)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);

            if (a.Length != b.Length)
                throw new ArgumentException("数组长度必须相同");

            byte[] result = new byte[a.Length];
            fixed (byte* ptrA = a, ptrB = b, ptrResult = result)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    ptrResult[i] = (byte)(ptrA[i] ^ ptrB[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// 异或(两次异或可以还原)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static unsafe byte[] Xor(this byte[] data, byte key)
        {
            ArgumentNullException.ThrowIfNull(data);
            byte[] result = new byte[data.Length];
            fixed (byte* ptr = data, ptrResult = result)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    ptrResult[i] = (byte)(ptr[i] ^ key); // 直接操作内存地址
                }
            }
            return result;
        }
        #endregion

        #region 校验
        /// <summary>
        /// 累加校验和
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte Checksum(this byte[] data)
        {
            int sum = 0;
            foreach (byte b in data)
            {
                sum = (sum + b) % 0xFFFF; // 取16位模
            }
            return (byte)(sum & 0xFF);
        }

        /// <summary>
        /// 双字节校验和
        /// <para>场景: 网络协议(如TCP校验和)</para>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] DoubleChecksum(this byte[] data)
        {
            int sum = 0;
            foreach (byte b in data)
            {
                sum = (sum + b) % 0xFFFF;
            }
            return [
                (byte)((sum & 0xFF00) >> 8), // 高8位
                (byte)(sum & 0xFF)           // 低8位
            ];
        }

        /// <summary>
        /// 累加后取反加一生成补码校验值
        /// <para>场景: 硬件协议校验(如工业设备通信)</para>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte ComplementChecksum(this byte[] data)
        {
            int sum = data.Sum(b => (int)b);
            sum = ~sum + 1; // 补码操作
            return (byte)(sum & 0xFF);
        }

        /// <summary>
        /// BCC校验(异或校验)
        /// <para>场景: 数据快速校验(如传感器数据)</para>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte BCC(this byte[] data)
        {
            byte xor = 0;
            foreach (byte b in data)
            {
                xor ^= b;
            }
            return xor;
        }
        #endregion
    }
}
