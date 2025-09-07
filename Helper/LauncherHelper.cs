using System.Diagnostics;

namespace M56X.Core.Helper
{
    public class LauncherHelper
    {
        ///// <summary>
        ///// 将程序至于前台
        ///// </summary>
        //public static void SetForegroundWindow()
        //{
        //    nuint SC_RESTORE = 0xF120;
        //    uint WM_SYSCOMMAND = 0x0112;
        //    Process cur = Process.GetCurrentProcess();
        //    foreach (Process p in Process.GetProcesses())
        //    {
        //        if (p.Id == cur.Id) continue;
        //        if (p.ProcessName == cur.ProcessName)
        //        {
        //            Win32Net.Methods.WindowMethods.SetForegroundWindow(p.MainWindowHandle);
        //            Win32Net.Methods.MessageMethods.SendMessage(p.MainWindowHandle, WM_SYSCOMMAND, SC_RESTORE, 0);
        //            return;
        //        }
        //    }
        //}
    }
}
