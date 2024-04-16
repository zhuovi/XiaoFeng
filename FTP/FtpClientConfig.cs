using System;
using System.ComponentModel;
using System.Net;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/1/25 14:07:52          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.FTP
{
    /// <summary>
    /// FTP客户端配置
    /// </summary>
    public class FtpClientConfig
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public FtpClientConfig()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 账号
        /// </summary>
        [Description("账号")]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Description("密码")]
        public string Password { get; set; }
        /// <summary>
        /// 主机
        /// </summary>
        [Description("主机")]
        public string Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        [Description("端口")]
        public int Port { get; set; } = 21;
        /// <summary>
        /// 连接超时
        /// </summary>
        [Description("连接超时时长,单位是毫秒")]
        public int Timeout { get; set; } = 100_000;
        /// <summary>
        /// 读写超时时长,单位是毫秒
        /// </summary>
        [Description("读写超时时长,单位是毫秒")]
        public int ReadWriteTimeout { get; set; } = 300_000;
        /// <summary>
        /// 断开超时时长
        /// </summary>
        [Description("断开超时时长,单位是毫秒")]
        public int DisconnectTimeout { get; set; } = 300_000;
        /// <summary>
        /// 接收超时时长
        /// </summary>
        [Description("接收超时时长,单位是毫秒")]
        public int ReceiveTimeout { get; set; } = 300;
        /// <summary>
        /// 发送超时时长
        /// </summary>
        [Description("发送超时时长,单位是毫秒")]
        public int SendTimeout { get; set; } = 300;
        /// <summary>
        /// 重试连接次数
        /// </summary>
        [Description("重试连接次数")]
        public int ConnectCount { get; set; }
        /// <summary>
        /// 本地目录
        /// </summary>
        [Description("本地目录")]
        public string LocalDirectory { get; set; }
        /// <summary>
        /// 服务器目录
        /// </summary>
        public string RemoteDirectory { get; set; } = "/";
        /// <summary>
        /// 编码名称
        /// </summary>
        [Description("编码名称")]
        public string CharsetName { get; set; }
        /// <summary>
        /// 代理地址
        /// </summary>
        [Description("代理地址")]
        public string ProxyHost { get; set; }
        /// <summary>
        /// 代理端口
        /// </summary>
        [Description("代理端口")]
        public int ProxyPort { get; set; }
        /// <summary>
        /// 代理账号
        /// </summary>
        [Description("代理账号")]
        public string ProxyUserName { get; set; }
        /// <summary>
        /// 代理密码
        /// </summary>
        [Description("代理密码")]
        public string ProxyPassword { get; set; }
        /// <summary>
        /// 是否使用SSL链接
        /// </summary>
        [Description("是否使用SSL链接")]
        public Boolean EnableSSL { get; set; } = false;
        /// <summary>
        /// 是否允许二进制
        /// </summary>
        [Description("是否允许二进制")]
        public Boolean UseBinary { get; set; } = true;
        /// <summary>
        /// 是否允许被动式
        /// </summary>
        [Description("是否允许被动式")]
        public Boolean UsePassive { get; set; } = true;
        /// <summary>
        /// 是否请求完成关闭链接
        /// </summary>
        [Description("是否请求完成保持链接")]
        public Boolean KeepAlive { get; set; } = true;
        #endregion

        #region 方法
        /// <summary>
        /// 获取代理
        /// </summary>
        /// <returns></returns>
        public WebProxy GetProxy()
        {
            if (this.ProxyHost.IsNullOrEmpty()) return null;
            var proxy = new WebProxy();
            if (this.ProxyHost.IsSite())
            {
                proxy.Address = new Uri(this.ProxyHost);
            }
            if (this.ProxyPort > 0) proxy = new WebProxy(this.ProxyHost, this.ProxyPort);
            if (this.ProxyUserName.IsNotNullOrEmpty() && this.ProxyPassword.IsNotNullOrEmpty())
                proxy.Credentials = new NetworkCredential(this.ProxyUserName, this.ProxyPassword);
            return proxy;
        }
        #endregion
    }
}