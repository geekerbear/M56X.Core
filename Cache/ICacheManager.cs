using System.Diagnostics.CodeAnalysis;

namespace M56X.Core.Cache
{
    /// <summary>
    /// 缓存管理器接口
    /// </summary>
    public interface ICacheManager : IDisposable
    {
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        void Set<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                                       DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(string key, T value, TimeSpan expiry) where T : unmanaged;

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T? Get<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(string key) where T : unmanaged;
    }
}
