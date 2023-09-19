using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 17:59:28                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Internal
{
    /// <summary>
    /// 基础命令
    /// </summary>
    public abstract class BaseCommand
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BaseCommand()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// Socket
        /// </summary>
        public ISocketClient SocketClient { get; set; }
        /// <summary>
        /// 命令
        /// </summary>
        public Protocol.CommandType CommandType { get; set; }
        /// <summary>
        /// 结束符
        /// </summary>
        public string CRLF = "\r\n";
        #endregion

        #region 方法
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public abstract Task<GetOperationResult> GetGetResponseAsync();
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public abstract Task<StoreOperationResult> GetStoreResponseAsync();
        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public abstract Task<StatsOperationResult> GetStatResponseAsync();
        #endregion
    }
}