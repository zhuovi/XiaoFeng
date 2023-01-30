using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using XiaoFeng.IO;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-07-11 16:55:30                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Resource
{
    /// <summary>
    /// 内嵌资源
    /// </summary>
    public class ResourceFileInfo : IResourceFileInfo
    {
        #region 构造器
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="path">路径</param>
        public ResourceFileInfo(Assembly assembly, string path)
        {
            if (assembly == null || path.IsNullOrEmpty()) return;

            this.Assembly = assembly;

            var assemblyName = assembly.GetName().Name;
            this.Extension = path.GetExtension();
            if (path.StartsWith(assemblyName, StringComparison.OrdinalIgnoreCase))
                path = path.RemovePattern($@"^{assemblyName}(\/|\\|\.)");
            path = path.RemovePattern($@"{this.Extension}$");
            if (!path.IsMatch(@"(\/|\\)"))
            {
                path = path.ReplacePattern(@"\.+", "/");
            }
            else
                path = path.ReplacePattern(@"(\\|\/)+", "/");
            this.ResourcePath = "/" + path.Trim(new char[] { '/', '\\' }) + this.Extension;
            // - 转换成_ @转换成_ .数字转换成._数字
            var pIndex = this.ResourcePath.LastIndexOfAny(new char[] { '\\', '/' });
            var subPath = this.ResourcePath.Substring(0, pIndex);
            if (subPath.IsMatch(@"(-|@|\.\d+)"))
            {
                this.ResourcePath = subPath.ReplacePattern(@"[-@]", "_").ReplacePattern(@"(\.)(\d+)", "$1_$2") + this.ResourcePath.Substring(pIndex);
            }
            this.Key = assemblyName + "." + path.TrimStart('/').ReplacePattern(@"(\\|\/)+", ".") + this.Extension;
            this.Name = path.GetFileName() + this.Extension;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 程序集
        /// </summary>
        public Assembly Assembly { get; }
        /// <summary>
        /// 是否存在
        /// </summary>
        private bool? _Exists;
        /// <summary>
        /// 是否存在
        /// </summary>
        public bool Exists
        {
            get
            {
                if (!_Exists.HasValue)
                {
                    CreateReadStream();
                }
                return _Exists.Value;
            }
        }
        /// <summary>
        /// 文件长度
        /// </summary>
        private long? _Length;
        /// <summary>
        /// 文件长度
        /// </summary>
        public long Length
        {
            get
            {
                if (!_Length.HasValue)
                {
                    CreateReadStream();
                }
                return _Length.Value;
            }
        }
        /// <summary>
        /// 路径
        /// </summary>
        public string PhysicalPath => String.Empty;
        /// <summary>
        /// 资源路径
        /// </summary>
        public string ResourcePath { get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 后缀
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Key { get; }
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
        public bool IsDirectory => false;
        #endregion

        #region 方法
        /// <summary>
        /// 获取资源流
        /// </summary>
        /// <returns></returns>
        public Stream CreateReadStream()
        {
            var name = Assembly.GetManifestResourceNames().Find(a => a.Equals(Key, StringComparison.OrdinalIgnoreCase));
            if (name.IsNullOrEmpty())
            {
                _Length = 0;
                _Exists = false;
                return null;
            }
            var stream = Assembly.GetManifestResourceStream(name);
            if (stream == null)
            {
                _Length = 0;
                _Exists = false;
                return null;
            }
            if (!_Length.HasValue)
            {
                _Length = stream.Length;
            }
            if (!_Exists.HasValue)
                _Exists = true;
            return stream;
        }
        /// <summary>
        /// 读取文件字节
        /// </summary>
        /// <returns></returns>
        public byte[] CreateReadBytes()
        {
            using(var stream = CreateReadStream())
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }
        #endregion
    }
}