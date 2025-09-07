using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace M56X.Core.Net
{
    public struct NicInfo
    {
        public string Name;
        public string Type;
        public string MacAddress;
        public bool IsUsbAdapter;
        public string Description;
        public bool IsVirtualAdapter;
    }

    /// <summary>
    /// 网卡助手类
    /// </summary>
    public class NetworkCardHelper
    {
        /// <summary>
        /// 获取实体网卡信息
        /// </summary>
        /// <returns></returns>
        public static NicInfo[] GetNics()
        {
            var nics = M56X.Core.Net.NetworkCardHelper.GetAllNetworkInterfaces().Where(x => x.IsVirtualAdapter == false
                && !x.Description.Contains("microsoft", StringComparison.OrdinalIgnoreCase)
                && !x.Description.Contains("wan", StringComparison.OrdinalIgnoreCase)
                && !x.Description.Contains("bluetooth", StringComparison.OrdinalIgnoreCase)).ToArray();
            return nics;
        }

        public static NicInfo[] GetAllNetworkInterfaces()
        {
            return [.. NetworkInterface.GetAllNetworkInterfaces()
                .Select(adapter => new NicInfo
                {
                    Name = adapter.Name,
                    Type = adapter.NetworkInterfaceType.ToString(),
                    MacAddress = FormatMacAddress(adapter.GetPhysicalAddress()),
                    IsUsbAdapter = IsUsbNetworkAdapter(adapter),
                    Description = adapter.Description,
                    IsVirtualAdapter = IsVirtualNetworkAdapter(adapter),
                })];
        }

        private static string FormatMacAddress(PhysicalAddress address)
        {
            var bytes = address.GetAddressBytes();
            return string.Join(":", bytes.Select(b => b.ToString("X2")));
        }

        private static bool IsUsbNetworkAdapter(NetworkInterface adapter)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return adapter.Description.Contains("USB", StringComparison.OrdinalIgnoreCase) ||
                       adapter.Name.Contains("USB", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return adapter.Description.Contains("usb", StringComparison.OrdinalIgnoreCase) ||
                       adapter.Name.StartsWith("usb", StringComparison.OrdinalIgnoreCase) ||
                       adapter.Name.Contains("rndis", StringComparison.OrdinalIgnoreCase);
            }
        }

        private static bool IsVirtualNetworkAdapter(NetworkInterface adapter)
        {
            string tp = adapter.NetworkInterfaceType.ToString();
            if (tp.ToLower().Contains("tunnel", StringComparison.OrdinalIgnoreCase))
                return true; 
            else if (tp.ToLower().Contains("ppp", StringComparison.OrdinalIgnoreCase))
                return true;
            else if (tp.ToLower().Contains("loopback", StringComparison.OrdinalIgnoreCase))
                return true;

            string[] blocks = ["virtual", "virbr", "veth", "docker", "br-", "tun", "npcap", "tap", "pptp", "l2tp", "ikev", "sstp", "pppoe", "loopback", "LightWeight", "QoS", "Packet"];

            string desc = adapter.Description.ToLower();
            foreach (string block in blocks)
            {
                if(desc.Contains(block, StringComparison.OrdinalIgnoreCase)) return true;
            }

            string name = adapter.Name.ToLower();
            foreach (string block in blocks)
            {
                if (name.Contains(block, StringComparison.OrdinalIgnoreCase)) return true;
            }

            return false;
        }
    }
}
