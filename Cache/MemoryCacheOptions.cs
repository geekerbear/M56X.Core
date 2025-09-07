using System.Runtime.Caching;

namespace M56X.Core.Cache
{
    /// <summary>
    /// 内存缓存配置项
    /// </summary>
    public class MemoryCacheOptions
    {
        /// <summary>
        /// 缓存名称
        /// </summary>
        public string CacheName { get; set; } = "M56XCoreCache";

        /// <summary>
        /// 默认过期时间
        /// </summary>
        public TimeSpan DefaultExpiry { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// 占用空间大小
        /// </summary>
        public int CacheMemoryLimit { get; set; } = 1024 * 1024 * 1024; // 1MB

        /// <summary>
        /// 可用物理内存百分比
        /// </summary>
        public long PhysicalMemoryLimit { get; set; } = 50;

        /// <summary>
        /// 内存检查间隔
        /// </summary>
        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMinutes(2);
    }
}
