using System.Diagnostics;
using System.Text;

namespace M56X.Core.Shell
{
    public class ShellHelper
    {

        #region 杀进程
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
        #endregion

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
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return WindowsShell.ExecuteCommand(command, workDir, data, error, completed, encoding, token);
            }
            else
            {
                return LinuxShell.ExecuteCommand(command, workDir, data, error, completed, encoding, token);
            }
        }
    }
}
