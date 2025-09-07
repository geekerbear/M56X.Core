using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace M56X.Core.Config
{
    /// <summary>
    /// 配置接口
    /// </summary>
    public interface ISettingFile
    {
        /// <summary>
        /// 配置文件名称
        /// </summary>
        [JsonIgnore]
        string FileName { get; }
    }
}
