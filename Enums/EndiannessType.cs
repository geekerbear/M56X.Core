namespace M56X.Core.Enums
{
    /// <summary>
    /// 字节顺序类型
    /// </summary>
    public enum EndiannessType
    {
        /// <summary>
        /// 默认
        /// </summary>
        DEFAULT = 0,

        /// <summary>
        /// 小端,(LittleEndian)Windows默认
        /// 格式: HGFEDCBA DCBA BA
        /// </summary>
        DCBA = 1,

        /// <summary>
        /// 大端,(BigEndian)
        /// 格式: AB ABCD ABCDEFGH
        /// </summary>
        ABCD = 2,

        /// <summary>
        /// 小端字节交换(LittleEndianByteSwap)
        /// 格式: GHEFCDAB CDAB
        /// </summary>
        CDAB = 3,

        /// <summary>
        /// 大端字节交换(BigEndianByteSwap)
        /// 格式: BADCFEHG BADC
        /// </summary>
        BADC = 4
    }
}
