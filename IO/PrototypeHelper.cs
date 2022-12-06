using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
namespace XiaoFeng.IO
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static partial class PrototypeHelper
    {
        /// <summary>
        /// 转换成魔法文件操作
        /// </summary>
        /// <param name="_">文件路径</param>
        /// <returns></returns>
        public static FayFile ToFayFile(this string _) => new FayFile(_);
        /// <summary>
        /// 获取路径的绝对路径
        /// </summary>
        /// <param name="_">路径</param>
        /// <returns></returns>
        public static string GetBasePath(this string _) => FileHelper.GetBasePath(_);
        /// <summary>
        /// 复制目录到另一个目录
        /// </summary>
        /// <param name="_">源目录</param>
        /// <param name="destDirName">目标目录</param>
        public static void CopyTo(this DirectoryInfo _, string destDirName) => FileHelper.CopyDirectory(_, new DirectoryInfo(destDirName.GetBasePath()));
        /// <summary>
        /// 复制目录到另一个目录
        /// </summary>
        /// <param name="_">源目录</param>
        /// <param name="destDirName">目标目录</param>
        public static void CopyToLinux(this DirectoryInfo _, string destDirName) => FileHelper.CopyDirectoryLinux(_, new DirectoryInfo(destDirName.GetBasePath()));
        /// <summary>
        /// 移除目录
        /// </summary>
        /// <param name="_">目标目录</param>
        public static void Remove(this DirectoryInfo _) => _.Delete(true);
        /// <summary>
        /// 获取子目录
        /// </summary>
        /// <param name="_">目录</param>
        /// <param name="dirName">子目录名称</param>
        /// <returns></returns>
        public static DirectoryInfo GetSubDirectory(this DirectoryInfo _, string dirName) => _.GetDirectories(dirName, SearchOption.AllDirectories).FirstOrDefault();
        /// <summary>
        /// 获取子文件
        /// </summary>
        /// <param name="_">目录</param>
        /// <param name="fileName">子文件</param>
        /// <returns></returns>
        public static FileInfo GetSubFile(this DirectoryInfo _, string fileName) => _.GetFiles(fileName, SearchOption.AllDirectories).FirstOrDefault();
        /// <summary>
        /// 转换为文件对象
        /// </summary>
        /// <param name="_">文件地址</param>
        /// <returns></returns>
        public static FileInfo ToFileInfo(this String _) => _.IsNullOrEmpty() ? null : new FileInfo(_.GetBasePath());
        /// <summary>
        /// 转换为目录对象
        /// </summary>
        /// <param name="_">目录地址</param>
        /// <returns></returns>
        public static DirectoryInfo ToDirectoryInfo(this String _) => _.IsNullOrEmpty() ? null : new DirectoryInfo(_.GetBasePath());
        /// <summary>
        /// 返回指定路径字符串目录信息
        /// </summary>
        /// <param name="_">路径字符串</param>
        /// <returns>path 的目录信息；如果 path 表示根目录或为 null，则为 null。 如果 path 不包含目录信息，则返回 System.String.Empty。</returns>
        public static string GetDirectoryName(this String _) => Path.GetDirectoryName(_);
        /// <summary>
        /// 更改路径字符串的扩展名
        /// </summary>
        /// <param name="_">要修改的路径信息</param>
        /// <param name="extension">新的扩展名（有或没有前导句点）。 指定 null 以从 path 移除现有扩展名</param>
        /// <returns>已修改的路径信息</returns>
        public static string ChangeExtension(this String _, string extension) => Path.ChangeExtension(_, extension);
        /// <summary>
        /// 返回指定的路径字符串的扩展名
        /// </summary>
        /// <param name="_">从中获取扩展名的路径字符串</param>
        /// <returns>指定路径的扩展名（包含句点“.”）、或 null、或 System.String.Empty。 如果 path 为 null，则 System.IO.Path.GetExtension(System.String) 返回 null。 如果 path 不具有扩展名信息，则 System.IO.Path.GetExtension(System.String) 返回 System.String.Empty。</returns>
        public static string GetExtension(this String _) => Path.GetExtension(_);
        /// <summary>
        /// 返回指定路径字符串的文件名和扩展名
        /// </summary>
        /// <param name="_">从中获取文件名和扩展名的路径字符串</param>
        /// <returns>中最后一个目录字符后的字符。 如果 path 的最后一个字符是目录或卷分隔符，则此方法返回 System.String.Empty。 如果 path为 null，则此方法返回 null</returns>
        public static string GetFileName(this String _) => Path.GetFileName(_);
        /// <summary>
        /// 返回不具有扩展名的指定路径字符串的文件名
        /// </summary>
        /// <param name="_">文件的路径</param>
        /// <returns>但不包括最后的句点 (.) 以及之后的所有字符</returns>
        public static string GetFileNameWithoutExtension(this String _) => Path.GetFileNameWithoutExtension(_);
        /// <summary>
        /// 确定路径是否包括文件扩展名
        /// </summary>
        /// <param name="_">用于搜索扩展名的路径</param>
        /// <returns>如果路径中最后一个目录分隔符（\\ 或 /）或卷分隔符 (:) 之后的字符包括句点 (.)，并且后面跟有一个或多个字符，则为 true；否则为 false</returns>
        public static Boolean HasExtension(this String _) => Path.HasExtension(_);
        /// <summary>
        /// 读取流文件内容
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string ReadToEnd(this Stream stream, Encoding encoding = null)
        {
            var content = string.Empty;
            if (stream == null || !stream.CanRead) return content;
            stream.Position = 0;
            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                content = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }
            return content;
        }
        /// <summary>
        /// 异步读取流文件内容
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static async Task<string> ReadToEndAsync(this Stream stream, Encoding encoding = null)
        {
            var content = string.Empty;
            if (stream == null || !stream.CanRead) return content;
            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                content = await reader.ReadToEndAsync();
                reader.Close();
                reader.Dispose();
            }
            return content;
        }
        /// <summary>
        /// 使用从缓冲区读取的数据将字节块写入当前流
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="buffer">字节组</param>
        public static void Write(this Stream stream, byte[] buffer) => stream.Write(buffer, 0, buffer.Length);
        /// <summary>
        /// 将字符串数组合并成一个路径
        /// </summary>
        /// <param name="_">开始字符串</param>
        /// <param name="param">路径串</param>
        /// <returns>路径</returns>
        public static string Combine(this String _, params String[] param) => Path.Combine(new string[] { _ }.Concat(param).ToArray());
        /// <summary>
        /// 写字符串
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="msg">字符串</param>
        public static void Write(this FileStream fileStream, string msg)
        {
            if (msg.IsNullOrEmpty()) return;
            fileStream.Write(msg.GetBytes());
        }
        /// <summary>
        /// 写字符串最后加回车
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="msg">字符串</param>
        public static void WriteLine(this FileStream fileStream, string msg)
        {
            fileStream.Write((msg + Environment.NewLine).GetBytes());
        }
        /// <summary>
        /// 写字节
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <param name="bytes">字节</param>
        public static void Write(this FileStream fileStream, byte[] bytes)
        {
            if (fileStream != null)
            {
                if (bytes.IsNullOrEmpty()) return;
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }
        /// <summary>
        /// 将字符串数组组合成一个路径。
        /// </summary>
        /// <param name="paths">由路径的各部分构成的数组。</param>
        /// <returns>已组合的路径。</returns>
        public static string Combine(this IEnumerable<string> paths) => FileHelper.Combine(paths);
    }
}