using M56X.Core.Enums;

namespace M56X.Core.Helper
{
    /// <summary>
    /// 缩放助手
    /// </summary>
    public class ScaleHelper
    {
        /// <summary>
        /// 正向缩放处理
        /// </summary>
        /// <param name="rawValue">原始值</param>
        /// <param name="rawLow">原始值下限</param>
        /// <param name="rawHigh">原始值上限</param>
        /// <param name="scaledLow">缩放后下限</param>
        /// <param name="scaledHigh">缩放后上限</param>
        /// <param name="scalingType">缩放类型</param>
        /// <returns>缩放后的值</returns>
        public static double ScaleValue(double rawValue, double rawLow, double rawHigh,
                                      double scaledLow, double scaledHigh, ScalingType scalingType)
        {
            if (scalingType == ScalingType.Linear)
            {
                return ((scaledHigh - scaledLow) / (rawHigh - rawLow)) * (rawValue - rawLow) + scaledLow;
            }
            else if (scalingType == ScalingType.SquareRoot)
            {
                double normalized = (rawValue - rawLow) / (rawHigh - rawLow);
                return Math.Sqrt(normalized) * (scaledHigh - scaledLow) + scaledLow;
            }
            return rawValue;
        }

        /// <summary>
        /// 逆向缩放处理
        /// </summary>
        /// <param name="scaledValue">缩放后的值</param>
        /// <param name="rawLow">原始值下限</param>
        /// <param name="rawHigh">原始值上限</param>
        /// <param name="scaledLow">缩放后下限</param>
        /// <param name="scaledHigh">缩放后上限</param>
        /// <param name="scalingType">缩放类型</param>
        /// <returns>原始值</returns>
        public static double ReverseScaleValue(double scaledValue, double rawLow, double rawHigh,
                                         double scaledLow, double scaledHigh, ScalingType scalingType)
        {
            if (scalingType == ScalingType.Linear)
            {
                return ((scaledValue - scaledLow) / (scaledHigh - scaledLow)) * (rawHigh - rawLow) + rawLow;
            }
            else if (scalingType == ScalingType.SquareRoot)
            {
                double normalized = (scaledValue - scaledLow) / (scaledHigh - scaledLow);
                return (normalized * normalized) * (rawHigh - rawLow) + rawLow;
            }
            return scaledValue;
        }
    }
}
