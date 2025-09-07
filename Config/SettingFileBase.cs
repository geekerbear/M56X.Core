using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace M56X.Core.Config
{
    /// <summary>
    /// 配置文件基类
    /// </summary>
    /// <param name="name">配置文件名</param>
    public abstract class SettingFileBase(string name) : ObservableObject, ISettingFile
    {
        /// <summary>
        /// 配置文件名
        /// </summary>
        [JsonIgnore]
        public string FileName { get; } = name;
    }
}
