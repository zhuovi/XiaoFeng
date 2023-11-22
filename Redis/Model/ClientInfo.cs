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
        [Json.JsonElement("addr")]
        public string Addr { get; set; }
        /// <summary>
        ///  套接字所使用的文件描述符
        /// </summary>
        [Json.JsonElement("fd")] 
        public string Fd { get; set; }
        /// <summary>
        ///  以秒计算的已连接时长
        /// </summary>
        [Json.JsonElement("age")] 
        public string Age { get; set; }
        /// <summary>
        ///  以秒计算的空闲时长
        /// </summary>
        [Json.JsonElement("idle")] 
        public string Idle { get; set; }
        /// <summary>
        ///  客户端 flag
        /// </summary>
        [Json.JsonElement("flags")] 
        public string Flags { get; set; }
        /// <summary>
        ///  该客户端正在使用的数据库 ID
        /// </summary>
        [Json.JsonElement("db")] 
        public string Db { get; set; }
        /// <summary>
        ///  已订阅频道的数量
        /// </summary>
        [Json.JsonElement("sub")] 
        public string Sub { get; set; }
        /// <summary>
        ///  已订阅模式的数量
        /// </summary>
        [Json.JsonElement("psub")] 
        public string Psub { get; set; }
        /// <summary>
        ///  在事务中被执行的命令数量
        /// </summary>
        [Json.JsonElement("multi")] 
        public string Multi { get; set; }
        /// <summary>
        ///  查询缓冲区的长度（字节为单位， 0 表示没有分配查询缓冲区）
        /// </summary>
        [Json.JsonElement("qbuf")] 
        public string Qbuf { get; set; }
        /// <summary>
        ///  查询缓冲区剩余空间的长度（字节为单位， 0 表示没有剩余空间）
        /// </summary>
        [Json.JsonElement("qbuf-free")]
        public string Qbuffree { get; set; }
        /// <summary>
        ///  输出缓冲区的长度（字节为单位， 0 表示没有分配输出缓冲区）
        /// </summary>
        [Json.JsonElement("obl")]
        public string Obl { get; set; }
        /// <summary>
        ///   输出列表包含的对象数量（当输出缓冲区没有剩余空间时，命令回复会以字符串对象的形式被入队到这个队列里）
        /// </summary>
        [Json.JsonElement("oll")] 
        public string Oll { get; set; }
        /// <summary>
        ///  输出缓冲区的长度（字节为单位， 0 表示没有分配输出缓冲区）
        /// </summary>
        [Json.JsonElement("omem")] 
        public string Omem { get; set; }
        /// <summary>
        ///  文件描述符事件
        /// </summary>
        [Json.JsonElement("events")] 
        public string Events { get; set; }
        /// <summary>
        ///  最近一次执行的命令
        /// </summary>
        [Json.JsonElement("cmd")] 
        public string Cmd { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}