using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if NETFRAMEWORK 
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif
using XiaoFeng.Config;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/2/4 10:13:47                 *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.Web
{
    /// <summary>
    /// 上传接口
    /// </summary>
    public interface IUploadFiles
    {
        /// <summary>
        /// 上传完一个文件事件
        /// </summary>
        UploadMessageEventHandler OnMessage { get; set; }
        /// <summary>
        /// 上传完所有文件事件
        /// </summary>
        UploadCompleteEventHandler OnComplete { get; set; }
        /// <summary>
        /// 上传配置
        /// </summary>
        Upload Config { get; set; }
        /// <summary>
        /// 失败列表
        /// </summary>
        IEnumerable<FileMessage> ErrorList { get; }
        /// <summary>
        /// 成功列表
        /// </summary>
        IEnumerable<FileMessage> SuccessList { get; }
        /// <summary>
        /// 列表
        /// </summary>
        ConcurrentBag<FileMessage> Result { get; set; }
        /// <summary>
        /// 是否验证文件类型
        /// </summary>
        bool? IsValidateMime { get; set; }
        /// <summary>
        /// 上传文件集合
        /// </summary>
#if NETFRAMEWORK
        HttpFileCollection HttpFiles { get; set; }
#else
        IFormFileCollection HttpFiles { get; set; }
#endif
        /// <summary>
        /// 取消标识
        /// </summary>
        CancellationTokenSource Token { get; set; }
        /// <summary>
        /// 文件格式
        /// </summary>
        string Format { get; set; }
        /// <summary>
        /// 允许上传文件类型 多个用,隔开
        /// </summary>
        string FileType { get; set; }
        /// <summary>
        /// 路径格式 日期格式 yyyy MM dd HH mm ss ffff
        /// </summary>
        string PathFormat { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        string FileName { get; set; }
        /// <summary>
        /// 验证文件MIME
        /// </summary>
        /// <param name="fType">当前文件类型</param>
        /// <param name="fMime">当前文件MIME</param>
        /// <returns></returns>
        bool IsMime(string fType, string fMime);
        /// <summary>
        /// 是否含有木马
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns></returns>
        bool IsMuMa(Stream stream);
        /// <summary>
        /// 是否有木马
        /// </summary>
        /// <param name="fileContent">文件内容</param>
        /// <returns></returns>
        bool IsMuMa(string fileContent);
        /// <summary>
        /// 异步上传
        /// </summary>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task UploadAsync(CancellationToken token = default);
        /// <summary>
        /// 上传base64
        /// </summary>
        /// <param name="base64String">base64字符串</param>
        /// <param name="token">取消标识</param>
        /// <returns></returns>
        Task UploadBase64Async(string base64String, CancellationToken token = default);
    }
}