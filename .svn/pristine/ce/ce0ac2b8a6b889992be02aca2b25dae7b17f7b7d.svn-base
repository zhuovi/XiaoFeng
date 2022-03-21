using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using XiaoFeng.IO;
using XiaoFeng.Json;
using XiaoFeng.Cache;
using System.Linq;

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

        #region 获取配置
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public virtual TConfig GetEntity(Func<TConfig, Boolean> func)
        {
            if (func == null) func = a => true;
            return this.GetEntities(func)?.FirstOrDefault();
        }
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public virtual IEnumerable<TConfig> GetEntities(Func<TConfig, Boolean> func)
        {
            if (func == null) func = a => true;
            return new List<TConfig>() { this as TConfig }.Where(func);
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
            if (File.Exists(attr.FileName))
            {
                return this.OpenFile(attr.FileName);
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
            if (!reload)
            {
                var val = cache.Get(attr.CacheKey);
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
                if (this.ConfigFileAttribute == null) this.ConfigFileAttribute = attr;
                if (Reload) cache.Set(attr.CacheKey, this as TConfig, attr.FileName);
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
            if (!Directory.Exists(Path.GetDirectoryName(attr.FileName))) Directory.CreateDirectory(Path.GetDirectoryName(attr.FileName));
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
                var f = FileHelper.WriteText(attr.FileName, val, Encoding.UTF8);
                /*
                 * 更新缓存,有延迟,故直接清除缓存
                 * Cache.CacheHelper.Set(attr.CacheKey, this as TConfig, attr.FileName);
                 */
                CacheFactory.Create(CacheType.Memory).Remove(attr.CacheKey);
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
            if (!FileHelper.Exists(path)) return string.Empty;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
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

        #endregion

    }
}