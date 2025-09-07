using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

namespace M56X.Core.Config
{
    /// <summary>
    /// 
    /// </summary>
    public static class SettingFileExtensions
    {
        /// <summary>
        /// 添加配置文件
        /// </summary>
        /// <typeparam name="TSetting"></typeparam>
        /// <param name="services"></param>
        /// <param name="resolvers"></param>
        public static IServiceCollection AddSettingFile<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] TSetting>(this IServiceCollection services, string configDir = "", params IJsonTypeInfoResolver[] resolvers)
            where TSetting : class, ISettingFile, new()
        {
            if (string.IsNullOrEmpty(configDir))
            {
                configDir = Path.Combine(AppContext.BaseDirectory, "Config");
            }
            if(!Directory.Exists(configDir))
                Directory.CreateDirectory(configDir);
            services.AddSingleton<ISettingOptions<TSetting>>(new SettingOptions<TSetting>(configDir, resolvers));
            return services;
        }
    }
}
