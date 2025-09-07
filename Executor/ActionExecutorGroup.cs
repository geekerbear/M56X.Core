namespace M56X.Core.Executor
{
    /// <summary>
    /// 动作执行器分组
    /// </summary>
    public class ActionExecutorGroup
    {
        /// <summary>
        /// 分组ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 分组描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 参数
        /// </summary>
        public Dictionary<string, string> Options { get; set; } = [];
    }
}
