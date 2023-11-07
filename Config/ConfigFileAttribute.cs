using System;
using System.ComponentModel;
using System.Linq;
using XiaoFeng.IO;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-08 10:43:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Config
{
    /// <summary>
    /// 配置文件属性
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ConfigFileAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConfigFileAttribute()
        {
            this.CacheKey = Guid.NewGuid().ToString("N");
        }
        /// <summary>
        /// 设置配置文件名
        /// </summary>
        /// <param name="fileName">配置文件名</param>
        public ConfigFileAttribute(string fileName) : this()
        {
            this.FileName = fileName;
            var ext = this.FileName.GetMatch(@"\.(json|xml|ini)$");
            if (ext.IsNotNullOrEmpty() && Enum.GetNames(typeof(ConfigFormat)).Where(a => a.ToLower() == ext.ToLower()).Count() > 0) this.Format = ext.ToEnum<ConfigFormat>();
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="fileName">配置文件名</param>
        /// <param name="timeOut">超时时间 0为永久 单位为秒</param>
        public ConfigFileAttribute(string fileName, int timeOut) : this(fileName)
        {
            this.TimeOut = timeOut;
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="fileName">配置文件名</param>
        /// <param name="timeOut">超时时间 0为永久 单位为秒</param>
        /// <param name="cacheKey">缓存Key</param>
        public ConfigFileAttribute(string fileName, int timeOut, string cacheKey) : this(fileName, timeOut)
        {
            this.CacheKey = cacheKey;
        }
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="timeOut">超时时间 0为永久 单位为秒</param>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="format">配置格式 默认为Json</param>
        public ConfigFileAttribute(string fileName, int timeOut, string cacheKey, ConfigFormat format) : this(fileName, timeOut, cacheKey)
        {
            this.Format = format;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 配置文件名
        /// </summary>
        private string _fileName = "";
        /// <summary>
        /// 配置文件名
        /// </summary>
        [Description("配置文件名")]
        public string FileName
        {
            get
            {
                if (this._fileName.IsNullOrEmpty()) return "";
                return this._fileName.TrimStart('/').GetBasePath();
            }
            set { this._fileName = value; }
        }
        /// <summary>
        /// 超时时间
        /// </summary>
        [Description("超时时间")]
        public int TimeOut { get; set; } = 1500;
        /// <summary>
        /// 缓存Key
        /// </summary>
        public string CacheKey { get; set; }
        /// <summary>
        /// 配置格式
        /// </summary>
        [Description("配置格式")]
        public ConfigFormat Format { get; set; } = ConfigFormat.Json;
        #endregion
    }
}