using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Event;
using XiaoFeng.IO;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/1/25 14:03:52                *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.FTP
{
    /// <summary>
    /// FTP客户端
    /// </summary>
    public class FtpClient : IFtpClient
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public FtpClient()
        {

        }
        /// <summary>
        /// 设置FTP配置
        /// </summary>
        /// <param name="config"></param>
        public FtpClient(FtpClientConfig config)
        {
            this.Config = config;
        }
        #endregion

        #region 属性
        /// <summary>
        /// FTP客户端配置
        /// </summary>
        public FtpClientConfig Config { get; set; }
        /// <summary>
        /// 发送信号
        /// </summary>
        protected readonly SemaphoreSlim SendSlim = new SemaphoreSlim(1, 1);
        /// <summary>
        /// 接收信号
        /// </summary>
        protected readonly SemaphoreSlim ReceiveSlim = new SemaphoreSlim(1, 1);
        /// <summary>
        /// 取消通知
        /// </summary>
        public CancellationToken Token { get; set; }
        /// <summary>
        /// 进行控制连接的socket
        /// </summary>
        public Socket SocketControl { get; set; }
        /// <summary>
        /// 接收和发送数据的缓冲区
        /// </summary>
        private Byte[] buffer = new Byte[4096];
        /// <summary>
        /// 是否连接
        /// </summary>
        public Boolean Connected { get; set; } = false;
        /// <summary>
        /// 服务器目录
        /// </summary>
        public string RemoteDirectory { get; set; }
        #endregion

        #region 事件

        /// <summary>
        /// 委托事件 接收消息
        /// </summary>
        public event MessageEventHandler OnMessage;
        /// <summary>
        /// 委托事件 发送消息
        /// </summary>
        public event MessageEventHandler OnSendByte;

        #endregion

        #region 方法

        #region 连接
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="remoteFullName">服务器文件地址</param>
        /// <returns></returns>
        public FtpWebRequest Create(string remoteFullName)
        {
            var FtpPath = "ftp://{0}:{1}{2}".format(this.Config.Host, this.Config.Port, remoteFullName);
            var request = (FtpWebRequest)WebRequest.Create(FtpPath);
            var Proxy = this.Config.GetProxy();
            if (Proxy != null) request.Proxy = Proxy;
            request.Credentials = new NetworkCredential(this.Config.UserName, this.Config.Password);
            request.KeepAlive = this.Config.KeepAlive;
            request.UseBinary = this.Config.UseBinary;
            request.EnableSsl = this.Config.EnableSSL;
            request.UsePassive = this.Config.UsePassive;
            request.Timeout = this.Config.Timeout;
            request.ReadWriteTimeout = this.Config.ReadWriteTimeout;

            return request;
        }
        #endregion

        #region 获取响应内容
        /// <summary>
        /// 获取响应流
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public FtpWebResponse GetFtpResponse(FtpWebRequest request) => (FtpWebResponse)request.GetResponse();
        /// <summary>
        /// 获取响应内容
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public string GetFtpResonseResult(FtpWebRequest request) => GetFtpResponse(request).GetResponseStream().ReadToEnd();
        #endregion

        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="localFile">本地文件信息</param>
        /// <param name="remoteFile">服务器文件名称</param>
        /// <returns></returns>
        public string Upload(string localFile, string remoteFile)
        {
            var file = new FileInfo(localFile);
            if (file.Exists)
            {
                FtpWebRequest request = this.Create(remoteFile);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                //上传数据
                using (Stream rs = request.GetRequestStream())
                using (FileStream fs = file.OpenRead())
                {
                    byte[] buffer = new byte[4096];//4K
                    int count = fs.Read(buffer, 0, buffer.Length);
                    while (count > 0)
                    {
                        rs.Write(buffer, 0, count);
                        count = fs.Read(buffer, 0, buffer.Length);
                    }
                    fs.Close();
                }
                return this.GetFtpResonseResult(request);
            }
            this.Message("本地文件不存在,文件路径:{0}", file.FullName);
            return "";
        }
        #endregion

        #region 连接
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="token">取消通知</param>
        /// <returns></returns>
        public async Task<Boolean> ConnectAsync(CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            await SendSlim.WaitAsync(token);
            this.SocketControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = this.Config.ReceiveTimeout,
                SendTimeout = this.Config.SendTimeout
            };
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(this.Config.Host), this.Config.Port);
            try
            {
                this.SocketControl.Connect(ep);
            }
            catch (Exception)
            {
                this.Message($"不能连接到 FTP[{this.Config.Host}:{this.Config.Port}] 服务器.");
                this.SocketControl = null;
                return false;
            }
            finally
            {
                SendSlim.Release();
                this.Connected = true;
            }
            var response = await this.GetResponseAsync(token);
            if (response.StatusCode != FtpStatusCode.SendUserCommand)
            {
                await this.DisConnect(token);
                this.Message(response.Message);
                this.SocketControl = null;
                this.Connected = false;
                return false;
            }
            response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.USER, this.Config.UserName), token);
            if (!(response.StatusCode == FtpStatusCode.SendPasswordCommand || response.StatusCode == FtpStatusCode.LoggedInProceed))
            {
                this.CloseSocketConnect();
                this.Message(response.Message);
                this.SocketControl = null;
                this.Connected = false;
                return false;
            }
            if (response.StatusCode != FtpStatusCode.LoggedInProceed)
            {
                response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.PASS, this.Config.Password), token);
                if (!(response.StatusCode == FtpStatusCode.LoggedInProceed || response.StatusCode == FtpStatusCode.CommandExtraneous))
                {
                    this.CloseSocketConnect();
                    this.Message(response.Message);
                    this.SocketControl = null;
                    this.Connected = false;
                    return false;
                }
            }
            return await this.ChangeDirectoryAsync(this.Config.RemoteDirectory, token);
        }
        #endregion

        #region 关闭连接
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task DisConnect(CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            if (this.SocketControl != null)
            {
                await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.QUIT), token);
            }
            this.CloseSocketConnect();
        }
        #endregion

        #endregion

        #region 设置传输模式
        /// <summary>
        /// 设置传输模式
        /// </summary>
        /// <param name="tType">传输模式</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task SetTransferTypeAsync(TransferType tType, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.TYPE, tType.GetDescription()), token);
            if (response.StatusCode != FtpStatusCode.CommandOK)
                this.Message(response.Message);
        }
        #endregion

        #region 文件操作

        #region 获得文件列表
        /// <summary>
        /// 获得文件列表
        /// </summary>
        /// <param name="strMask">文件名的匹配字符串</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<List<string>> GetDirAsync(string strMask, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            Socket socketData = await this.CreateDataSocketAsync(token);
            if (socketData == null) return null;
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.NLST, strMask), token);
            if (!(response.StatusCode == FtpStatusCode.OpeningData || response.StatusCode == FtpStatusCode.DataAlreadyOpen || response.StatusCode == FtpStatusCode.ClosingData))
            {
                this.Message(response.Message);
                return null;
            }
            var msg = "";
            //await Task.Delay(500);
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                //string line = ASCII.GetString(buffer, 0, iBytes);
                string line = buffer.GetString("", 0, iBytes);
                msg += line;
                if (iBytes < buffer.Length) break;
            }
            char[] seperator = { '\n' };
            string[] strsFileList = msg.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            if (socketData.Connected)
                socketData.Close(); //数据socket关闭时也会有返回码
            if (response.StatusCode != FtpStatusCode.ClosingData)
            {
                response = await this.GetResponseAsync(token);
                if (response.StatusCode != FtpStatusCode.ClosingData)
                {
                    this.Message(response.Message);
                    return null;
                }
            }
            return strsFileList.ToList();
        }
        /// <summary>
        /// 获得文件列表
        /// </summary>
        /// <param name="strMask">文件名的匹配字符串</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<List<Catalog>> GetDirListAsync(string strMask, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            List<Catalog> Result = new List<Catalog>();
            Socket socketData = await this.CreateDataSocketAsync(token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.LIST, strMask), token);
            if (!(response.StatusCode == FtpStatusCode.OpeningData || response.StatusCode == FtpStatusCode.DataAlreadyOpen || response.StatusCode == FtpStatusCode.ClosingData))
            {
                this.Message(response.Message);
                return null;
            }
            var msg = "";
            //Task.Delay(200).Wait();
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                //string line = ASCII.GetString(buffer, 0, iBytes);
                string line = buffer.GetString("", 0, iBytes);
                msg += line;

                Catalog fData = new Catalog
                {
                    CreateTime = line.GetMatch(@"^(?<a>\d{1,2}-\d{1,2}-\d{1,2}\s+\d{1,2}:\d{1,2}[AP]M)").ReplacePattern(@"(\d{1,2}-\d{1,2})-(\d{1,2})(\s+)", "$2-$1$3").ToDateTime(),
                    Name = line.GetMatch(@"\s+(?<a>[^\s]+)$"),
                    Length = line.GetMatch(@"\s+(?<a>\d+)\s+").ToInt32(),
                    Attribute = line.GetMatch(@"\s+(?<a>\<DIR\>)\s+").ToEnum<FileAttribute>()
                };
                fData.FullPath = this.RemoteDirectory.TrimEnd('/') + "/" + fData.Name;

                //Result.Add(line);
                Result.Add(fData);
                if (iBytes < buffer.Length) break;
            }
            //char[] seperator = { '\n' };
            //string[] strsFileList = this.Msg.Split(seperator);
            socketData.Close(); //数据socket关闭时也会有返回码
            if (response.StatusCode != FtpStatusCode.ClosingData)
            {
                response = await GetResponseAsync(token);
                if (response.StatusCode != FtpStatusCode.ClosingData)
                {
                    this.Message(response.Message);
                    return null;
                }
            }
            return Result;
            //return strsFileList;
        }
        #endregion

        #region 获取文件大小
        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="strFileName">文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns>文件大小</returns>
        public async Task<long> GetFileSizeAsync(string strFileName, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.SIZE, Path.GetFileName(strFileName)), token);
            if (response.StatusCode == FtpStatusCode.FileStatus)
            {
                return response.Message.ToCast<long>();
            }
            else
            {
                this.Message(response.Message);
                return -1;
            }
        }
        #endregion

        #region 获取文件信息
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="strFileName">文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns>文件大小</returns>
        public async Task<string> GetFileInfoAsync(string strFileName, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            Socket socketData = await this.CreateDataSocketAsync(token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.LIST, strFileName), token);
            if (!(response.StatusCode == FtpStatusCode.OpeningData || response.StatusCode == FtpStatusCode.DataAlreadyOpen
                || response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
            {
                this.Message(response.Message);
                return "";
            }
            byte[] b = new byte[512];
            MemoryStream ms = new MemoryStream();

            while (true)
            {
                int iBytes = socketData.Receive(b, b.Length, 0);
                ms.Write(b, 0, iBytes);
                if (iBytes <= 0) break;
            }
            byte[] bt = ms.GetBuffer();
            var strResult = bt.GetString();
            ms.Close();
            return strResult;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="strFileName">待删除文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<Boolean> DeleteAsync(string strFileName, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.DELE, strFileName), token);
            if (response.StatusCode != FtpStatusCode.FileActionOK)
            {
                this.Message(response.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region 重命名(如果新文件名与已有文件重名,将覆盖已有文件)
        /// <summary>
        /// 重命名(如果新文件名与已有文件重名,将覆盖已有文件)
        /// </summary>
        /// <param name="strOldFileName">旧文件名</param>
        /// <param name="strNewFileName">新文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<Boolean> RenameAsync(string strOldFileName, string strNewFileName, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.RNFR, strOldFileName), token);
            if (response.StatusCode != FtpStatusCode.FileCommandPending)
            {
                this.Message(response.Message);
                return false;
            }
            //  如果新文件名与原有文件重名,将覆盖原有文件
            response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.RNTO, strNewFileName), token);
            if (response.StatusCode != FtpStatusCode.FileActionOK)
            {
                this.Message(response.Message);
                return false;
            }
            return true;
        }
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
        public async Task DownFilesAsync(string strFileNameMask, string strFolder, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            var strFiles = await this.GetDirAsync(strFileNameMask, token);
            foreach (string strFile in strFiles)
            {
                if (!strFile.Equals(""))//一般来说strFiles的最后一个元素可能是空字符串
                {
                    await this.DownFileAsync(strFile, strFolder, strFile, token);
                }
            }
        }
        #endregion

        #region 下载一个文件
        /// <summary>
        /// 下载一个文件
        /// </summary>
        /// <param name="remoteFileName">要下载的文件名</param>
        /// <param name="localFolder">本地目录(绝对地址)</param>
        /// <param name="localFileName">保存在本地时的文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<Boolean> DownFileAsync(string remoteFileName, string localFolder, string localFileName, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            if (localFileName.IsNullOrEmpty())
                localFileName = remoteFileName;
            Socket socketData = await this.CreateDataSocketAsync(token);
            try
            {
                await this.SetTransferTypeAsync(TransferType.Binary, token);
                if (localFileName.Equals(""))
                    localFileName = remoteFileName;
                var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.RETR, remoteFileName), token);
                if (!(response.StatusCode == FtpStatusCode.OpeningData || response.StatusCode == FtpStatusCode.DataAlreadyOpen || response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
                {
                    this.Message(response.Message);
                    return false;
                }
                FileStream output = new FileStream(localFolder.Trim(new char[] { '/', '\\' }) + FileHelper.AltDirectorySeparatorChar + localFileName, FileMode.Create);
                while (true)
                {
                    int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                    output.Write(buffer, 0, iBytes);
                    if (iBytes <= 0) { Array.Clear(buffer, 0, buffer.Length); break; }
                }
                output.Close();
                if (socketData.Connected)
                    socketData.Close();
                if (!(response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
                {
                    response = await GetResponseAsync(token);
                    if (!(response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
                    {
                        this.Message(response.Message);
                        return false;
                    }
                }
            }
            catch
            {
                socketData.Close();
                this.SocketControl.Close();
                this.Connected = false;
                this.SocketControl = null;
                return false;
            }
            return true;
        }
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
        public async Task<Boolean> DownFileNoBinaryAsync(string strRemoteFileName, string strFolder, string strLocalFileName, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            if (strLocalFileName.IsNullOrEmpty())
                strLocalFileName = strRemoteFileName;
            Socket socketData = await this.CreateDataSocketAsync(token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.RETR, strRemoteFileName), token);
            if (!(response.StatusCode == FtpStatusCode.OpeningData || response.StatusCode == FtpStatusCode.DataAlreadyOpen || response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
            {
                this.Message(response.Message);
                return false;
            }
            FileStream output = new FileStream(strFolder + "\\" + strLocalFileName, FileMode.Create);
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                output.Write(buffer, 0, iBytes);
                if (iBytes <= 0) break;
            }
            output.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
            {
                response = await GetResponseAsync(token);
                if (!(response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
                {
                    this.Message(response.Message);
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 上传文件夹内所有文件到指定目录
        /// <summary>
        /// 上传文件夹内所有文件到指定目录
        /// </summary>
        /// <param name="localFolder">本地文件夹</param>
        /// <param name="remoteFolder">服务器文件夹</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task UploadFolderAsync(string localFolder, string remoteFolder, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            var dir = new DirectoryInfo(localFolder);
            foreach (var strFile in dir.GetFiles())
            {
                await this.UploadFileAsync(strFile.FullName, remoteFolder, token);
            }
            foreach (var d in dir.GetDirectories())
            {
                await this.MakeDirectoryAsync(remoteFolder + "/" + d.Name, token);
                await UploadFolderAsync(d.FullName, remoteFolder + "/" + d.Name, token);
            }
        }
        /// <summary>
        /// 上传文件到指定目录
        /// </summary>
        /// <param name="localFile">本地文件</param>
        /// <param name="remoteFolder">服务器文件夹</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<Boolean> UploadFileAsync(string localFile, string remoteFolder, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            await this.ChangeDirectoryAsync(remoteFolder, token);
            return await this.PutAsync(localFile, token);
        }
        #endregion

        #region 上传一批文件
        /// <summary>
        /// 上传一批文件
        /// </summary>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="strFileNameMask">文件名匹配字符(可以包含*和?)</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task PutAsync(string strFolder, string strFileNameMask, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            string[] strFiles = Directory.GetFiles(strFolder, strFileNameMask);
            foreach (var strFile in strFiles)
            {
                await this.PutAsync(strFile, token);
            }
        }
        #endregion

        #region 上传一个文件
        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="FileName">本地文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<Boolean> PutAsync(string FileName, CancellationToken token = default(CancellationToken))
        {
            //this.Message("1.生成socket");
            token.IfEmptyValue(this.Token);
            Socket socketData = await this.CreateDataSocketAsync(token);

            //this.Message("2.提交命令 发送命令并获取应答码和应答字符串");
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.STOR, FileName.GetExtension().IsNullOrEmpty() ? FileName.GetFileNameWithoutExtension() : FileName.GetFileName()), token);
            if (!(response.StatusCode == FtpStatusCode.DataAlreadyOpen || response.StatusCode == FtpStatusCode.OpeningData))
            {
                string msg = response.Message;
                if (msg.Contains("Permission denied"))
                {
                    msg += "(请检查是否没有权限或文件已上传)";
                }
                this.Message(msg);
                return false;
            }

            //this.Message("3.上传数据");
            FileStream input = new FileStream(FileName, FileMode.Open);
            int iBytes;
            long allbyte = 0;
            while ((iBytes = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                socketData.Send(buffer, iBytes, 0);
                allbyte += iBytes;
                this.SendByte(allbyte.ToString());
            }
            input.Close();

            //this.Message("4.关闭socket");
            if (socketData.Connected)
                socketData.Close();

            //this.Message("5.错误处理");
            if (!(response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
            {
                response = await GetResponseAsync(token);
                if (!(response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
                {
                    this.Message(response.Message);
                    return false;
                }
            }

            //this.Message("6.ftp结束");
            return true;
        }
        #endregion

        #region 上传一个文件
        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="FileName">本地文件名</param>
        /// <param name="strGuid">服务器文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<Boolean> PutByGuid(string FileName, string strGuid, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            string str = FileName.Substring(0, FileName.LastIndexOf("\\"));
            strGuid = str + "\\" + strGuid;
            File.Copy(FileName, strGuid);
            File.SetAttributes(strGuid, FileAttributes.Normal);
            Socket socketData = await this.CreateDataSocketAsync(token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.STOR, strGuid.GetFileName()), token);
            if (!(response.StatusCode == FtpStatusCode.DataAlreadyOpen || response.StatusCode == FtpStatusCode.OpeningData))
            {
                this.Message(response.Message);
                return false;
            }
            FileStream input = new FileStream(strGuid, FileMode.Open, FileAccess.Read, FileShare.Read);
            int iBytes = 0;
            while ((iBytes = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                socketData.Send(buffer, iBytes, 0);
            }
            input.Close();
            File.Delete(strGuid);
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
            {
                response = await GetResponseAsync(token);
                if (!(response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
                {
                    this.Message(response.Message);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="FileName">本地文件名</param>
        /// <param name="strGuid">服务器文件名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<Boolean> NewPutByGuid(string FileName, string strGuid, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            string str = FileName.Substring(0, FileName.LastIndexOf("\\"));
            strGuid = str + "\\" + strGuid;
            Socket socketData = await this.CreateDataSocketAsync(token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.STOR, Path.GetFileName(strGuid)), token);
            if (!(response.StatusCode == FtpStatusCode.DataAlreadyOpen || response.StatusCode == FtpStatusCode.OpeningData))
            {
                this.Message(response.Message);
                return false;
            }
            FileStream input = new FileStream(strGuid, FileMode.Open);
            input.Flush();
            int iBytes = 0;
            while ((iBytes = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                socketData.Send(buffer, iBytes, 0);
            }
            input.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
            {
                response = await GetResponseAsync(token);
                if (!(response.StatusCode == FtpStatusCode.ClosingData || response.StatusCode == FtpStatusCode.FileActionOK))
                {
                    this.Message(response.Message);
                    return false;
                }
            }
            return true;
        }
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
        public async Task<Boolean> MakeDirectoryAsync(string DirName, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.MKD, DirName), token);
            if (response.StatusCode != FtpStatusCode.PathnameCreated)
            {
                this.Message(response.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region 删除目录
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="DirName">目录名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<Boolean> RemoveDirectory(string DirName, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.RMD, DirName), token);
            if (response.StatusCode != FtpStatusCode.FileActionOK)
            {
                this.Message(response.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region 改变目录
        /// <summary>
        /// 改变目录
        /// </summary>
        /// <param name="NewDirName">新的工作目录名</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<Boolean> ChangeDirectoryAsync(string NewDirName, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            if (NewDirName.Equals(".") || NewDirName.IsNullOrEmpty()) return false;
            if (this.RemoteDirectory == NewDirName) return false;
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.CWD, NewDirName), token);
            if (response.StatusCode != FtpStatusCode.FileActionOK)
            {
                this.Message(response.Message);
                return false;
            }
            this.RemoteDirectory = NewDirName;
            return true;
        }
        #endregion

        #endregion

        #region 内部函数

        #region 将一行应答字符串记录在Reply和Msg,应答码记录在iReplyCode
        /// <summary>
        /// 获取应答信息
        /// </summary>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<FtpResponse> GetResponseAsync(CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            var line = await this.ReadLineAsync(token);
            return new FtpResponse(line);
        }
        #endregion

        #region 建立进行数据连接的socket
        /// <summary>
        /// 建立进行数据连接的socket
        /// </summary>
        /// <param name="token">取消标识</param>
        /// <returns>数据连接socket</returns>
        public async Task<Socket> CreateDataSocketAsync(CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            var response = await this.SendCommandAsync(new FtpCommandEnvelope(FtpCommand.PASV), token);
            if (response.StatusCode != FtpStatusCode.EnteringPassive)
            {
                this.Message(response.Message); return null;
            }
            var msg = response.Message;
            int index1 = msg.IndexOf('(');
            int index2 = msg.IndexOf(')');
            string ipData = msg.Substring(index1 + 1, index2 - index1 - 1);
            int[] parts = new int[6];
            int len = ipData.Length;
            int partCount = 0;
            string buf = "";
            for (int i = 0; i < len && partCount <= 6; i++)
            {
                char ch = Char.Parse(ipData.Substring(i, 1));
                if (Char.IsDigit(ch))
                    buf += ch;
                else if (ch != ',')
                {
                    throw new IOException("Malformed PASV strReply: " + msg);
                }
                if (ch == ',' || i + 1 == len)
                {
                    try
                    {
                        parts[partCount++] = Int32.Parse(buf);
                        buf = "";
                    }
                    catch (Exception)
                    {
                        throw new IOException("Malformed PASV strReply: " + msg);
                    }
                }
            }
            string ipAddress = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];
            int port = (parts[4] << 8) + parts[5];
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = this.Config.ReceiveTimeout,
                SendTimeout = this.Config.SendTimeout
            };
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            try
            {
                s.Connect(ep);
            }
            catch (Exception ex)
            {
                this.Message("无法连接ftp服务器." + ex.Message);
                s = null;
            }
            return s;
        }
        #endregion

        #region 关闭socket连接(用于登录以前)
        /// <summary>
        /// 关闭socket连接(用于登录以前)
        /// </summary>
        public void CloseSocketConnect()
        {
            if (this.SocketControl != null)
            {
                this.SocketControl.Close();
                this.SocketControl = null;
            }
        }
        #endregion

        #region 读取Socket返回的所有字符串
        /// <summary>
        /// 读取Socket返回的所有字符串
        /// </summary>
        /// <param name="token">取消标识</param>
        /// <returns>包含应答码的字符串行</returns>
        private async Task<string> ReadLineAsync(CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            if (token.IsCancellationRequested) return string.Empty;
            await this.ReceiveSlim.WaitAsync(token);
            var msg = "";
            try
            {
                while (true)
                {
                    int iBytes = this.SocketControl.Receive(buffer, buffer.Length, 0);
                    msg += buffer.GetString("", 0, iBytes);
                    if (iBytes < buffer.Length) break;
                }
                char[] seperator = { '\n' };
                string[] mess = msg.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                if (mess.Length > 2)
                    msg = mess[mess.Length - 2];
                else
                    msg = mess[0];
                if (!msg.IsMatch(@"\d{3}[-\s]")) //返回字符串正确的是以应答码(如220开头,后面接一空格,再接问候字符串)
                    return await this.ReadLineAsync(token);
            }
            catch (SocketException ex)
            {
                LogHelper.Error(ex);
            }
            finally
            {
                ReceiveSlim.Release();
            }
            return msg;
        }
        #endregion

        #region 发送命令并获取应答码和最后一行应答字符串
        /// <summary>
        /// 发送命令并获取应答码和应答字符串
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        public async Task<FtpResponse> SendCommandAsync(FtpCommandEnvelope command, CancellationToken token = default(CancellationToken))
        {
            token.IfEmptyValue(this.Token);
            if (command.FtpCommand != FtpCommand.USER && command.FtpCommand != FtpCommand.PASS && !this.Connected) await this.ConnectAsync(token);
            if (this.SocketControl == null)
            {
                return new FtpResponse
                {
                    StatusCode = FtpStatusCode.Undefined,
                    Message = "服务器连接不成功."
                };
            }
            await this.SendSlim.WaitAsync(token);
            var bytes = command.ToBytes();
            this.SocketControl.Send(bytes, bytes.Length, 0);
            //await Task.Delay(500);
            var response = await this.GetResponseAsync(token);
            this.SendSlim.Release();
            return response;
        }
        #endregion

        #region 处理消息
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="msg">消息</param>
        public void Message(string msg) => this.OnMessage?.Invoke(msg, EventArgs.Empty);
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="format">格式</param>
        /// <param name="param">参数组</param>
        public void Message(string format, params object[] param) => this.Message(format.format(param));

        /// <summary>
        /// 返回已上传的数据
        /// </summary>
        /// <param name="sendbyte">发送字节</param>
        public void SendByte(string sendbyte) => this.OnSendByte?.Invoke(sendbyte, EventArgs.Empty);


        #endregion

        #endregion
    }
}