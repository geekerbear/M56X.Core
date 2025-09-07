using M56X.Core.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace M56X.Core.Config
{
    /// <summary>
    /// 配置文件读写器
    /// </summary>
    /// <typeparam name="TSetting"></typeparam>
    public class SettingOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] TSetting> : ISettingOptions<TSetting> where TSetting : class, ISettingFile, new()
    {
        private TSetting _instance;
        private readonly string _path;
        private readonly JsonSerializerOptions _options;

        private void Load()
        {
            if (File.Exists(_path)) //判断是否存在
            {
                string json = File.ReadAllText(_path);
                var instance = json.JsonDeserialize<TSetting>(true, _options);
                if (instance != null) _instance = instance;
            }
            else
            {
                var json = Value.JsonSerialize(true, _options);
                File.WriteAllText(_path, json);
            }
        }

        /// <summary>
        /// 配置文件读写器
        /// </summary>
        /// <param name="configDir"></param>
        /// <param name="resolver"></param>
        public SettingOptions(string configDir, params IJsonTypeInfoResolver[] resolvers)
        {
            _instance = new TSetting();
            _path = Path.Combine(configDir, $"{_instance.FileName}");
            _options = JsonOptionsFactory.DefaultOptions(resolvers: resolvers);
            Load();
        }

        /// <summary>
        /// 当前值
        /// </summary>
        public TSetting Value => _instance;

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="applyChanges"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Action<TSetting> applyChanges)
        {
            applyChanges(Value);

            var json = Value.JsonSerialize(true, _options);
            await File.WriteAllTextAsync(_path, json);
        }
    }
}
