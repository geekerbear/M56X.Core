using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace M56X.Core.Cache
{
    /// <summary>
    /// 缓存管理器扩展
    /// </summary>
    public static class CacheManagerExtensions
    {
        /// <summary>
        /// 添加内存缓存管理器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        public static IServiceCollection AddCacheManager(this IServiceCollection services, Action<MemoryCacheOptions>? options = null)
        {
            //if (services.LastOrDefault(d => d.ServiceType == typeof(IAppInfo))?.ImplementationInstance is not IAppInfo info)
            //    return;

            services.AddOptions<MemoryCacheOptions>()
                .Configure(options ?? (opt => opt.DefaultExpiry = TimeSpan.FromMinutes(30)));

            services.AddSingleton<ICacheManager>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<MemoryCacheOptions>>();
                return new CacheManager(options.Value);
            });
            return services;
        }
    }
}
