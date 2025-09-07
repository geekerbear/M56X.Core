using TimeCrontab;

namespace M56X.Core.Time
{
    /// <summary>
    /// 异步Cron定时器
    /// </summary>
    public class AsyncCronTimer : IDisposable
    {
        private bool disposedValue;
        private readonly Timer _taskTimer;
        private readonly Crontab _crontab;

        private volatile bool _performingTasks;
        private volatile bool _isRunning;

        /// <summary>
        /// 是否启动就运行一次
        /// </summary>
        public bool RunOnStart { get; set; }

        /// <summary>
        /// 事件
        /// </summary>
        public event Func<AsyncCronTimer, Task> Elapsed = _ => Task.CompletedTask;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <exception cref="ArgumentException"></exception>
        public AsyncCronTimer(string cronExpression = "0/5 * * * * ?")
        {
            if (string.IsNullOrEmpty(cronExpression))
            {
                throw new ArgumentException("Cron表达式不能为空", nameof(cronExpression));
            }
            if (!Crontab.IsValid(cronExpression, CronStringFormat.WithSeconds))
            {
                throw new ArgumentException("Cron表达式无效", nameof(cronExpression));
            }

            _crontab = Crontab.Parse(cronExpression, CronStringFormat.WithSeconds);

            _taskTimer = new Timer(
                TimerCallBack!,
                null,
                Timeout.Infinite,
                Timeout.Infinite
            );
        }

        public void Start()
        {
            lock (_taskTimer)
            {
                _taskTimer.Change(RunOnStart ? 0 : GetPeriod(), Timeout.Infinite);
                _isRunning = true;
            }
        }

        public void Stop()
        {
            lock (_taskTimer)
            {
                _taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
                while (_performingTasks)
                {
                    Monitor.Wait(_taskTimer);
                }

                _isRunning = false;
            }
        }

        private void TimerCallBack(object state)
        {
            lock (_taskTimer)
            {
                if (!_isRunning || _performingTasks)
                {
                    return;
                }

                _taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _performingTasks = true;
            }

            _ = Timer_Elapsed();
        }

        private async Task Timer_Elapsed()
        {
            try
            {
                await Elapsed(this);
            }
            catch (Exception ex)
            {
                // TODO Logger
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                lock (_taskTimer)
                {
                    _performingTasks = false;
                    if (_isRunning)
                    {
                        _taskTimer.Change(GetPeriod(), Timeout.Infinite);
                    }

                    Monitor.Pulse(_taskTimer);
                }
            }
        }

        private int GetPeriod()
        {
            return (int)_crontab.GetSleepMilliseconds(DateTime.Now);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)

                    _taskTimer.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~AsyncCronTimer()
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
