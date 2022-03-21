using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Net
{
    #region 客户端Session
    /// <summary>
    /// 客户端Session
    /// </summary>
    public class ClientSession : NetSession, IClientSession
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ClientSession() { }
        #endregion

        #region 属性
        /// <summary>
        /// Cookie
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// Origin
        /// </summary>
        public string Origin { get; set; }
        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Path
        /// </summary>
        public string Path { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            base.Close();

        }
        #endregion
    }
    #endregion
}