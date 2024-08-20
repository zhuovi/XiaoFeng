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
    /// 配置基类
    /// </summary>
    public class ConfigSets<TConfig> : ConfigSet<TConfig>, IConfigSets<TConfig> where TConfig : ConfigSets<TConfig>, new()
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConfigSets() : base()
        {
        }
        /// <summary>
        /// 设置路径
        /// </summary>
        /// <param name="fileName">路径</param>
        public ConfigSets(string fileName) : base(fileName) { }
        /// <summary>
        /// 列表数据
        /// </summary>
        [Description("列表数据属性")]
        [JsonIgnore]
        [XmlArrayItem("Config")]
        public List<TConfig> List { get; set; }
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public override TConfig GetEntity(Func<TConfig, Boolean> func)
        {
            if (func == null) func = a => true;
            return this.GetEntities(func)?.FirstOrDefault();
        }
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public override IEnumerable<TConfig> GetEntities(Func<TConfig, Boolean> func)
        {
            if (func == null) func = a => true;
            return this.List.Any() ? this.List.Where(func) : null;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="reLoad">强制重新加载</param>
        /// <returns></returns>
        public override TConfig Get(bool reLoad = false)
        {
            var attr = this.ConfigFileAttribute;
            if (attr == null) return null;
            var cache = CacheFactory.Create(CacheType.Memory);
            if (!reLoad)
            {
                var val = cache.Get(attr.CacheKey);
                if (val == null || val == default(TConfig))
                    reLoad = true;
                else
                {
                    //var Config = new TConfig();
                    //typeof(TConfig).GetProperty("List", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(Config, val as List<TConfig>);
                    this.List = val as List<TConfig>;
                    return (TConfig)this;
                }
            }
            if (reLoad)
            {
                List<TConfig> list = null;
                var val = this.ReadContent();
                if (attr.Format == ConfigFormat.Json)
                {
                    list = val.JsonToObject<List<TConfig>>();
                }
                else if (attr.Format == ConfigFormat.Xml)
                {
                    list = val.XmlToEntity<List<TConfig>>();
                }
                else if (attr.Format == ConfigFormat.Ini)
                {

                }
                if (this.ConfigFileAttribute == null) this.ConfigFileAttribute = attr;
                cache.Set(attr.CacheKey, list, attr.FileName);
                //var Config = new TConfig();
                //typeof(TConfig).GetProperty("List", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).SetValue(Config, list);
                this.List = list;
                return (TConfig)this;
            }
            return null;
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="indented">是否格式化</param>
        /// <param name="comment">是否带注释说明</param>
        public override bool Save(Boolean indented = true, Boolean comment = true)
        {
            var attr = this.ConfigFileAttribute;
            if (attr == null) return false;
            var configPath = base.GetConfigPath(attr.FileName);
            var dirPath = attr.FileName.GetDirectoryName();
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            string val = "";
            if (this.List == null) return false;
            if (attr.Format == ConfigFormat.Json)
            {
                val = this.List.ToJson(new Json.JsonSerializerSetting
                {
                    Indented = indented,
                    IsComment = comment
                });
            }
            else if (attr.Format == ConfigFormat.Xml)
            {
                val = this.EntityToXml();
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
                CacheFactory.Create(CacheType.Memory).Remove(attr.CacheKey);
                return f;
            }
            return false;
        }
    }
}