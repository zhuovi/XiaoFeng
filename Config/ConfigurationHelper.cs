/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2020-05-05 上午 11:43:46          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.Config
{
#if NETFRAMEWORK
    /// <summary>
    /// ConfigurationHelper 类说明
    /// </summary>
    public class ConfigurationHelper
    {
#region 构造器
#if NETFRAMEWORK
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConfigurationHelper() : this(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile) { }
#else
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ConfigurationHelper():this("web.config".GetBasePath()){}
#endif
        /// <summary>
        /// 设置配置文件
        /// </summary>
        /// <param name="ConfigPath">配置文件</param>
        public ConfigurationHelper(string ConfigPath)
        {
            if (ConfigPath.IsNotNullOrEmpty())
            {
                this.Path = ConfigPath.IsBasePath() ? ConfigPath : (AppDomain.CurrentDomain.BaseDirectory + ConfigPath.Replace("/", "\\"));
                this.config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                {
                    ExeConfigFilename = this.Path
                }, ConfigurationUserLevel.None);
            }
        }
#endregion

#region 属性
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 操作对象
        /// </summary>
        public Configuration config { get; set; }
#endregion

#region 方法
        /// <summary>
        /// 获取配置结点内容
        /// </summary>
        /// <param name="key">配置结点名称</param>
        /// <returns></returns>
        public string Get(string key)
        {
            if (!this.config.HasFile) return "";
            return this.GetConfig(key).IsNullOrEmpty() ?
                this.GetConnectionString(key).ConnectionString
                : this.GetConfig(key);
        }
        /// <summary>
        /// 获取配置结点内容
        /// </summary>
        /// <param name="key">配置结点名称</param>
        /// <returns></returns>
        public string GetConfig(string key)
        {
            if (!this.config.HasFile) return "";
            return this.config.AppSettings.Settings[key].Value ?? "";
        }
        /// <summary>
        /// 获取配置结点内容
        /// </summary>
        /// <param name="key">配置结点名称</param>
        /// <returns></returns>
        public ConnectionStringSettings GetConnectionString(string key)
        {
            if (!this.config.HasFile) return null;
            return this.config.ConnectionStrings.ConnectionStrings[key];
        }
        /// <summary>
        /// 获取配置结点内容
        /// </summary>
        /// <param name="index">配置结点位置</param>
        /// <returns></returns>
        public ConnectionStringSettings GetConnectionString(int index)
        {
            if (!this.config.HasFile) return null;
            return this.config.ConnectionStrings.ConnectionStrings[index];
        }
        /// <summary>
        /// 更新配置结点内容
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public Boolean SetAppSetting(string key, string value)
        {
            if (!this.config.HasFile) return false;
            AppSettingsSection appSettings = (AppSettingsSection)this.config.GetSection("AppSettings");
            if (appSettings.Settings[key] == null)
                appSettings.Settings.Add(key, value);
            else
            {
                appSettings.Settings.Remove(key);
                appSettings.Settings.Add(key, value);
            }
            try { this.config.Save(); return true; }
            catch (System.IO.IOException ex) { LogHelper.WriteLog(ex); return false; }
        }
        /// <summary>
        /// 移除配置结点内容
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public Boolean RemoveAppSetting(string key)
        {
            if (!this.config.HasFile) return false;
            AppSettingsSection appSettings = (AppSettingsSection)this.config.GetSection("AppSettings");
            if (appSettings.Settings[key] == null) return true;
            else
                appSettings.Settings.Remove(key);
            try { this.config.Save(); return true; }
            catch { return false; }
        }
        /// <summary>
        /// 更新配置结点内容
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="provider">Provider Name</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public Boolean SetConnectionString(string key, string provider, string value)
        {
            if (!this.config.HasFile) return false;
            ConnectionStringsSection ConnectionString = (ConnectionStringsSection)this.config.GetSection("ConnectionStrings");
            if (ConnectionString.ConnectionStrings[key] == null)
            {
                ConnectionStringSettings Connection = new ConnectionStringSettings(key, value, provider);
                ConnectionString.ConnectionStrings.Add(Connection);
            }
            else
            {
                ConnectionString.ConnectionStrings[key].ConnectionString = value;
                ConnectionString.ConnectionStrings[key].ProviderName = provider;
            }
            try { this.config.Save(); return true; }
            catch (System.IO.IOException ex) { LogHelper.WriteLog(ex); return false; }
        }
        /// <summary>
        /// 移除配置结点内容
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public Boolean RemoveConnectionString(string key)
        {
            if (!this.config.HasFile) return false;
            ConnectionStringsSection ConnectionString = (ConnectionStringsSection)this.config.GetSection("ConnectionStrings");
            if (ConnectionString.ConnectionStrings[key] == null) return true;
            else
                ConnectionString.ConnectionStrings.Remove(key);
            try { this.config.Save(); return true; }
            catch (System.IO.IOException ex) { LogHelper.WriteLog(ex); return false; }
        }
        /// <summary>
        /// 获取指定配置节点
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <returns></returns>
        public object GetSection(string name)
        {
#if NETFRAMEWORK
            name = name.ReplacePattern(@":","/");
#else
            name = name.ReplacePattern(@"/", ":");
#endif
            return ConfigurationManager.GetSection(name);
        }
#endregion
    }
#endif
}