using System.Diagnostics.CodeAnalysis;

namespace M56X.Core.Config
{
    /// <summary>
    /// 配置文件读写接口
    /// </summary>
    /// <typeparam name="TSetting"></typeparam>
    public interface ISettingOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] out TSetting> where TSetting : class, ISettingFile, new()
    {
        /// <summary>
        /// 当前值
        /// </summary>
        TSetting Value { get; }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="applyChanges"></param>
        /// <returns></returns>
        Task UpdateAsync(Action<TSetting> applyChanges);
    }
}
