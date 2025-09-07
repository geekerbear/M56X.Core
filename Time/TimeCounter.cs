using System.Threading.Tasks;

namespace M56X.Core.Time
{
    /// <summary>
    /// 计数事件
    /// </summary>
    /// <param name="time"></param>
    /// <param name="value"></param>
    public class CounterEventArgs(DateTime time, double value) : EventArgs
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time = time;

        /// <summary>
        /// 当前计数值
        /// </summary>
        public double Value = value;
    }

    /// <summary>
    /// 时间计数器
    /// </summary>
    /// <param name="duration">计数时长</param>
    /// <param name="interval">计数间隔时间(毫秒)</param>
    /// <param name="init">初始值</param>
    public class TimeCounter(TimeSpan duration, int interval = 1000, ulong init = 0) : IDisposable
    {
        private bool disposedValue;
        private readonly TimeSpan _duration = duration;
        private readonly int _interval = interval;
        private Timer? _timer;
        private readonly ManualResetEvent _stopEvent = new(false);
        private ulong _value;
        private readonly ulong _init = init;
        private bool _startFlag;

        /// <summary>
        /// 开始事件
        /// </summary>
        public event EventHandler? OnStarted;

        /// <summary>
        /// 结束事件
        /// </summary>
        public event EventHandler? OnStoped;

        /// <summary>
        /// 计数事件
        /// </summary>
        public event EventHandler<CounterEventArgs>? OnCounter;

        /// <summary>
        /// 开始计数
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            _value = _init;
            _timer = new Timer(StepTimerCallback, null, 0, _interval);
            OnStarted?.Invoke(this, EventArgs.Empty);
            _startFlag = true;
            await Task.Run(() =>
            {
                _stopEvent.WaitOne(_duration);
                _startFlag = false;
                _timer?.Dispose();// 停止计时器
                OnStoped?.Invoke(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// 停止
        /// </summary>
        public async Task Stop()
        {
            _stopEvent.Set();
            await Task.Delay(1);
        }

        public async Task Reset()
        {
            await Stop();
            _stopEvent.Reset();   
            await Start();
        }

        private void StepTimerCallback(object? state)
        {
            if (_startFlag)
                OnCounter?.Invoke(this, new CounterEventArgs(DateTime.Now, _value++));
        }

        #region 销毁
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    _timer?.Dispose();
                    _stopEvent.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~TimeCounter()
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
        #endregion
    }
}
