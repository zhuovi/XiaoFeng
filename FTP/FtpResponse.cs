using System;
using System.Net;
using System.Text;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/1/26 11:29:08          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.FTP
{
    /// <summary>
    /// FTP响应模型
    /// </summary>
    public class FtpResponse
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public FtpResponse()
        {
            this.StatusCode = FtpStatusCode.Undefined;
        }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="msg">消息</param>
        public FtpResponse(string msg)
        {
            if (msg.IsNullOrEmpty()) return;
            this.Data = msg.GetBytes(Encoding.ASCII);
            if (msg.IsMatch(@"^\d{3}[-\s][\s\S]*$"))
            {
                var dict = msg.GetMatchs(@"^(?<status>\d{3})[-\s](?<msg>[\s\S]*)$");
                this.StatusCode = dict["status"].ToCast<FtpStatusCode>();
                this.Message = dict["msg"];
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 状态码
        /// </summary>
        public FtpStatusCode StatusCode { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 接收数据
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public Boolean IsSuccess
        {
            get
            {
                int statusCode = (int)StatusCode;
                return statusCode >= 100 && statusCode < 400;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 空响应
        /// </summary>
        public static FtpResponse EmptyResponse = new FtpResponse
        {
            Message = "No response was received",
            StatusCode = FtpStatusCode.Undefined
        };
        #endregion
    }
}