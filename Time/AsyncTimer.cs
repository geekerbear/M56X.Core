namespace M56X.Core.Time
{
    /// <summary>
    /// 异步定时器
    /// </summary>
    public class AsyncTimer : IDisposable
    {
        private bool disposedValue;
        private readonly Timer _taskTimer;
        private volatile bool _performingTasks;
        private volatile bool _isRunning;

        /// <summary>
        /// 周期
        /// </summary>
        public int Period { get; set; }

        public bool RunOnStart { get; set; }

        public event Func<AsyncTimer, Task> Elapsed = _ => Task.CompletedTask;
        

        public AsyncTimer()
        {
            _taskTimer = new Timer(
                TimerCallBack!,
                null,
                Timeout.Infinite,
                Timeout.Infinite
            );
        }

        public void Start()
        {
            if (Period <= 0)
            {
                throw new ArgumentException("Period should be set before starting the timer!");
            }

            lock (_taskTimer)
            {
                _taskTimer.Change(RunOnStart ? 0 : Period, Timeout.Infinite);
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
                        _taskTimer.Change(Period, Timeout.Infinite);
                    }

                    Monitor.Pulse(_taskTimer);
                }
            }
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
        // ~AsyncTimer()
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
