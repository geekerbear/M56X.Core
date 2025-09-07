namespace M56X.Core.Helper
{
    /// <summary>
    /// 字节序助手
    /// </summary>
    public static class EndianHelper
    {
        /// <summary>
        /// 从字节数据指定位置读取一个无符号16位整数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset">偏移</param>
        /// <param name="isLittleEndian">是否小端字节序</param>
        /// <returns></returns>
        public static ushort ToUInt16(this byte[] data, int offset = 0, bool isLittleEndian = true)
        {
            if (isLittleEndian)
                return (ushort)((data[offset + 1] << 8) | data[offset]);
            else
                return (ushort)((data[offset] << 8) | data[offset + 1]);
        }

        public static uint ToUInt32(this byte[] data, int offset = 0, bool isLittleEndian = true)
        {
            if (isLittleEndian) return BitConverter.ToUInt32(data, offset);

            // BitConverter得到小端，如果不是小端字节顺序，则倒序
            if (offset > 0) data = data.ReadBytes(offset, 4);
            if (isLittleEndian)
                return (UInt32)(data[0] | data[1] << 8 | data[2] << 0x10 | data[3] << 0x18);
            else
                return (UInt32)(data[0] << 0x18 | data[1] << 0x10 | data[2] << 8 | data[3]);
        }
    }
}
