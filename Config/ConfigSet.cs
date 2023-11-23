using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using XiaoFeng.Cache;
using XiaoFeng.IO;
using XiaoFeng.Json;

namespace XiaoFeng.Config
{
    /// <summary>
    /// 配置基类
    /// </summary>
    public class ConfigSet<TConfig> : EntityBase, IConfigSet<TConfig> where TConfig : ConfigSet<TConfig>, new()
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConfigSet()
        {
            var type = typeof(TConfig);
            var attr = type.GetCustomAttribute<ConfigFileAttribute>();
            if (attr != null)
                this.ConfigFileAttribute = attr;
            this.EncryptFile = type.IsDefined(typeof(EncryptFileAttribute), false);
        }
        /// <summary>
        /// 设置配置文件名
        /// </summary>
        /// <param name="fileName">配置文件名</param>
        public ConfigSet(string fileName) : this()
        {
            this.ConfigFileAttribute.FileName = fileName;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 配置文件属性
        /// </summary>
        [Description("配置文件属性")]
        [JsonIgnore]
        [XmlIgnore]
        public virtual ConfigFileAttribute ConfigFileAttribute { get; set; }
        /// <summary>
        /// 是否加密文件
        /// </summary>
        [Description("是否加密文件")]
        [JsonIgnore]
        [XmlIgnore]
        public virtual Boolean EncryptFile { get; set; } = false;
        /// <summary>
        /// 配置
        /// </summary>
        public static TConfig Current
        {
            get
            {
                return new TConfig().Get();
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 读取最新的配置文件
        /// </summary>
        /// <returns></returns>
        public static TConfig Get() => new TConfig().Get(true);
        /// <summary>
        /// 获取泛配置
        /// </summary>
        /// <param name="value">泛值</param>
        /// <returns></returns>
        public static TConfig Get(object value) => new TConfig().GetEntity(value);

        #region 获取配置
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public virtual TConfig GetEntity(Func<TConfig, Boolean> func)
        {
#if NETSTANDARD2_0
            if (func == null) func = 
#else
            func ??=
#endif
            a => true;
            return this.GetEntities(func)?.FirstOrDefault();
        }
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public virtual IEnumerable<TConfig> GetEntities(Func<TConfig, Boolean> func)
        {
#if NETSTANDARD2_0
            if (func == null) func = 
#else
            func ??=
#endif
            a => true;
            return new List<TConfig>() { this as TConfig }.Where(func);
        }
        /// <summary>
        /// 获取泛路径配置
        /// </summary>
        /// <param name="value">泛值</param>
        /// <returns></returns>
        public virtual TConfig GetEntity(object value)
        {
            var attr = this.ConfigFileAttribute;
            if (attr == null) return default;
            if (IsGenericPath(attr.FileName))
                this[GetGenericKey(attr.FileName)] = value;
            return this.Get(true);
        }
        #endregion

        #region 读取内容
        /// <summary>
        /// 读取内容
        /// </summary>
        /// <returns></returns>
        public virtual string ReadContent()
        {
            var attr = this.ConfigFileAttribute;
            if (attr == null) return string.Empty;
            if (IsGenericPath(attr.FileName) && this[GetGenericKey(attr.FileName)].IsNullOrEmpty()) return string.Empty;
            var configPath = this.GetConfigPath(attr.FileName);
            if (File.Exists(configPath))
            {
                return this.OpenFile(configPath);
            }
            else
            {
                var val = new TConfig();
                val.CopyTo(this as TConfig);
                val.Save();
                if (attr.Format == ConfigFormat.Json)
                {
                    return val.ToJson();
                }
                else if (attr.Format == ConfigFormat.Xml)
                {
                    return val.EntityToXml();
                }
                else if (attr.Format == ConfigFormat.Ini)
                {

                }
                return "";
            }
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="reload">是否强制从文件中读取 默认否</param>
        /// <returns></returns>
        public virtual TConfig Get(Boolean reload = false)
        {
            var attr = this.ConfigFileAttribute;
            if (attr == null) return null;
            var Reload = false;
            var cache = CacheFactory.Create(CacheType.Memory);
            var cacheKey = this.GetConfigPath(attr.CacheKey);
            var configPath = this.GetConfigPath(attr.FileName);
            if (!reload)
            {
                var val = cache.Get(cacheKey);
                if (val == null || val == default(TConfig))
                    Reload = true;
                else
                {
                    val.ToCast<TConfig>().CopyTo(this as TConfig);
                    return this as TConfig;
                }
            }
            if (reload || Reload)
            {
                if ((this as TConfig) == null) return null;
                var val = (this as TConfig).ReadContent();
                if (val.IsNullOrEmpty()) return new TConfig();
                if (attr.Format == ConfigFormat.Json)
                {
                    val.JsonToObject<TConfig>().CopyTo(this as TConfig);
                }
                else if (attr.Format == ConfigFormat.Xml)
                {
                    val.XmlToEntity<TConfig>().CopyTo(this as TConfig);
                }
                else if (attr.Format == ConfigFormat.Ini)
                {

                }
#if NETSTANDARD2_0
                if (this.ConfigFileAttribute == null) this.ConfigFileAttribute = 
#else
                this.ConfigFileAttribute ??=
#endif
                attr;
                if (Reload) cache.Set(cacheKey, this as TConfig, configPath);
            }
            return this as TConfig;
        }
        #endregion

        #region 保存数据
        /// <summary>
        /// 保存内容
        /// </summary>
        /// <param name="indented">是否格式化</param>
        /// <param name="comment">是否带注释说明</param>
        public virtual bool Save(Boolean indented = true, Boolean comment = true)
        {
            var attr = this.ConfigFileAttribute;
            if (attr == null) return false;
            if (IsGenericPath(attr.FileName) && this[GetGenericKey(attr.FileName)].IsNullOrEmpty()) return false;
            var configPath = this.GetConfigPath(attr.FileName);
            var dirPath = configPath.GetDirectoryName();
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            string val = "";
            if (attr.Format == ConfigFormat.Json)
            {
                val = (this as TConfig).ToJson(new Json.JsonSerializerSetting
                {
                    Indented = indented,
                    IsComment = comment
                });
            }
            else if (attr.Format == ConfigFormat.Xml)
            {
                val = (this as TConfig).EntityToXml();
            }
            else if (attr.Format == ConfigFormat.Ini)
            {

            }
            if (val.IsNotNullOrEmpty())
            {
                var f = FileHelper.WriteText(configPath, val, Encoding.UTF8);
                /*
                 * 更新缓存,有延迟,故直接清除缓存
                 * Cache.CacheHelper.Set(attr.CacheKey, this as TConfig, attr.FileName);
                 */
                CacheFactory.Create(CacheType.Memory).Remove(this.GetConfigPath(attr.CacheKey));
                return f;
            }
            return false;
        }
        #endregion

        #region 打开文件
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="path">文件</param>
        /// <returns></returns>
        public virtual string OpenFile(string path)
        {
            var flag = "Encrypt";
            var content = string.Empty;
            var configPath = this.GetConfigPath(path);
            if (!FileHelper.Exists(configPath)) return string.Empty;

            using (var fs = new FileStream(configPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                /*长度少于加密标识,说明一定不会加密,直接返回数据*/
                if (fs.Length <= flag.Length)
                {
                    var bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    return bytes.GetString();
                }
                /*如果没有加密文件属性,说明一定会不加密,直接返回数据*/
                if (!this.EncryptFile)
                {
                    var bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    return bytes.GetString();
                }
                var setting = Setting.Current;
                var first = new byte[flag.Length];
                fs.Read(first, 0, first.Length);
                var DES = new Cryptography.DESEncryption();
                if (first.GetString().EqualsIgnoreCase(flag))
                {
                    var bytes = new byte[fs.Length - flag.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    bytes = DES.Decrypt(bytes, setting.DataKey);
                    content = bytes.GetString();
                    if (!setting.DataEncrypt)
                    {
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.SetLength(0);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Flush();
                    }
                }
                else
                {
                    var bytes = new byte[fs.Length];
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Read(bytes, 0, bytes.Length);
                    content = bytes.GetString();
                    if (setting.DataEncrypt)
                    {
                        bytes = DES.Encrypt(bytes, setting.DataKey);
                        fs.Seek(0, SeekOrigin.Begin);
                        fs.SetLength(0);
                        var FirstByte = flag.GetBytes();
                        fs.Write(FirstByte, 0, FirstByte.Length);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Flush();
                    }
                }
            }
            return content;
        }
        #endregion

        #region 获取主键
        /// <summary>
        /// 获取主键
        /// </summary>
        /// <returns></returns>
        public string GetPrimaryKey()
        {
            var PrimaryKey = string.Empty;
            var fields = typeof(TConfig).GetPropertiesAndFields().Each(p =>
            {
                if (p.IsDefined(typeof(ColumnAttribute), false))
                {
                    var column = p.GetColumnAttribute();
                    if (column.PrimaryKey)
                    {
                        PrimaryKey = p.Name;
                        return false;
                    }
                }
                return true;
            });
            return PrimaryKey;
        }
        #endregion

        #region 删除文件
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        public virtual Boolean Delete()
        {
            var attr = this.ConfigFileAttribute;
            var cache = CacheFactory.Create(CacheType.Memory);
            cache.Remove(this.GetConfigPath(attr.CacheKey));
            if (attr == null) return false;
            var configPath = this.GetConfigPath(attr.FileName);
            if (File.Exists(configPath))
                return FileHelper.DeleteFile(configPath);
            return false;
        }
        #endregion

        #region 获取配置文件路径
        /// <summary>
        /// 是否是泛路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public Boolean IsGenericPath(string path) => path.IsMatch(@"\{[a-z0-9_-]+\}");
        /// <summary>
        /// 获取泛key
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public string GetGenericKey(string path) => IsGenericPath(path) ? path.GetMatch(@"(?<a>\{[a-z0-9_-]+\})").Trim(new char[] { '{', '}' }) : string.Empty;
        /// <summary>
        /// 获取配置文件路径
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns>配置文件路径</returns>
        public string GetConfigPath(string path)
        {
            return IsGenericPath(path) ?
                 path.ReplacePattern(@"(?<a>\{[a-z0-9_-]+\})", m => GetValue(this[m.Groups["a"].Value.Trim(new char[] { '{', '}' })])) : path;
        }
        #endregion

        #region 转换为路径格式
        /// <summary>
        /// 转换为路径格式
        /// </summary>
        /// <param name="o">数据</param>
        /// <returns></returns>
        public string GetValue(object o)
        {
            if (o == null) return "";
            Type t = o.GetType();
            if (t.IsEnum) return t.IsDefined(typeof(FlagsAttribute)) ? o.ToString() : ((int)o).ToString();
            if (t == typeof(string)) return o.ToString().RemovePattern(@"(\r|\n|\t|\s|:|\\|\/|\*|\?|\""|\>|\<|\|)+");
            if (t == typeof(Guid)) return new Guid(o.ToString()).ToString("N");
            if (t == typeof(DateTime)) return ((DateTime)o).ToString("yyyyMMddHHmmssfffffff");
            if (t == typeof(bool)) return o.ToString().ToLower();
            return o.ToString();
        }
        #endregion

        #endregion
    }
}