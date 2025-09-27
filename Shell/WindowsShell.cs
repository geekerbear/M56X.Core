using System.Diagnostics;
using System.Text;

namespace M56X.Core.Shell
{
    public class WindowsShell
    {
        /// <summary>
        /// 执行命令行指令
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="workDir">工作目录</param>
        /// <param name="data">数据回调</param>
        /// <param name="error">错误回调</param>
        /// <param name="completed">
        /// 执行成功回调
        /// <para>0 正常结束</para>
        /// <para>1 手动终止</para>
        /// </param>
        /// <param name="encoding">输出编码, 默认: GBK</param>
        /// <param name="token">终止token</param>
        /// <returns></returns>
        public static Task ExecuteCommand(string command, string? workDir = null, Action<string?>? data = null, Action<string, Exception?>? error = null, Action<int, string>? completed = null, Encoding? encoding = null, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                string result = string.Empty;
                try
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    encoding ??= Encoding.GetEncoding("GBK");

                    ProcessStartInfo info = new()
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C {command}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = encoding,
                        StandardErrorEncoding = encoding,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    if (!string.IsNullOrEmpty(workDir))
                        info.WorkingDirectory = workDir;

                    using Process process = new() { StartInfo = info };
                    process.Start();

                    process.OutputDataReceived += (s, e) =>
                    {
                        result += $"{e.Data}{Environment.NewLine}";
                        data?.Invoke(e.Data);
                    };

                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            error?.Invoke(e.Data, null);
                    };

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    await process.WaitForExitAsync(token);
                    completed?.Invoke(0, result);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(TaskCanceledException))
                    {
                        completed?.Invoke(1, result);
                    }
                    else
                    {
                        error?.Invoke($"Error executing command: {ex.Message}", ex);
                    }
                }
            }, token);
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="launchFile">启动文件路径</param>
        /// <param name="args">参数</param>
        /// <param name="workDir">工作目录</param>
        /// <returns></returns>
        public static Task<int> Start(string launchFile, string args, string? workDir = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    var info = new ProcessStartInfo
                    {
                        FileName = launchFile,
                        Arguments = args,
                    };
                    if (!string.IsNullOrEmpty(workDir))
                        info.WorkingDirectory = workDir;

                    using var proces = Process.Start(info);
                    if (proces != null)
                        return proces.Id;
                    else
                        return 0;
                }
                catch (Exception)
                {
                    return -1;
                }
            });
        }

        /// <summary>
        /// 调用默认浏览器打开网址
        /// </summary>
        /// <param name="url">网址</param>
        public static Task OpenUrl(string url)
        {
            return Start("explorer.exe", url, null);
        }
    }
}
