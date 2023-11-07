/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
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