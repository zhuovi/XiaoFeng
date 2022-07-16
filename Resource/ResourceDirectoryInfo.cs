using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-07-11 17:42:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Resource
{
    /// <summary>
    /// 内嵌资源目录
    /// </summary>
    public class ResourceDirectoryInfo
    {
        #region 构造器
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="root">目录</param>
        public ResourceDirectoryInfo(Assembly assembly, string root)
        {
            if (assembly == null) return;
            this.Assembly = assembly;
            var assemblyName = assembly.GetName().Name;
            if (root.StartsWith(assemblyName, StringComparison.OrdinalIgnoreCase))
                root = root.RemovePattern($@"^{assemblyName}(\/|\\|\.)");
            if (!root.IsMatch(@"(\/|\\)"))
                root = root.ReplacePattern(@"\.+", "/");
            else
                root = root.ReplacePattern(@"(\\|\/)+", "/");
            this.ResourcePath = "/" + root.Trim(new char[] { '/', '\\' });

            this.Key = assemblyName + "." + root.TrimStart('/').ReplacePattern(@"(\\|\/)+", ".");
        }
        #endregion

        #region 属性
        /// <summary>
        /// 程序集
        /// </summary>
        public Assembly Assembly { get; }
        /// <summary>
        /// 文件个数
        /// </summary>
        private long? _Length;
        /// <summary>
        /// 文件个数
        /// </summary>
        public long Length
        {
            get
            {
                if (!_Length.HasValue)
                {
                    _Length = Assembly.GetManifestResourceNames().FindAll(a => a.StartsWith(Key + ".")).Length;
                }
                return _Length.Value;
            }
        }
        /// <summary>
        /// 目录
        /// </summary>
        public string ResourcePath { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 是否存在
        /// </summary>
        public bool Exists => true;
        /// <summary>
        /// 最后修改时间
        /// </summary>
        private DateTimeOffset _LastModified = DateTimeOffset.UtcNow;
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTimeOffset LastModified
        {
            get
            {
                if (Assembly.Location.IsNotNullOrEmpty())
                {
                    try
                    {
                        _LastModified = File.GetLastWriteTimeUtc(Assembly.Location);
                    }
                    catch (PathTooLongException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
                return _LastModified;
            }
        }
        /// <summary>
        /// 是否是目录
        /// </summary>
        public bool IsDirectory => true;
        #endregion

        #region 方法
        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <returns></returns>
        public List<ResourceFileInfo> GetFiles()
        {
            var list = new List<ResourceFileInfo>();
            var files = Assembly.GetManifestResourceNames().FindAll(a => a.StartsWith(Key + "."));
            if (!_Length.HasValue)
            {
                _Length = files.Length;
            }
            files.Each(f =>
            {
                list.Add(new ResourceFileInfo(Assembly, f));
            });
            return list;
        }
        #endregion
    }
}