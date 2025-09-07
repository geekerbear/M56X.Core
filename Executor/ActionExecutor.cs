using M56X.Core.Time;

namespace M56X.Core.Executor
{
    /// <summary>
    /// 动作执行器
    /// </summary>
    public class ActionExecutor<TActionItem> : IDisposable where TActionItem : ActionItemBase
    {
        private AsyncCronTimer? _timer;
        private bool disposedValue;
        private readonly SemaphoreSlim _syncLock = new(1, 1);
        private CancellationTokenSource? _cts;

        #region 属性
        /// <summary>
        /// 动作ID
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 动作名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 动作描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 动作分子ID
        /// </summary>
        public string GroupId { get; set; } = string.Empty;

        /// <summary>
        /// Cron时间表达式(默认每5秒执行一次)
        /// </summary>
        public string Cron { get; set; } = "0/5 * * * * ?";

        /// <summary>
        /// 动作触发类型
        /// </summary>
        public ActionTriggerType TriggerType { get; set; } = ActionTriggerType.Manual;

        /// <summary>
        /// 执行方式,默认顺序执行
        /// </summary>
        public ActionExecuteMode ExecuteMode { get; set; } = ActionExecuteMode.Sequence;

        /// <summary>
        /// 动作执行项集合
        /// </summary>
        public List<TActionItem> Items { get; set; } = [];

        /// <summary>
        /// 执行时长偏移量
        /// </summary>
        public int DurationOffset { get; set; } = 0;

        /// <summary>
        /// 动作运行状态
        /// </summary>
        public bool IsRunning { get; private set; } = false;

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; } = true;
        #endregion

        /// <summary>
        /// 执行动作项事件
        /// </summary>
        public event Func<string, TActionItem, CancellationToken, Task>? OnExecute;

        #region 启动、停止
        /// <summary>
        /// 启动
        /// </summary>
        public async Task StartAsync(CancellationToken stoppingToken)
        {
            await _syncLock.WaitAsync(stoppingToken);
            try
            {
                if (IsRunning) return;
                _cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                IsRunning = true;

                if (TriggerType == ActionTriggerType.Timer)
                {
                    if (string.IsNullOrEmpty(Cron))
                        Cron = "0/5 * * * * ?";
                    _timer = new AsyncCronTimer(Cron);
                    _timer.Elapsed += async (arg) => await ExecuteActionAsync(_cts.Token);
                    _timer.Start();
                }
            }
            finally { _syncLock.Release(); }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public async Task StopAsync()
        {
            await _syncLock.WaitAsync();

            try
            {
                if (!IsRunning) return;

                _cts?.Cancel();
                _timer?.Stop();
                _timer?.Dispose();
                _timer = null;
                IsRunning = false;
            }
            finally { _syncLock.Release(); }
        }
        #endregion

        #region 内部方法
        private async Task ExecuteActionAsync(CancellationToken ct)
        {
            if (ExecuteMode == ActionExecuteMode.SameTime)
            {
                await Parallel.ForEachAsync(Items, ct, async (item, _) =>
                {
                    await (OnExecute?.Invoke(Id, item, ct) ?? Task.CompletedTask);
                });
            }
            else
            {
                foreach (var item in Items)
                {
                    ct.ThrowIfCancellationRequested();
                    await (OnExecute?.Invoke(Id, item, ct) ?? Task.CompletedTask);
                }
            }
        }
        #endregion

        /// <summary>
        /// 执行动作
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="delay">延时执行毫秒数</param>
        /// <returns></returns>
        public async Task Execute(int delay = 0)
        {
            if (!IsRunning) return;
            if (!Status) return;
            if (_cts == null) return;

            if (delay > 0)
                await Task.Delay(delay, _cts.Token).ConfigureAwait(false);

            await ExecuteActionAsync(_cts.Token);
        }

        /// <summary>
        /// 取消操作
        /// </summary>
        public void Cancel()
        {
            _cts?.Cancel();
        }

        /// <summary>
        /// 获取动作执行完成的时长
        /// </summary>
        /// <returns></returns>
        public int GetDuration()
        {
            int duration = 0;
            if (ExecuteMode == ActionExecuteMode.SameTime)
            {
                var max = Items.Max(x => x.GetDuration());
                duration = max;
            }
            else
            {
                foreach (var item in Items)
                {
                    duration += item.GetDuration();
                }
            }

            return duration + DurationOffset;
        }

        public void Dispose()
        {
            if (disposedValue) return;

            _cts?.Dispose();
            _timer?.Dispose();
            _syncLock.Dispose();
            disposedValue = true;
            GC.SuppressFinalize(this);
        }
    }
}
