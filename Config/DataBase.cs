using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XiaoFeng.Data;
using XiaoFeng.IO;
using XiaoFeng.Cache;

namespace XiaoFeng.Config
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    [ConfigFile("/Config/DataBase.json", 0, "FAYELF-CONFIG-DATABASES", ConfigFormat.Json)]
    [EncryptFile]
    public class DataBase : ConfigSet<DataBase>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public DataBase() : base() { }
        /// <summary>
        /// 设置配置文件名
        /// </summary>
        /// <param name="fileName">配置文件名</param>
        public DataBase(string fileName) : base(fileName) { }
        #endregion

        #region 属性
        /// <summary>
        /// 数据
        /// </summary>
        public Dictionary<string, ConnectionConfig[]> Data { get; set; }
        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public new ConnectionConfig[] this[string key] => this.Get(key);
        #endregion

        #region 方法

        #region 是否存在
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public Boolean Exists(string key)
        {
            if (key.IsNullOrEmpty() || this.Data == null) return false;
            return this.Data.Where(a => key.IsMatch(@"^(" + a.Key.ReplacePattern(",", "|") + ")$")).Count() > 0;
        }
        #endregion

        #region 获取数据库配置
        /// <summary>
        /// 获取数据库配置
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public ConnectionConfig[] Get(string key = "")
        {
            if (this.Data.IsNullOrEmpty())
                return null;
            else
               if (key.IsNullOrEmpty()) return this.Data?.FirstOrDefault().Value;
            return this.Data.Where(a => key.IsMatch(@"^(" + a.Key.ReplacePattern(",", "|") + ")$"))?.FirstOrDefault().Value;
        }
        /// <summary>
        /// 第一个数据库配置
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public ConnectionConfig First(string key = "")
        {
            return this.Get(key).FirstOrDefault();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="reload">是否强制从文件中读取 默认否</param>
        /// <returns></returns>
        public override DataBase Get(bool reload = false)
        {
            var attr = this.ConfigFileAttribute;
            var cache = CacheFactory.Create(CacheType.Memory);
            var Reload = false;
            if (!reload)
            {
                var val = cache.Get(attr.CacheKey);
                if (val == null || val == default(Dictionary<string, ConnectionConfig[]>))
                    Reload = true;
                else
                {
                    var value = val as DataBase;
                    this.Data = value.Data;
                    this.ConfigFileAttribute = value.ConfigFileAttribute;
                    return this as DataBase;
                }
            }
            if (reload || Reload)
            {
                var val = this.ReadContent();
                if (attr.Format == ConfigFormat.Json)
                {
                    this.Data = val.JsonToObject<Dictionary<string, ConnectionConfig[]>>();
                }
                else if (attr.Format == ConfigFormat.Xml)
                {
                    this.Data = val.XmlToEntity<Dictionary<string, ConnectionConfig[]>>();
                }
                else if (attr.Format == ConfigFormat.Ini)
                {
                    this.Data = new Dictionary<string, ConnectionConfig[]>();
                }
                if (this.ConfigFileAttribute == null) this.ConfigFileAttribute = attr;
                if (Reload) cache.Set(attr.CacheKey, this, attr.FileName);
            }
            return this;
        }
        #endregion

        #region 读取配置文件内容
        /// <summary>
        /// 读取配置文件内容
        /// </summary>
        /// <returns></returns>
        public override string ReadContent()
        {
            var attr = this.ConfigFileAttribute;
            if (File.Exists(attr.FileName))
            {
                return this.OpenFile(attr.FileName);
                /*string val = string.Empty;
                using (StreamReader sr = new StreamReader(
                            new FileStream(attr.FileName, FileMode.Open, FileAccess.Read, FileShare.Read),
                            Encoding.UTF8))
                {
                    val = sr.ReadToEnd();
                }
                return val;*/
            }
            else
            {
                this.Data = new Dictionary<string, ConnectionConfig[]>
                {
                    {"www.zhuovi.com",new ConnectionConfig[]
                        {
                            new ConnectionConfig
                            {
                                ProviderType = DbProviderType.SqlServer,
                                ConnectionString = ""
                            }
                        }
                    }
                };
                this.Save();
                if (attr.Format == ConfigFormat.Json)
                {
                    return this.Data.ToJson();
                }
                else if (attr.Format == ConfigFormat.Xml)
                {
                    return this.Data.EntityToXml();
                }
                else if (attr.Format == ConfigFormat.Ini)
                {

                }
                return "";
            }
        }
        #endregion

        #region 保存配置文件
        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="indented">是否格式化</param>
        /// <param name="comment">是否带注释说明</param>
        public override bool Save(bool indented = true, bool comment = true)
        {
            var attr = this.ConfigFileAttribute;
            if (File.Exists(attr.FileName)) File.Delete(attr.FileName);
            FileHelper.Create(Path.GetDirectoryName(attr.FileName), FileAttribute.Directory);
            using (var sw = new StreamWriter(
                        new FileStream(attr.FileName,
                        FileMode.OpenOrCreate,
                        FileAccess.Write,
                        FileShare.ReadWrite),
                        Encoding.UTF8))
            {
                string val = "";
                if (attr.Format == ConfigFormat.Json)
                {
                    val = this.Data.ToJson(new Json.JsonSerializerSetting
                    {
                        Indented = indented,
                        IsComment = comment
                    });
                }
                else if (attr.Format == ConfigFormat.Xml)
                {
                    val = this.Data.EntityToXml();
                }
                else if (attr.Format == ConfigFormat.Ini)
                {

                }
                sw.Write(val);
            }
            /*
            * 更新缓存,有延迟,故直接清除缓存
            * Cache.CacheHelper.Set(attr.CacheKey, this as DataBase, attr.FileName);
            */
            CacheFactory.Create(CacheType.Memory).Remove(attr.CacheKey);
            return true;
        }
        #endregion

        #region 更新配置
        /// <summary>
        /// 更新配置
        /// <para>Set("key", 1, null)则为删除此节点</para>
        /// <para>Set("key", 1, config)则为更新此节点 不存在则添加当前节点</para>
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引 -1是删除当前节点</param>
        /// <param name="config">配置 如果为null则为删除此节点</param>
        /// <returns></returns>
        public void Set(string key, int index = 0, ConnectionConfig config = null)
        {
            if (key.IsNullOrEmpty()) return;
            if (this.Data == null || this.Data == default(Dictionary<string, ConnectionConfig[]>))
            {
                this.Data = new Dictionary<string, ConnectionConfig[]>();
            }
            if (this.Data.ContainsKey(key))
            {
                if (index == -1)
                {
                    this.Data.Remove(key); Save(); return;
                }
                var list = this.Data[key];
                if (config == null)
                {
                    if (index >= list.Length) return;
                    var _list = list.ToList<ConnectionConfig>();
                    _list.RemoveAt(index);
                    this.Data[key] = _list.ToArray<ConnectionConfig>();
                    if (this.Data[key].Length == 0) this.Data.Remove(key);
                }
                else
                {
                    if (index >= list.Length)
                    {
                        index = list.Length;
                        var _list = new ConnectionConfig[index + 1];
                        list.CopyTo(_list, 0);
                        list = _list;
                    }
                    list[index] = config;
                    this.Data[key] = list;
                }
            }
            else
            {
                this.Data.Add(key, new ConnectionConfig[] { config });
            }
            Save();
        }
        /// <summary>
        /// 添加配置节点
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="configs">配置数据</param>
        public void Add(string key, ConnectionConfig[] configs)
        {
            if (key.IsNullOrEmpty()) return;
            if (this.Data.ContainsKey(key))
                this.Data[key] = configs;
            else
                this.Data.Add(key, configs);
            Save();
        }
        #endregion

        #region 移除节点
        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="index">索引 -1是删除当前key下所有节点 默认为-1</param>
        public void Remove(string key, int index = -1)
        {
            this.Set(key, index);
        }
        #endregion

        #endregion
    }
}