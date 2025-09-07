using M56X.Core.Security;

namespace M56X.Core
{
    public class UniqueID
    {
        /// <summary>
        /// 获取本机唯一ID
        /// </summary>
        /// <param name="custom">自定义字符串</param>
        /// <param name="mac">网卡mac是否参与计算</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string GetUniqueID(string custom, bool mac = false, string separator = "-")
        {
            string result = string.Empty;
            //if (mac)
            //{
            //    result += Network.GetMacAddress()[0].SerialNumber;
            //}
            //if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            //{
            //    //var disk = M56X.Win32Net.Methods.PhysicalDriveMethods.GetPhysicalDrive();
            //    //result += disk.SerialNumber + disk.ProductRevision;
            //    return "";
            //}
            //else
            //    result += ""; /*NativeMethods.Linux.Hardware.Disk.GetLinuxDiskSerialNumber()[0].SerialNumber*/;

            var cards = M56X.Core.Net.NetworkCardHelper.GetNics()
                .Where(x => x.Name.Contains("usb", StringComparison.OrdinalIgnoreCase) == false && x.Description.Contains("usb", StringComparison.OrdinalIgnoreCase) == false).ToArray();
            if (cards == null || cards.Length == 0)
                cards = M56X.Core.Net.NetworkCardHelper.GetNics();
            var macs = cards.Select(x => x.MacAddress).ToArray();
            result += string.Join(':', macs);

            result += custom;
            //Console.WriteLine(result);
            result = result.MD5();
            return $"{result[..8]}{separator}{result.Substring(8, 4)}{separator}{result.Substring(12, 4)}{separator}{result.Substring(16, 4)}{separator}{result[20..]}";
        }
    }
}
