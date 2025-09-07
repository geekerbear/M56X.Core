using System.Diagnostics;
using System.Text;

namespace M56X.Core
{
    public class ShellCommand
    {

        /// <summary>
        /// 执行命令行
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
        public static Task Execute(string command, string? workDir, Action<string?> data, Action<string?, Exception?> error, Action<int, string> completed, Encoding encoding, CancellationToken token = default)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return LinuxExecuteCommand(command, workDir, data, error, completed, encoding, token);
            else
                return WindowsExecuteCommand(command, workDir, data, error, completed, encoding, token);
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
                    return proces?.Id ?? -1;
                }
                catch (Exception)
                {
                    return -1;
                }
            });
        }

        /// <summary>
        /// 获取进程
        /// </summary>
        /// <param name="processName">进程名</param>
        /// <returns></returns>
        public static Task<Process[]> GerProcess(string processName)
        {
            return Task.Run(() =>
            {
                Process[] processes = Process.GetProcessesByName(processName);
                return processes;
            });
        }

        /// <summary>
        /// 杀进程
        /// </summary>
        /// <param name="processName">进程名</param>
        public static Task Kill(string processName)
        {
            return Task.Run(async () =>
            {
                Process[] processes = await GerProcess(processName);
                foreach (Process process in processes)
                {
                    try
                    {
                        process.Kill(); // 杀死进程
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"无法杀死进程 {process.ProcessName}: {ex.Message}");
                    }
                }
            });
        }

        /// <summary>
        /// 杀进程
        /// </summary>
        /// <param name="id">进程ID</param>
        public static Task Kill(int id)
        {
            return Task.Run(() =>
            {
                Process process = Process.GetProcessById(id);
                if (process == null) return;
                try
                {
                    process.Kill(); // 杀死进程
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"无法杀死进程 {process.ProcessName}: {ex.Message}");
                }
            });
        }


        #region 平台差异
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
        private static Task WindowsExecuteCommand(string command, string? workDir, Action<string?> data, Action<string?, Exception?> error, Action<int, string> completed, Encoding encoding, CancellationToken token = default)
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
        private static Task LinuxExecuteCommand(string command, string? workDir, Action<string?> data, Action<string?, Exception?> error, Action<int, string> completed, Encoding encoding, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                var escapedArgs = command.Replace("\"", "\\\"");
                string result = string.Empty;
                try
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    encoding ??= Encoding.GetEncoding("GBK");

                    ProcessStartInfo info = new()
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{escapedArgs}\"",
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
        #endregion
    }
}
