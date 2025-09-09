using System.Linq.Expressions;
using System.Reflection;

namespace M56X.Core.Extension
{
    /// <summary>
    ///  深度拷贝扩展
    /// </summary>
    public static class DeepCopyExtensions
    {
        private static readonly Dictionary<Type, Delegate> _cachedCopiers = [];

        /// <summary>
        /// 深度拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T? DeepCopy<T>(this T source) where T : class, new()
        {
            if (source == null) return null;

            var copier = GetOrCreateCopier<T>();
            return copier(source);
        }

        private static Func<T, T> GetOrCreateCopier<T>() where T : class, new()
        {
            var type = typeof(T);
            if (!_cachedCopiers.TryGetValue(type, out _))
            {
                lock (_cachedCopiers)
                {
                    if (!_cachedCopiers.ContainsKey(type))
                    {
                        Delegate? del = CreateCopier<T>();
                        _cachedCopiers[type] = del;
                    }
                }
            }
            return (Func<T, T>)_cachedCopiers[type];
        }

        private static Func<T, T> CreateCopier<T>() where T : class, new()
        {
            var sourceParam = Expression.Parameter(typeof(T), "source");
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var bindings = new List<MemberBinding>();
#pragma warning disable IL2090 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The generic parameter of the source method or type does not have matching annotations.
            foreach (var prop in typeof(T).GetProperties(bindingFlags))
            {
                if (!prop.CanWrite) continue;

                var sourceProp = Expression.Property(sourceParam, prop);
                MemberBinding binding;

                if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                {
                    binding = Expression.Bind(prop, sourceProp);
                }
                else
                {
#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable IL2076 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The return value of the source method does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
                    var copyMethod = typeof(DeepCopyExtensions).GetMethod(nameof(DeepCopy))
                        .MakeGenericMethod(prop.PropertyType);
#pragma warning restore IL2076 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The return value of the source method does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
#pragma warning restore CS8602 // 解引用可能出现空引用。
                    var copiedValue = Expression.Call(copyMethod, sourceProp);
                    binding = Expression.Bind(prop, copiedValue);
                }

                bindings.Add(binding);
            }
#pragma warning restore IL2090 // 'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The generic parameter of the source method or type does not have matching annotations.

            var memberInit = Expression.MemberInit(Expression.New(typeof(T)), bindings);
            var lambda = Expression.Lambda<Func<T, T>>(memberInit, sourceParam);
            return lambda.Compile();
        }
    }
}
