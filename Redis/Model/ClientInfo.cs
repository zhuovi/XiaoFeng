using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-20 上午 12:55:26                       *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 客户端信息类
    /// </summary>
    public class ClientInfo
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ClientInfo()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 客户端的地址和端口
        /// </summary>
        public string addr { get; set; }
        /// <summary>
        ///  套接字所使用的文件描述符
        /// </summary>
        public string fd { get; set; }
        /// <summary>
        ///  以秒计算的已连接时长
        /// </summary>
        public string age { get; set; }
        /// <summary>
        ///  以秒计算的空闲时长
        /// </summary>
        public string idle { get; set; }
        /// <summary>
        ///  客户端 flag
        /// </summary>
        public string flags { get; set; }
        /// <summary>
        ///  该客户端正在使用的数据库 ID
        /// </summary>
        public string db { get; set; }
        /// <summary>
        ///  已订阅频道的数量
        /// </summary>
        public string sub { get; set; }
        /// <summary>
        ///  已订阅模式的数量
        /// </summary>
        public string psub { get; set; }
        /// <summary>
        ///  在事务中被执行的命令数量
        /// </summary>
        public string multi { get; set; }
        /// <summary>
        ///  查询缓冲区的长度（字节为单位， 0 表示没有分配查询缓冲区）
        /// </summary>
        public string qbuf { get; set; }
        /// <summary>
        ///  查询缓冲区剩余空间的长度（字节为单位， 0 表示没有剩余空间）
        /// </summary>
        [Json.JsonElement("qbuf-free")]
        public string qbuffree { get; set; }
        /// <summary>
        ///  输出缓冲区的长度（字节为单位， 0 表示没有分配输出缓冲区）
        /// </summary>
        public string obl { get; set; }
        /// <summary>
        ///   输出列表包含的对象数量（当输出缓冲区没有剩余空间时，命令回复会以字符串对象的形式被入队到这个队列里）
        /// </summary>
        public string oll { get; set; }
        /// <summary>
        ///  输出缓冲区的长度（字节为单位， 0 表示没有分配输出缓冲区）
        /// </summary>
        public string omem { get; set; }
        /// <summary>
        ///  文件描述符事件
        /// </summary>
        public string events { get; set; }
        /// <summary>
        ///  最近一次执行的命令
        /// </summary>
        public string cmd { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}