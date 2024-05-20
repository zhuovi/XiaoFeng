﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using XiaoFeng.Data.SQL;
using XiaoFeng.IO;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-11 16:13:17                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Zip
{
    /// <summary>
    /// 压缩操作类
    /// Verstion : 1.0.0
    /// Create Time : 2017/10/11 16:13:17
    /// Update Time : 2017/10/11 16:13:17
    /// </summary>
    public class ZipHelper
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ZipHelper() { }
        #endregion

        #region 属性
        /// <summary>
        /// 静态方法
        /// </summary>
        public static ZipHelper Instance { get { return new ZipHelper(); } }
        #endregion

        #region 方法

        #region 解压文件
        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="ZipPath">压缩包路径</param>
        /// <param name="rootPath">解压的目录</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public Boolean UNZip(string ZipPath, string rootPath, Encoding encoding = null)
        {
            if (ZipPath.IsNullOrEmpty() || rootPath.IsNullOrEmpty() || !File.Exists(ZipPath)) return false;
            //if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
            rootPath = rootPath.GetBasePath();
            if (!FileHelper.Exists(rootPath, FileAttribute.Directory)) FileHelper.CreateDirectory(rootPath);
            if (encoding == null)
                ZipFile.ExtractToDirectory(ZipPath, rootPath);
            else
                ZipFile.ExtractToDirectory(ZipPath, rootPath, encoding);
            return true;
        }
        #endregion

        #region 获取压缩包内文件
        /// <summary>
        /// 获取压缩包内文件
        /// </summary>
        /// <param name="zipPath">压缩包路径</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public List<string> GetList(string zipPath, Encoding encoding)
        {
            List<string> list = new List<string>();

            if (zipPath.IsNullOrEmpty()) return list;
            zipPath = zipPath.GetBasePath();
            if (!File.Exists(zipPath)) return list;
            using (FileStream zipFileToOpen = new FileStream(zipPath, FileMode.Open))
            {
                using (ZipArchive archive = encoding == null ? new ZipArchive(zipFileToOpen, ZipArchiveMode.Read) : new ZipArchive(zipFileToOpen, ZipArchiveMode.Read, false, encoding))
                    foreach (var zipArchiveEntry in archive.Entries)
                        list.Add(zipArchiveEntry.FullName);
            }
            return list;
        }
        #endregion

        #region 创建压缩包
        /// <summary>
        /// 创建压缩包
        /// </summary>
        /// <param name="ZipPath">压缩包路径</param>
        /// <param name="rootPath">目录地址</param>
        /// <returns></returns>
        public Boolean Create(string ZipPath, string rootPath)
        {
            if (File.Exists(ZipPath)) File.Delete(ZipPath);
            if (ZipPath.IsNullOrEmpty() || rootPath.IsNullOrEmpty() || !Directory.Exists(rootPath)) return false;
            ZipFile.CreateFromDirectory(rootPath, ZipPath);
            return File.Exists(ZipPath);
        }
        /// <summary>
        /// 创建压缩包
        /// </summary>
        /// <param name="ZipPath">压缩包路径</param>
        /// <param name="rootPath">目录地址</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <returns></returns>
        public Boolean Create(string ZipPath, string rootPath, CompressionLevel compressionLevel)
        {
            if (File.Exists(ZipPath)) File.Delete(ZipPath);
            if (ZipPath.IsNullOrEmpty() || rootPath.IsNullOrEmpty() || !Directory.Exists(rootPath)) return false;
            ZipFile.CreateFromDirectory(rootPath, ZipPath, compressionLevel, false);
            return File.Exists(ZipPath);
        }
        /// <summary>
        /// 创建压缩包
        /// </summary>
        /// <param name="ZipPath">压缩包路径</param>
        /// <param name="rootPath">目录地址</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public Boolean Create(string ZipPath, string rootPath, CompressionLevel compressionLevel, Encoding encoding = null)
        {
            if (File.Exists(ZipPath)) File.Delete(ZipPath);
            if (ZipPath.IsNullOrEmpty() || rootPath.IsNullOrEmpty() || !Directory.Exists(rootPath)) return false;
            if (encoding == null)
                ZipFile.CreateFromDirectory(rootPath, ZipPath, compressionLevel, false);
            else
                ZipFile.CreateFromDirectory(rootPath, ZipPath, compressionLevel, false, encoding);
            return File.Exists(ZipPath);
        }
        /// <summary>
        /// 创建压缩包
        /// </summary>
        /// <param name="zipPath">压缩包路径</param>
        /// <param name="filePath">文件地址</param>
        /// <returns></returns>
        public Boolean Create(string zipPath, string[] filePath = null)
        {
            if (zipPath.IsNullOrEmpty()) return false;
            if (File.Exists(zipPath)) File.Delete(zipPath);
            using (ZipArchive zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                if (filePath != null && filePath.Length > 0)
                {
                    foreach (string path in filePath)
                    {
                        string filename = System.IO.Path.GetFileName(path);

                        zipArchive.CreateEntryFromFile(path, filename);
                    }
                }
            }
            return File.Exists(zipPath);
            /* using (FileStream zipFileToOpen = new FileStream(zipPath, FileMode.Create))
             {
                 using (ZipArchive archive = new ZipArchive(zipFileToOpen, ZipArchiveMode.Create))
                 {
                     if (filePath != null && filePath.Length > 0)
                     {
                         foreach (string path in filePath)
                         {
                             string filename = System.IO.Path.GetFileName(path);
                             ZipArchiveEntry readMeEntry = archive.CreateEntry(filename);
                             using (System.IO.Stream stream = readMeEntry.Open())
                             {
                                 byte[] bytes = System.IO.File.ReadAllBytes(path);
                                 stream.Write(bytes, 0, bytes.Length);
                             }
                         }
                     }
                 }
             }
             return File.Exists(zipPath);*/
        }
        #endregion

        #endregion

        #region 析构器
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposeValue">释放标识</param>
        protected virtual void Dispose(Boolean disposeValue)
        {
           
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~ZipHelper() { Dispose(false); }
        #endregion
    }
}