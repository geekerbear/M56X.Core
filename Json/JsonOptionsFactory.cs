using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace M56X.Core.Json
{
    /// <summary>
    /// Json参数工厂
    /// </summary>
    public class JsonOptionsFactory
    {

        /// <summary>
        /// 合并源生成(去重) 
        /// </summary>
        /// <param name="resolvers">源生成参数</param>
        /// <returns>至少带BaseJsonContext.Default</returns>
        public static IJsonTypeInfoResolver Combine(params IJsonTypeInfoResolver?[] resolvers)
        {
            List<IJsonTypeInfoResolver> _resolvers = [];

            _resolvers.Add(BaseJsonSerializerContext.Default);
            
            foreach (var resolver in resolvers)
            {
                if (resolver != null)
                {
                    if (!_resolvers.Any(x=>x.GetHashCode() == resolver.GetHashCode()))
                        _resolvers.Add(resolver);
                }
            } 
            var arr = _resolvers.ToArray();
            return JsonTypeInfoResolver.Combine(arr);
        }

        /// <summary>
        /// 默认JSON参数
        /// </summary>
        /// <param name="indented">是否格式化缩进</param>
        /// <param name="ignoreNull">是否忽略空值</param>
        /// <param name="dateTimeFormat">时间格式</param>
        /// <param name="additionalConverters">自定义转换器集合</param>
        /// <param name="resolvers">源生成参数</param>
        /// <returns>至少带BaseJsonContext.Default</returns>
        public static JsonSerializerOptions DefaultOptions(bool indented = false, bool ignoreNull = true, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss", IList<JsonConverter>? additionalConverters = null, params IJsonTypeInfoResolver[] resolvers)
        {
            var converters = new List<JsonConverter> { new JsonDateTimeConverter(dateTimeFormat) };
            if (additionalConverters != null) converters.AddRange(additionalConverters);

            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = null, // 使用默认的属性命名策略
                IgnoreReadOnlyProperties = false, // 确保不忽略只读属性
                IgnoreReadOnlyFields = false, // 确保不忽略只读字段
                WriteIndented = indented,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = ignoreNull ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never,
                TypeInfoResolver = Combine(resolvers)
            };

            if (converters != null)
            {
                foreach (var jsonConverter in converters)
                {
                    options.Converters.Add(jsonConverter);
                }
            }

            return options;
        }
    }
}
