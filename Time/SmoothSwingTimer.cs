namespace M56X.Core.Time
{
    /// <summary>
    /// 平滑摆动计数器
    /// </summary>
    /// <param name="duration">时长</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="speed">速度</param>
    /// <param name="interval">周期</param>
    public class SmoothSwingTimer(TimeSpan duration, double min, double max, double speed = 10, int interval = 50) : IDisposable
    {
        private bool disposedValue;
        private readonly TimeSpan _duration = duration;
        private readonly int _interval = interval;
        private Timer? _timer;
        private readonly ManualResetEvent _stopEvent = new(false);
        private bool _startFlag;

        private readonly double _min = min;
        private readonly double _max = max;
        private readonly double _speed = speed; // 速度（控制正弦波频率）
        private readonly double _amplitude = (max - min) / 2; // 摆动幅度（区间范围）
        private readonly double _center = (min + max) / 2;
        private double _time;      // 时间累计器
        private double _value;

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
            {
                _value = _center + Math.Sin(_time * _speed) * _amplitude;
                _time += 0.1;
                OnCounter?.Invoke(this, new CounterEventArgs(DateTime.Now, _value));
            }
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
