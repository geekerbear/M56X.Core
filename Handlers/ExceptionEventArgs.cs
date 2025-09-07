namespace M56X.Core.Handlers
{
    public class ExceptionEventArgs(int code, string message, object? data = null, Exception? exception = null) : EventArgs
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public readonly int Code = code;

        /// <summary>
        /// 错误消息
        /// </summary>
        public readonly string Message = message;

        /// <summary>
        /// 附带数据
        /// </summary>
        public readonly object? Data = data;

        /// <summary>
        /// 异常
        /// </summary>
        public readonly Exception? Exception = exception;
    }
}
