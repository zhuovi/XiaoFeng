using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using XiaoFeng;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using XiaoFeng.Web;
using System.Management;
using XiaoFeng.IO;
namespace XiaoFeng.Device
{
    /*
    ===================================================================
        Author : jacky
        Email : jacky @zhuovi.com
        QQ : 7092734
        Site : www.zhuovi.com
        Create Time : 2017/9/17 23:56:56
        Update Time : 2017/9/17 23:56:56
    ===================================================================
    */
    /// <summary>
    /// 电脑操作类
    /// Verstion : 1.0.1
    /// Author : jacky
    /// Email : jacky@zhuovi.com
    /// QQ : 7092734
    /// Site : www.zhuovi.com
    /// Create Time : 2017/9/17 23:56:56
    /// Update Time : 2018/07/03 17:47:25
    /// </summary>
    public class Computer
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Computer() { }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #region 获取网卡硬件地址
        /// <summary>
        /// 获取网卡硬件地址
        /// </summary>
        /// <returns></returns>
        public static string MacAddress
        {
            get
            {
                var separator = "-";
                string mac = string.Empty;
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                if (nics == null || nics.Length < 1)
                {
                    return "";
                }

                var macAddress = new List<string>();

                //Debug.WriteLine(" Number of interfaces .................... : {0}", nics.Length);
                foreach (NetworkInterface adapter in nics.Where(c =>
                 c.NetworkInterfaceType != NetworkInterfaceType.Loopback && c.OperationalStatus == OperationalStatus.Up))
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();

                    var unicastAddresses = properties.UnicastAddresses;
                    if (unicastAddresses.Any(temp => temp.Address.AddressFamily == AddressFamily.InterNetwork))
                    {
                        var address = adapter.GetPhysicalAddress();
                        if (separator.IsNullOrEmpty())
                        {
                            macAddress.Add(address.ToString());
                        }
                        else
                        {
                            macAddress.Add(address.GetAddressBytes().Join(separator));
                        }
                    }
                }
                return mac;
            }
        }
        #endregion

        #region 获得IP4地址
        /// <summary>
        /// 获得IP4地址
        /// </summary>
        /// <returns></returns>
        public static string GetIP4Address()
        {
            try
            {
                return HttpContext.Current.
#if NETFRAMEWORK
                Request.UserHostAddress;
#else
                Connection.RemoteIpAddress.ToString();
#endif
            }
            catch
            {
                return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            }
        }
        #endregion

        #region 获取CPU ID
        /// <summary>
        /// 获取 CPU ID
        /// </summary>
        /// <returns></returns>
        public static string GetCpuID()
        {
            try
            {
                string strCpuID = string.Empty;

                if (OS.Platform.GetOSPlatform() == PlatformOS.Windows)
                {
                    ManagementClass mc = new ManagementClass("Win32_Processor");
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                    }
                    moc = null;
                    mc = null;
                }
                else
                {
                    string CPU_FILE_PATH = "/proc/cpuinfo";
                    var s = FileHelper.OpenText(CPU_FILE_PATH);
                    var lines = s.Split(new[] { '\n' });
                    foreach (var item in lines)
                    {
                        if (item.StartsWith("Serial"))
                        {
                            var temp = item.Split(new[] { ':' });
                            strCpuID = temp[1].Trim();
                            break;
                        }
                    }
                }
                return strCpuID;
            }
            catch
            {
                return "unknown";
            }
        }
        #endregion

        #region 操作系统的登录用户名
        /// <summary>
        /// 操作系统的登录用户名
        /// </summary>
        /// <returns></returns>
        public static string UserName
        {
            get
            {
                return Environment.UserName;
            }
        }
        #endregion

        #region 获取计算机名
        /// <summary>
        /// 获取计算机名
        /// </summary>
        /// <returns></returns>
        public static string ComputerName
        {
            get { return Environment.MachineName; }
        }
        #endregion

        #region 物理内存
        /// <summary>
        /// 物理内存
        /// </summary>
        /// <returns></returns>
        public static MemoryInfo GetPhysicalMemory()
        {
            MemoryInfo mm = new MemoryInfo();

            if (OS.Platform.GetOSPlatform() == PlatformOS.Windows)
            {
                ManagementClass mc = new ManagementClass("Win32_OperatingSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                double sizeAll = 0.0;
                double freeSizeAll = 0.0;
                foreach (ManagementObject m in moc)
                {
                    if (m.Properties["TotalVisibleMemorySize"].Value != null)
                    {
                        sizeAll += m.Properties["TotalVisibleMemorySize"].Value.ToCast<double>() * 1024;
                    }
                    if (m.Properties["FreePhysicalMemory"].Value != null)
                    {
                        freeSizeAll += m.Properties["FreePhysicalMemory"].Value.ToCast<double>() * 1024;
                    }
                }
                mm.Total = sizeAll;
                mm.Available = freeSizeAll;

                moc.Dispose();
            }
            else
            {
                string CPU_FILE_PATH = "/proc/meminfo";
                var mem_file_info = FileHelper.OpenText(CPU_FILE_PATH);
                var lines = mem_file_info.Split(new[] { '\n' });
                int count = 0;
                foreach (var item in lines)
                {
                    if (item.StartsWith("MemTotal:"))
                    {
                        count++;
                        var tt = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        mm.Total = tt[1].Trim().ToCast<double>();
                    }
                    else if (item.StartsWith("MemAvailable:"))
                    {
                        count++;
                        var tt = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        mm.Available = tt[1].Trim().ToCast<double>();
                    }
                    if (count >= 2) break;
                }
            }
            return mm;
        }
        #endregion

        #region  获取主板ID
        /// <summary>
        /// 获取主板ID
        /// </summary>
        /// <returns></returns>
        public static string GetMotherBoardID()
        {
            try
            {
                string strID = string.Empty;
                if (OS.Platform.GetOSPlatform() == PlatformOS.Windows)
                {
                    ManagementClass mc = new ManagementClass("Win32_BaseBoard");
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        strID = mo.Properties["SerialNumber"].Value.ToString();
                        break;
                    }
                }
                else
                {

                }

                return strID;
            }
            catch
            {
                return "unknown";
            }
        }
        #endregion

        #endregion
    }

    #region 内存数据
    /// <summary>
    /// 内存数据
    /// </summary>
    public class MemoryInfo
    {
        /// <summary>
        /// 总计内存大小
        /// </summary>
        public double Total { get; set; }
        /// <summary>
        /// 可用内存大小
        /// </summary>
        public double Available { get; set; }
    }
    #endregion
}