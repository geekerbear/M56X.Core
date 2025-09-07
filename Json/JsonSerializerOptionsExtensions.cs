using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace M56X.Core.Json
{
    /// <summary>
    /// JsonSerializerOptions 扩展
    /// </summary>
    public static class JsonSerializerOptionsExtensions
    {
        /// <summary>
        /// 克隆
        /// <para>value为空则创建默认值</para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JsonSerializerOptions Clone(this JsonSerializerOptions? value, params IJsonTypeInfoResolver[] resolvers)
        {
            value ??= JsonOptionsFactory.DefaultOptions(resolvers: resolvers);

            List<IJsonTypeInfoResolver> _resolvers = [];
            _resolvers.AddRange(value.TypeInfoResolverChain);
            _resolvers.AddRange(resolvers);
            
            value.TypeInfoResolver = JsonOptionsFactory.Combine([.. _resolvers]);
            return new JsonSerializerOptions(value);
        }


    }
}
