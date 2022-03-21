using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/1/26 14:07:00          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.FTP
{
    /// <summary>
    /// FTP命令行封装包
    /// </summary>
    public class FtpCommandEnvelope
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public FtpCommandEnvelope()
        {

        }
        /// <summary>
        /// 设置命令数据
        /// </summary>
        /// <param name="command">FTP命令</param>
        /// <param name="data">数据</param>
        public FtpCommandEnvelope(FtpCommand command, string data)
        {
            this.FtpCommand = command;
            this.Data = data;
        }
        /// <summary>
        /// 设置命令数据
        /// </summary>
        /// <param name="command">FTP命令</param>
        public FtpCommandEnvelope(FtpCommand command) : this(command, "") { }
        #endregion

        #region 属性
        /// <summary>
        /// FTP命令
        /// </summary>
        public FtpCommand FtpCommand { get; set; }
        /// <summary>
        /// FTP数据
        /// </summary>
        public string Data { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 重新转换成字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{0}{1}\r\n".format(this.FtpCommand.ToString(), this.Data.IsNullOrEmpty() ? "" : (" " + this.Data));
        }
        /// <summary>
        /// 获取字节
        /// </summary>
        public byte[] ToBytes() => Encoding.Default.GetBytes(this.ToString());
        #endregion
    }
}