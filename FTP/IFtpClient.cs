using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/1/25 14:08:55          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.FTP
{
    /// <summary>
    /// FTP客户端接口
    /// </summary>
    public interface IFtpClient
    {
        #region 属性
        /// <summary>
        /// FTP客户端配置
        /// </summary>
        FtpClientConfig Config { get; set; }
        /// <summary>
        /// 取消通知
        /// </summary>
        CancellationToken Token { get; set; }
        /// <summary>
        /// 进行控制连接的socket
        /// </summary>
        Socket SocketControl { get; set; }
        /// <summary>
        /// 是否连接
        /// </summary>
        bool Connected { get; set; }
        /// <summary>
        /// 服务器目录
        /// </summary>
        string RemoteDirectory { get; set; }

        #endregion

        #region 方法

        #region 连接
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="remoteFullName">服务器文件地址</param>
        /// <returns></returns>
        FtpWebRequest Create(string remoteFullName);
        #endregion

        #region 获取响应内容
        /// <summary>
        /// 获取响应流
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        FtpWebResponse GetFtpResponse(FtpWebRequest request);
        /// <summary>
        /// 获取响应内容
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        string GetFtpResonseResult(FtpWebRequest request);
        #endregion

        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localFile">本地文件信息</param>
        /// <param name="remoteFile">服务器文件名称</param>
        /// <returns></returns>
        string Upload(string localFile, string remoteFile);
        #endregion

        #region 连接
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="token">取消通知</param>
        /// <returns></returns>
        Task<bool> ConnectAsync(CancellationToken token = default);
        #endregion

        #region 关闭连接
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task DisConnect(CancellationToken token = default);
        #endregion

        #region 设置传输模式
        /// <summary>
        /// 设置传输模式
        /// </summary>
        /// <param name="tType">传输模式</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task SetTransferTypeAsync(TransferType tType, CancellationToken token = default);
        #endregion

        #region 文件操作

        #region 获得文件列表
        /// <summary>
        /// 获得文件列表
        /// </summary>
        /// <param name="strMask">文件名的匹配字符串</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<List<string>> GetDirAsync(string strMask, CancellationToken token = default);
        /// <summary>
        /// 获得文件列表
        /// </summary>
        /// <param name="strMask">文件名的匹配字符串</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<List<Catalog>> GetDirListAsync(string strMask, CancellationToken token = default);
        #endregion

        #region 获取文件大小
        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="strFileName">文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns>文件大小</returns>
        Task<long> GetFileSizeAsync(string strFileName, CancellationToken token = default);
        #endregion

        #region 获取文件信息
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="strFileName">文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns>文件大小</returns>
        Task<string> GetFileInfoAsync(string strFileName, CancellationToken token = default);
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="strFileName">待删除文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string strFileName, CancellationToken token = default);
        #endregion

        #region 重命名(如果新文件名与已有文件重名,将覆盖已有文件)
        /// <summary>
        /// 重命名(如果新文件名与已有文件重名,将覆盖已有文件)
        /// </summary>
        /// <param name="strOldFileName">旧文件名</param>
        /// <param name="strNewFileName">新文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<bool> RenameAsync(string strOldFileName, string strNewFileName, CancellationToken token = default);
        #endregion

        #endregion

        #region 上传和下载

        #region 下载一批文件
        /// <summary>
        /// 下载一批文件
        /// </summary>
        /// <param name="strFileNameMask">文件名的匹配字符串</param>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task DownFilesAsync(string strFileNameMask, string strFolder, CancellationToken token = default);
        #endregion

        #region 下载一个文件
        /// <summary>
        /// 下载一个文件
        /// </summary>
        /// <param name="strRemoteFileName">要下载的文件名</param>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="strLocalFileName">保存在本地时的文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<bool> DownFileAsync(string strRemoteFileName, string strFolder, string strLocalFileName, CancellationToken token = default);
        #endregion

        #region 下载一个文件
        /// <summary>
        /// 下载一个文件
        /// </summary>
        /// <param name="strRemoteFileName">要下载的文件名</param>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="strLocalFileName">保存在本地时的文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<bool> DownFileNoBinaryAsync(string strRemoteFileName, string strFolder, string strLocalFileName, CancellationToken token = default);
        #endregion

        #region 上传文件夹内所有文件到指定目录
        /// <summary>
        /// 上传文件夹内所有文件到指定目录
        /// </summary>
        /// <param name="localFolder">本地文件夹</param>
        /// <param name="remoteFolder">服务器文件夹</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task UploadFolderAsync(string localFolder, string remoteFolder, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 上传文件到指定目录
        /// </summary>
        /// <param name="localFile">本地文件</param>
        /// <param name="remoteFolder">服务器文件夹</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<Boolean> UploadFileAsync(string localFile, string remoteFolder, CancellationToken token = default(CancellationToken));
        #endregion

        #region 上传一批文件
        /// <summary>
        /// 上传一批文件
        /// </summary>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="strFileNameMask">文件名匹配字符(可以包含*和?)</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task PutAsync(string strFolder, string strFileNameMask, CancellationToken token = default);
        #endregion

        #region 上传一个文件
        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="FileName">本地文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<bool> PutAsync(string FileName, CancellationToken token = default);
        #endregion

        #region 上传一个文件
        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="FileName">本地文件名</param>
        /// <param name="strGuid">服务器文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<Boolean> PutByGuid(string FileName, string strGuid, CancellationToken token = default(CancellationToken));
        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="FileName">本地文件名</param>
        /// <param name="strGuid">服务器文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<Boolean> NewPutByGuid(string FileName, string strGuid, CancellationToken token = default(CancellationToken));
        #endregion

        #endregion

        #region 目录操作

        #region 创建目录
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="DirName">目录名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<Boolean> MakeDirectoryAsync(string DirName, CancellationToken token = default(CancellationToken));
        #endregion

        #region 删除目录
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="DirName">目录名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<Boolean> RemoveDirectory(string DirName, CancellationToken token = default(CancellationToken));
        #endregion

        #region 改变目录
        /// <summary>
        /// 改变目录
        /// </summary>
        /// <param name="NewDirName">新的工作目录名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<Boolean> ChangeDirectoryAsync(string NewDirName, CancellationToken token = default(CancellationToken));
        #endregion

        #endregion

        #region 将一行应答字符串记录在Reply和Msg,应答码记录在iReplyCode
        /// <summary>
        /// 获取应答信息
        /// </summary>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<FtpResponse> GetResponseAsync(CancellationToken token = default(CancellationToken));
        #endregion

        #region 建立进行数据连接的socket
        /// <summary>
        /// 建立进行数据连接的socket
        /// </summary>
        /// <param name="token">取消标识</param>
        /// <returns>数据连接socket</returns>
        Task<Socket> CreateDataSocketAsync(CancellationToken token = default(CancellationToken));
        #endregion

        #region 关闭socket连接(用于登录以前)
        /// <summary>
        /// 关闭socket连接(用于登录以前)
        /// </summary>
        void CloseSocketConnect();
        #endregion

        #region 发送命令并获取应答码和最后一行应答字符串
        /// <summary>
        /// 发送命令并获取应答码和应答字符串
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task<FtpResponse> SendCommandAsync(FtpCommandEnvelope command, CancellationToken token = default(CancellationToken));
        #endregion

        #region 处理消息
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="msg">消息</param>
        void Message(string msg);
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="format">格式</param>
        /// <param name="param">参数组</param>
        void Message(string format, params object[] param);
        #endregion

        #endregion
    }
}