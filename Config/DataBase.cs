using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XiaoFeng.Cache;
using XiaoFeng.Data;
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
    /// 数据库配置
    /// </summary>
    [ConfigFile("Config/DataBase.json", 0, "FAYELF-CONFIG-DATABASES", ConfigFormat.Json)]
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
        public Dictionary<string, List<ConnectionConfig>> Data { get; set; }
        /// <summary>
        /// 获取配置数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public new List<ConnectionConfig> this[string key] => this.Get(key);
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
        public List<ConnectionConfig> Get(string key = "")
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
                    this.Data = val.JsonToObject<Dictionary<string, List<ConnectionConfig>>>();
                }
                else if (attr.Format == ConfigFormat.Xml)
                {
                    this.Data = val.XmlToEntity<Dictionary<string, List<ConnectionConfig>>>();
                }
                else if (attr.Format == ConfigFormat.Ini)
                {
                    this.Data = new Dictionary<string, List<ConnectionConfig>>();
                }
                if (this.ConfigFileAttribute == null) this.ConfigFileAttribute = attr;

                this.VerifyConfig();

                if (Reload) cache.Set(attr.CacheKey, this, attr.FileName);
            }
            return this;
        }
        #endregion

        #region 验证配置中的节点值是否，不存在则默认生成
        /// <summary>
        /// 验证配置中的节点值是否，不存在则默认生成
        /// </summary>
        private void VerifyConfig()
        {
            if (this.Data != null && this.Data.Count > 0)
            {
                var isSave = false;
                this.Data.Keys.Each(k =>
                {
                    var configs = this.Data[k];
                    configs.Each(c =>
                    {
                        if (c.AppKey.IsNullOrEmpty())
                        {
                            c.AppKey = k;
                            isSave = true;
                        }
                        if (c.Id.IsNullOrEmpty())
                        {
                            c.Id = Guid.NewGuid().ToString("N");
                            isSave = true;
                        }
                    });
                    if (isSave)
                    {
                        this.Save();
                    }
                });
            }
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
                this.Data = new Dictionary<string, List<ConnectionConfig>>
                {
                    {"www.eelf.cn",new List <ConnectionConfig>
                        {
                            new ConnectionConfig
                            {
                                ProviderType = DbProviderType.SqlServer,
                                ConnectionString = "server=.;database=test;uid=sa;pwd=123;"
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
                this.Data.Keys.Each(k =>
                {
                    var configs = this.Data[k];
                    configs.Each(c =>
                    {
                        c.AppKey = k;
                        if (c.Id.IsNullOrEmpty()) c.Id = Guid.NewGuid().ToString("N");
                    });
                });
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
            if (this.Data == null || this.Data == default(Dictionary<string, List<ConnectionConfig>>))
            {
                this.Data = new Dictionary<string, List<ConnectionConfig>>();
            }
            if (config.AppKey.IsNullOrEmpty() || !config.AppKey.EqualsIgnoreCase(key)) config.AppKey = key;
            if (config.Id.IsNullOrEmpty()) config.Id = Guid.NewGuid().ToString("N");
            if (this.Data.TryGetValue(key,out var list))
            {
                if (index == -1)
                {
                    this.Data.Remove(key);
                    Save(); 
                    return;
                }
                //var list = this.Data[key];
                if (config == null)
                {
                    if (index >= list.Count) return;
                    var _list = list.ToList<ConnectionConfig>();
                    _list.RemoveAt(index);
                    this.Data[key] = _list;
                    if (this.Data[key].Count == 0) this.Data.Remove(key);
                }
                else
                {
                    if (index >= list.Count)
                    {
                        list.Add(config);
                    }else
                        list[index] = config;
                    this.Data[key] = list;
                }
            }
            else
            {
                this.Data.Add(key, new List<ConnectionConfig> { config });
            }
            Save();
        }
        /// <summary>
        /// 添加配置节点
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="configs">配置数据</param>
        public void Add(string key, List<ConnectionConfig> configs)
        {
            if (key.IsNullOrEmpty()) return;
            if (this.Data.ContainsKey(key))
                this.Data[key] = configs;
            else
                this.Data.Add(key, configs);
            configs.Each(c =>
            {
                if (c.AppKey.IsNullOrEmpty() || !c.AppKey.EqualsIgnoreCase(key))
                    c.AppKey = key;
                if (c.Id.IsNullOrEmpty()) c.Id = Guid.NewGuid().ToString("N");
            });
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