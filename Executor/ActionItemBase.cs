namespace M56X.Core.Executor
{
    /// <summary>
    /// 动作执行项
    /// </summary>
    public abstract class ActionItemBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 显示排序
        /// </summary>
        public int DisOrder { get; set; } = 0;

        /// <summary>
        /// 执行前延时(毫秒)
        /// </summary>
        public int BeforeDelay { get; set; } = 0;

        /// <summary>
        /// 执行后延时(毫秒)
        /// </summary>
        public int AfterDelay { get; set; } = 100;

        /// <summary>
        /// 动作时长
        /// </summary>
        public int Duration { get; set; } = 100;

        /// <summary>
        /// 执行完后是否重置
        /// </summary>
        public bool Reset { get; set; } = false;

        /// <summary>
        /// 执行次数
        /// </summary>
        public uint Count { get; set; } = 1;

        /// <summary>
        /// 获取动作执行完成的时长
        /// </summary>
        /// <returns></returns>
        public int GetDuration()
        {
            int result = BeforeDelay + AfterDelay + Duration;
            if (Reset)
                result += Duration;
            return result * (int)Count;
        }
    }
}
