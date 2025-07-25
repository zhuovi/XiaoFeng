
#if NETFRAMEWORK
using System.Runtime.InteropServices;
using System;
using System.Text;
namespace XiaoFeng
{
    /// <summary>
    /// INI 操作类
    /// </summary>
    public class INIHelper
    {
#region 声明读写INI文件的API函数
        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileIntA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int GetPrivateProfileInt(string lpApplicationName, string lpKeyName, int nDefault, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileSectionsNamesA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int GetPrivateProfileSectionsNames(byte[] lpszReturnBuffer, int nSize, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        
        [DllImport("KERNEL32")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpszKey, string lpString, Byte[] lpStruct, int uSizeStruct, string lpFileName);
        
        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStructA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int GetPrivateProfileStruct(string lpszSections, string lpszKey, byte[] lpStruct, int uSizeStruct, string szFile);

        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileSectionsA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int WritePrivateProfileSectionsA(string lpAppName, string lpString, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileStringA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileStructA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int WritePrivateProfileStruct(string lpszSections, string lpszKey, byte[] lpStruct, int uSizeStruct, string szFile);


#endregion

#region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public INIHelper() { }
        /// <summary>
        /// 无参构造器
        /// </summary>
        /// <param name="path">文件路径 绝对路径</param>
        public INIHelper(string path)
        {
            if (System.IO.File.Exists(path))
            {
                this.FPath = path;
            }
            else
            {
                this.ErrorMessage = "您输入的配置文件不存在.";
                this.HPath = path;
            }
        }
#endregion

#region 属性
        /// <summary>
        /// 文件路径
        /// </summary>
        private string _HPath = "";
        /// <summary>
        /// 文件路径
        /// </summary>
        public string HPath { get { return _HPath; } set { _HPath = value; } }
        /// <summary>
        /// 文件路径 绝对路径
        /// </summary>
        private string _FPath = "";
        /// <summary>
        /// 文件路径 绝对路径
        /// </summary>
        public string FPath { get { return _FPath; } set { _FPath = value; } }
        /// <summary>
        /// 错误信息
        /// </summary>
        private string _ErrorMessage = "";
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get { return _ErrorMessage; } set { _ErrorMessage = value; } }
#endregion

#region 方法
        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="section">段落，格式[]</param>
        /// <param name="key">键</param>
        /// <param name="iValue">值</param>
        public void WriteValue(string section, string key, string iValue)
        {
            if (this.FPath == "")
            {
                using (var fs = System.IO.File.Create(HPath))
                    this.FPath = this.HPath;
            }
            WritePrivateProfileString(section, key, iValue, this.FPath);
        }

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">段落，格式[]</param>
        /// <param name="key">键</param>
        /// <returns>返回的键值</returns>
        public string ReadValue(string section, string key)
        {
            if (this.FPath == "") return "";
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, this.FPath);
            return temp.ToString();
        }

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">段落，格式[]</param>
        /// <param name="key">键</param>
        /// <returns>返回byte类型的section组或键值组</returns>
        public byte[] ReadValues(string section, string key)
        {
            byte[] temp = new byte[255];
            GetPrivateProfileString(section, key, "", temp, 255, this.FPath);
            return temp;
        }
#endregion
    }
}
#endif