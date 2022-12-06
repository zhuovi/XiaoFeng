/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-09-18 00:51:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.FTP
{
    /// <summary>
    /// FTP命令
    /// </summary>
    public enum FtpCommand
    {
        /// <summary>
        /// 打开
        /// </summary>
        OPEN,
        /// <summary>
        /// 等待
        /// </summary>
        NOOP,
        /// <summary>
        /// 用户
        /// </summary>
        USER,
        /// <summary>
        /// 密码
        /// </summary>
        PASS,
        /// <summary>
        /// 退出
        /// </summary>
        QUIT,
        /// <summary>
        /// EPRT模式建立连接
        /// </summary>
        EPRT,
        /// <summary>
        /// EPSV模式建立连接
        /// </summary>
        EPSV,
        /// <summary>
        /// PASV模式建立连接
        /// </summary>
        PASV,
        /// <summary>
        /// 改变工作目录
        /// </summary>
        CWD,
        /// <summary>
        /// 打印工作目录
        /// </summary>
        PWD,
        /// <summary>
        /// 
        /// </summary>
        CLNT,
        /// <summary>
        /// 文件简单列表
        /// </summary>
        NLST,
        /// <summary>
        /// 文件详情列表
        /// </summary>
        LIST,
        /// <summary>
        /// 目录列表
        /// </summary>
        MLSD,
        /// <summary>
        /// 下载文件
        /// </summary>
        RETR,
        /// <summary>
        /// 上传文件
        /// </summary>
        STOR,
        /// <summary>
        /// 删除文件
        /// </summary>
        DELE,
        /// <summary>
        /// 创建目录
        /// </summary>
        MKD,
        /// <summary>
        /// 删除目录
        /// </summary>
        RMD,
        /// <summary>
        /// 重命名
        /// </summary>
        RNFR,
        /// <summary>
        /// 重命名 覆盖原文件
        /// </summary>
        RNTO,
        /// <summary>
        /// 获取文件大小
        /// </summary>
        SIZE,
        /// <summary>
        /// 文件类型
        /// </summary>
        TYPE,
        /// <summary>
        /// 
        /// </summary>
        FEAT,
        /// <summary>
        /// 
        /// </summary>
        PBSZ,
        /// <summary>
        /// 端口
        /// </summary>
        PORT
    }
}