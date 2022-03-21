using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-09 19:39:23                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 命令请求类
    /// </summary>
    public class Command
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="datas">参数集</param>
        public Command(CommandType commandType, params object[] datas)
        {
            this.CommandType = commandType;
            this.Datas = datas;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 网络流
        /// </summary>
        public NetworkStream Stream { get; set; }
        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// 命令数据
        /// </summary>
        public object[] Datas { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 命令行
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var line = $"*{this.Datas.Length + 1}\r\n${this.CommandType.ToString().Length}\r\n{this.CommandType}\r\n";
            this.Datas.Each(d =>
            {
                line += $"${Encoding.UTF8.GetByteCount(d.ToString())}\r\n{d}\r\n";
            });
            return line;
        }
        /// <summary>
        /// 命令行字节组
        /// </summary>
        public byte[] ToBytes() => this.ToString().GetBytes();
        #endregion
    }
}