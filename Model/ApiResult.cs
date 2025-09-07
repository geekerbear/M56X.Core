using M56X.Core.Extension;

namespace M56X.Core.Model
{
    /// <summary>
    /// 返回结果
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 是否操作成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回代码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回的业务数据
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// 返回的消息
        /// </summary>
        public string? Msg { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; } = DateTime.Now.ToTimestamp(TimestampType.Millisecond);
    }

    /// <summary>
    /// 返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// 是否操作成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回代码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回的业务数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 返回的消息
        /// </summary>
        public string? Msg { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; } = DateTime.Now.ToTimestamp(TimestampType.Millisecond);
    }

    /// <summary>
    /// 返回结果助手
    /// </summary>
    public static class ApiResultHelper
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="success"></param>
        /// <param name="code"></param>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ApiResult ToResult(bool success, int code, object? data = null, string? msg = null)
        {
            return new ApiResult()
            {
                Success = success,
                Code = code,
                Data = data,
                Msg = msg
            };
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">业务数据</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static ApiResult Success(object? data = null, string? msg = null)
        {
            return new ApiResult() 
            {
                Success = true,
                Code = 200,
                Data = data,
                Msg = msg
            };
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="msg">原因消息</param>
        /// <param name="data">错误数据</param>
        /// <returns></returns>
        public static ApiResult Error(string? msg = null, object? data = null)
        {
            return new ApiResult()
            {
                Success = false,
                Code = 400,
                Data = data,
                Msg = msg
            };
        }
    }
}
