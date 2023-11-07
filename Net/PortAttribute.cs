using System;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-11 23:12:59                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Net
{
    /// <summary>
    /// 网络端口属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class PortAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 设置网络端口
        /// </summary>
        /// <param name="port">网络端口</param>
        public PortAttribute(int port)
        {
            this.Port = port;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}