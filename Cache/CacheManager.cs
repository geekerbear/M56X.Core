using System.Buffers;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Caching;
using System.Runtime.InteropServices;

namespace M56X.Core.Cache
{
    /// <summary>
    /// 内存缓存管理器
    /// </summary>
    public sealed class CacheManager : ICacheManager
    {
        private readonly MemoryCache _cache;
        private readonly MemoryPool<byte> _memoryPool = MemoryPool<byte>.Shared;
        private bool disposedValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public CacheManager(MemoryCacheOptions? options)
        {
            if (options == null)
                _cache = MemoryCache.Default;
            else
            {
                var config = new NameValueCollection
                {
                    {"cacheMemoryLimitMegabytes", $"{options.CacheMemoryLimit}"},
                    {"physicalMemoryLimitPercentage", $"{options.PhysicalMemoryLimit}"},
                    {"pollingInterval", $"{options.PollingInterval}"}
                };
                _cache = new MemoryCache("CustomCache", config);
            }   
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? Get<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(string key) where T : unmanaged
        {
            if (_cache.Get(key) is IMemoryOwner<byte> memory)
            {
                unsafe
                {
                    fixed (byte* ptr = memory.Memory.Span)
                    {
                        return Marshal.PtrToStructure<T>((IntPtr)ptr);
                    }
                }
            }
            return default;
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        public void Set<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(string key, T value, TimeSpan expiry) where T : unmanaged
        {
            var size = Marshal.SizeOf<T>();
            var memory = _memoryPool.Rent(size);

            unsafe
            {
                fixed (byte* ptr = memory.Memory.Span)
                {
                    Marshal.StructureToPtr(value, (IntPtr)ptr, false);
                }
            }

            _cache.Add(key, memory, DateTimeOffset.Now.Add(expiry));
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)

                    foreach (var item in _cache)
                    {
                        if (item.Value is IDisposable disposable)
                            disposable.Dispose();
                    }
                    _memoryPool.Dispose();

                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~CacheManager()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        
    }
}
