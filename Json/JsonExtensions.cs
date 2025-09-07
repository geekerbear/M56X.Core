using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace M56X.Core.Json
{
    public static class JsonExtensions
    {
        /// <summary>
        /// 对象序列化为JSON字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        /// <returns></returns>
        public static string JsonSerialize(this object value, JsonSerializerOptions? options = null, params IJsonTypeInfoResolver[] resolvers)
        {
            options = options.Clone(resolvers);
            return JsonSerializer.Serialize(value, options);
        }

        /// <summary>
        /// 对象序列化为JSON字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="indented"></param>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        /// <returns></returns>
        public static string JsonSerialize(this object value, bool indented, JsonSerializerOptions? options = null, params IJsonTypeInfoResolver[] resolvers)
        {
            options = options.Clone(resolvers);
            options.WriteIndented = indented;
            return JsonSerializer.Serialize(value, options);
        }

        /// <summary>
        /// 对象序列化为JSON字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        /// <returns></returns>
        public static byte[] JsonSerializeBytes(this object value, JsonSerializerOptions? options = null, params IJsonTypeInfoResolver[] resolvers)
        {
            options = options.Clone(resolvers);
            return JsonSerializer.SerializeToUtf8Bytes(value, options);
        }

        /// <summary>
        /// 对象序列化为JSON字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="indented"></param>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        /// <returns></returns>
        public static byte[] JsonSerializeBytes(this object value, bool indented, JsonSerializerOptions? options = null, params IJsonTypeInfoResolver[] resolvers)
        {
            options = options.Clone(resolvers);
            options.WriteIndented = indented;
            return JsonSerializer.SerializeToUtf8Bytes(value, options);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        /// <returns></returns>
        public static object? JsonDeserialize(this string value, Type type, JsonSerializerOptions? options = null, params IJsonTypeInfoResolver[] resolvers)
        {
            options = options.Clone(resolvers);
            return JsonSerializer.Deserialize(value, type, options);
        }

        /// <summary>
        /// Json反序列化为对象
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        /// <returns></returns>
        public static TObject? JsonDeserialize<TObject>(this string value, JsonSerializerOptions? options = null, params IJsonTypeInfoResolver[] resolvers)
        {
            options = options.Clone(resolvers);
            return JsonSerializer.Deserialize<TObject>(value, options);
        }

        /// <summary>
        /// Json反序列化为对象
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="value"></param>
        /// <param name="indented"></param>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        /// <returns></returns>
        public static TObject? JsonDeserialize<TObject>(this string value, bool indented, JsonSerializerOptions? options = null, params IJsonTypeInfoResolver[] resolvers)
        {
            options = options.Clone(resolvers);
            options.WriteIndented = indented;
            return JsonSerializer.Deserialize<TObject>(value, options);
        }

        /// <summary>
        /// Json反序列化为对象
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        /// <returns></returns>
        public static TObject? JsonDeserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TObject>(this ReadOnlySpan<byte> value, JsonSerializerOptions? options = null, params IJsonTypeInfoResolver[] resolvers)
        {
            options = options.Clone(resolvers);
            return JsonSerializer.Deserialize<TObject>(value, options);
        }

        /// <summary>
        /// Json反序列化为对象
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="value"></param>
        /// <param name="indented"></param>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        /// <returns></returns>
        public static TObject? JsonDeserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TObject>(this ReadOnlySpan<byte> value, bool indented, JsonSerializerOptions? options = null, params IJsonTypeInfoResolver[] resolvers) where TObject : class
        {
            options = options.Clone(resolvers);
            options.WriteIndented = indented;

            return JsonSerializer.Deserialize<TObject>(value, options);
        }

        /// <summary>
        /// Json反序列化为对象
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        /// <returns></returns>
        public static TObject? JsonDeserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TObject>(this byte[] value, JsonSerializerOptions? options = null, params IJsonTypeInfoResolver[] resolvers)
        {
            options = options.Clone(resolvers);
            return JsonSerializer.Deserialize<TObject>(value, options);
        }

        /// <summary>
        /// Json反序列化为对象
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="value"></param>
        /// <param name="indented"></param>
        /// <param name="options"></param>
        /// <param name="resolvers"></param>
        /// <returns></returns>
        public static TObject? JsonDeserialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TObject>(this byte[] value, bool indented, JsonSerializerOptions? options = null, params IJsonTypeInfoResolver[] resolvers) where TObject : class
        {
            options = options.Clone(resolvers);
            options.WriteIndented = indented;

            return JsonSerializer.Deserialize<TObject>(value, options);
        }
    }
}
