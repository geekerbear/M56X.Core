using System.Collections.Concurrent;
using System.Text;

namespace M56X.Core.Config
{
    public class IniParser : IDisposable
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _data;
        private readonly string _filePath;
        private readonly ReaderWriterLockSlim _fileLock = new();
        private readonly Encoding _encoding = Encoding.UTF8;
        private bool disposedValue;

        public IniParser(string filePath)
        {
            _filePath = filePath;
            _data = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            Load();
        }

        // 加载INI文件（线程安全）
        private void Load()
        {
            _fileLock.EnterReadLock();
            try
            {
                if (!File.Exists(_filePath)) return;

                string? currentSection = null;
                foreach (var line in File.ReadLines(_filePath, _encoding))
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith(';') || trimmed.StartsWith('#')) continue;

                    if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
                    {
                        currentSection = trimmed[1..^1];
                        _data[currentSection] = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    }
                    else if (currentSection != null && trimmed.Contains('='))
                    {
                        var splitPos = trimmed.IndexOf('=');
                        var key = trimmed[..splitPos].Trim();
                        var value = trimmed[(splitPos + 1)..].Trim();
                        _data[currentSection][key] = value;
                    }
                }
            }
            finally
            {
                _fileLock.ExitReadLock();
            }
        }

        // 基础类型读写方法（线程安全）
        public string GetString(string section, string key, string defaultValue = "")
        {
            return _data.TryGetValue(section, out var sectionData) &&
                   sectionData.TryGetValue(key, out var value)
                ? value : defaultValue;
        }

        public bool GetBool(string section, string key, bool defaultValue = false)
        {
            var value = GetString(section, key);
            return bool.TryParse(value, out var result) ? result : defaultValue;
        }

        public int GetInt(string section, string key, int defaultValue = 0)
        {
            var value = GetString(section, key);
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        public long GetLong(string section, string key, long defaultValue = 0)
        {
            var value = GetString(section, key);
            return int.TryParse(value, out var result) ? result : defaultValue;
        }

        public void SetValue(string section, string key, object value)
        {
            _fileLock.EnterWriteLock();
            try
            {
                var sectionData = _data.GetOrAdd(section, _ => new ConcurrentDictionary<string, string>());
                sectionData[key] = value?.ToString() ?? string.Empty;
                SaveInternal();
            }
            finally
            {
                _fileLock.ExitWriteLock();
            }
        }

        // 使用StringBuilder优化写入性能
        private void SaveInternal()
        {
            var sb = new StringBuilder(2048);
            foreach (var section in _data)
            {
                sb.Append('[').Append(section.Key).Append(']').AppendLine();
                foreach (var kvp in section.Value)
                    sb.Append(kvp.Key).Append('=').Append(kvp.Value).AppendLine();
                sb.AppendLine();
            }
            File.WriteAllText(_filePath, sb.ToString(), _encoding);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    _fileLock?.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~IniParser()
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
