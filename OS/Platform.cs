using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
namespace XiaoFeng.OS
{
    /// <summary>
    /// 运行平台参数
    /// </summary>
    public class Platform : Disposable
    {
        #region 获取平台操作系统
        /// <summary>
        /// 获取平台操作系统
        /// </summary>
        /// <returns></returns>
        public static PlatformOS GetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return PlatformOS.Linux;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return PlatformOS.OSX;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return PlatformOS.Windows;
            return PlatformOS.Windows;
        }
        #endregion

        #region 服务器名称
        /// <summary>
        /// 服务器名称
        /// </summary>
        public static string MachineName => Environment.MachineName;
        /// <summary>
        /// 系统名称
        /// </summary>
        public static string OSDescription => RuntimeInformation.OSDescription;
        #endregion

        #region 系统及版本
        /// <summary>
        /// 系统及版本
        /// </summary>
        public static string OSVersion => Environment.OSVersion.ToString();
        #endregion

        #region 系统框架
        /// <summary>
        /// 系统框架
        /// </summary>
        public static string FrameworkDescription => RuntimeInformation.FrameworkDescription;
        #endregion

        #region 系统架构
        /// <summary>
        /// 系统架构
        /// </summary>
        public static Architecture OSArchitecture => RuntimeInformation.OSArchitecture;
        /// <summary>
        /// 进程架构
        /// </summary>
        public static Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;
        #endregion

        #region 当前项目目录
        /// <summary>
        /// 当前项目目录
        /// </summary>
        public static string CurrentDirectory =>
        //Directory.GetCurrentDirectory();        
#if NETFRAMEWORK
            AppDomain.CurrentDomain.BaseDirectory;
#else
        //AppDomain.CurrentDomain.BaseDirectory.RemovePattern(@"\\bin\\Debug[\s\S]*$");
        Directory.GetCurrentDirectory(); 
#endif
        /// <summary>
        /// 系统目录
        /// </summary>
        public static string SystemDirectory => Environment.SystemDirectory;
        #endregion

        #region 是WebForm还是WinForm
        /// <summary>
        /// 是WebForm还是WinForm
        /// </summary>
        private static Boolean? _IsWebForm;
        /// <summary>
        /// 是WebForm还是WinForm
        /// </summary>
        public static Boolean IsWebForm =>  /*XiaoFeng.Web.HttpContext.Current != null*/(bool)(_IsWebForm ?? (_IsWebForm = !AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == "System.Windows.Forms").Any()));
        #endregion
    }
}