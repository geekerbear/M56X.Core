namespace M56X.Core.Model
{
    /// <summary>
    /// 网络统计数据
    /// </summary>
    public class NetStatistics
    {
        private readonly DateTime _startTime = DateTime.Now;
        private long _receivedBytes;
        private long _receivedMessages;
        private long _sentBytes;
        private long _sentMessages;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime => _startTime;

        /// <summary>
        /// 已启动活跃时长
        /// </summary>
        public TimeSpan UpTime => DateTime.Now - _startTime;

        /// <summary>
        /// 收到的字节数
        /// </summary>
        public long ReceivedBytes => _receivedBytes;

        /// <summary>
        /// 收到的消息数
        /// </summary>
        public long ReceivedMessages => _receivedMessages;

        /// <summary>
        /// 平均接收到的消息大小(字节)
        /// </summary>
        public int ReceivedMessageSizeAverage
        {
            get
            {
                if (_receivedBytes > 0 && _receivedMessages > 0)
                {
                    return (int)(_receivedBytes / _receivedMessages);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 发送的字节数
        /// </summary>
        public long SentBytes => _sentBytes;

        /// <summary>
        /// 发送的消息数
        /// </summary>
        public long SentMessages => _sentMessages;

        /// <summary>
        /// 平均发送的消息大小(字节)
        /// </summary>
        public decimal SentMessageSizeAverage
        {
            get
            {
                if (_sentBytes > 0 && _sentMessages > 0)
                {
                    return (int)(_sentBytes / _sentMessages);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _receivedBytes = 0;
            _receivedMessages = 0;
            _sentBytes = 0;
            _sentMessages = 0;
        }

        public override string ToString()
        {
            string ret =
                "--- Statistics ---" + Environment.NewLine +
                "    Started     : " + _startTime.ToString() + Environment.NewLine +
                "    Uptime      : " + UpTime.ToString() + Environment.NewLine +
                "    Received    : " + Environment.NewLine +
                "       Bytes    : " + ReceivedBytes + Environment.NewLine +
                "       Messages : " + ReceivedMessages + Environment.NewLine +
                "       Average  : " + ReceivedMessageSizeAverage + " bytes" + Environment.NewLine +
                "    Sent        : " + Environment.NewLine +
                "       Bytes    : " + SentBytes + Environment.NewLine +
                "       Messages : " + SentMessages + Environment.NewLine +
                "       Average  : " + SentMessageSizeAverage + " bytes" + Environment.NewLine;
            return ret;
        }

        /// <summary>
        /// 增加接收到的消息数量
        /// </summary>
        public void IncrementReceivedMessages()
        {
            _receivedMessages = Interlocked.Increment(ref _receivedMessages);
        }

        /// <summary>
        /// 增加发送的消息数量
        /// </summary>
        public void IncrementSentMessages()
        {
            _sentMessages = Interlocked.Increment(ref _sentMessages);
        }

        /// <summary>
        /// 增加收到的字节数
        /// </summary>
        /// <param name="bytes"></param>
        public void AddReceivedBytes(long bytes)
        {
            _receivedBytes = Interlocked.Add(ref _receivedBytes, bytes);
        }

        /// <summary>
        /// 增加发送的字节数
        /// </summary>
        /// <param name="bytes"></param>
        public void AddSentBytes(long bytes)
        {
            _sentBytes = Interlocked.Add(ref _sentBytes, bytes);
        }
    }
}
