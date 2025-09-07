namespace M56X.Core.Enums
{
    /// <summary>
    /// 标签缩放类型
    /// </summary>
    public enum ScalingType
    {
        /// <summary>
        /// 无缩放
        /// </summary>
        None,
        /// <summary>
        /// 线性缩放
        /// <para>通过线性比例将原始值(RawValue)从[RawLow, RawHigh]范围映射到[ScaledLow, ScaledHigh]范围</para>
        /// <para>计算公式为：((ScaledHigh - ScaledLow)/(RawHigh - RawLow))*(RawValue - RawLow) + ScaledLow</para>
        /// </summary>
        Linear,
        /// <summary>
        /// 平方根缩放(优化局部敏感性与分布形态)
        /// <para>先对输入值进行归一化处理，然后取平方根，最后线性映射到目标范围</para>
        /// <para>计算公式为：Math.Sqrt((RawValue - RawLow)/(RawHigh - RawLow))*(ScaledHigh - ScaledLow) + ScaledLow</para>
        /// </summary>
        SquareRoot
    }
}
