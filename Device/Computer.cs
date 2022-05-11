using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-04-25 15:59:13                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Device
{
    /// <summary>
    /// Computer 类说明
    /// </summary>
    public static class Computer
    {
        #region 属性

        #endregion

        #region 方法

        #region 获取网卡硬件地址
        /// <summary>
        /// 获取网卡硬件地址
        /// </summary>
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

        #region 获取CPU ID
        /// <summary>
        /// 获取 CPU ID
        /// </summary>
        public static string CpuId
        {
            get
            {
                var platform = OS.Platform.GetOSPlatform();
                var result = CmdExecute(platform == PlatformOS.Windows ? "wmic CPU get ProcessorID" : "dmidecode -t 4 | grep ID |sort -u |awk -F': ' '{print $2}'");
                return result.RemovePattern(@"^ProcessorID").RemovePattern(@"[\r\n\s]+");
            }
        }
        #endregion

        #region 主板序列号
        /// <summary>
        /// 主板序列号
        /// </summary>
        public static string BaseBoardSerialNumber
        {
            get
            {
                var platform = OS.Platform.GetOSPlatform();
                var result = CmdExecute(platform == PlatformOS.Windows ? "wmic baseboard get SerialNumber" : "dmidecode -s baseboard-serial-number");
                return result.RemovePattern(@"^ProcessorID").RemovePattern(@"[\r\n\s]+");
            }
        }
        #endregion

        #region 系统序列号
        /// <summary>
        /// 系统序列号
        /// </summary>
        public static string SystemSerialNumber
        {
            get
            {
                var platform = OS.Platform.GetOSPlatform();
                var result = CmdExecute(platform == PlatformOS.Windows ? "wmic bios get serialnumber" : "dmidecode -s system-serial-number");
                return result.RemovePattern(@"^ProcessorID").RemovePattern(@"[\r\n\s]+");
            }
        }
        #endregion

        #region 执行命令
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <returns></returns>
        public static string CmdExecute(string cmd)
        {
            string result = String.Empty;
            if (cmd.IsNullOrEmpty()) return result;
            try
            {
               using (var process = new Process())
                {
                    var fileName = string.Empty;
                    if (OS.Platform.GetOSPlatform() == PlatformOS.Windows)
                    {
                        fileName = "cmd.exe";
                        cmd = "/c " + cmd;
                    }
                    else
                    {
                        fileName = "/bin/bash";
                        cmd = "-c " + "\"" + cmd + "\"";
                    }
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = cmd,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        StandardErrorEncoding = Encoding.UTF8,
                        StandardOutputEncoding = Encoding.UTF8
                    };
                    process.Start();
                    process.StandardInput.AutoFlush = true;
                    result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(3000);
                    process.Kill();
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
            return result;
        }
        #endregion

        #endregion
    }
}