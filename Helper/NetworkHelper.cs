using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace M56X.Core.Helper
{
    public class NetworkHelper
    {
        #region Tcp端口获取和检测
        /// <summary>
        /// 判断端口是否可用
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool IsTcpPortAvailable(string ipAddress, int port)
        {
            using TcpClient tcpClient = new();
            try
            {
                tcpClient.Connect(ipAddress, port);
                if (tcpClient.Connected)
                {
                    tcpClient.Close();
                    return false;
                }
            }
            catch (SocketException)
            {
                // ignored
            }
            return true;
        }

        /// <summary>
        /// 获取可用端口
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="startPort"></param>
        /// <param name="endPort"></param>
        /// <returns></returns>
        public static int GetTcpPortAvailable(string ipAddress, int startPort = 10000, int endPort = 655354)
        {
            if (startPort >= endPort)
                return -1;

            for (int port = startPort; port < endPort; port++)
            {
                if (IsTcpPortAvailable(ipAddress, port))
                    return port;
            }
            return -1;
        }
        #endregion

        #region Ping检测
        /// <summary>
        /// PING IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool Ping(string ip)
        {
            if (IPAddress.TryParse(ip, out IPAddress? ipAddress))
            {
                return Ping(ipAddress);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// PING IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        /// <returns></returns>
        public static bool Ping(IPAddress ip)
        {

            try
            {
                Ping ping = new();
                PingReply pingReply = ping.Send(ip, 5000);//ping 目标 IP 超时时间 5秒
                return pingReply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
