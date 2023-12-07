using System;
using System.ComponentModel;
using XiaoFeng.Json;
using XiaoFeng.Log;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-05 11:10:58                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Config
{
    /// <summary>
    /// 日志配置
    /// </summary>
    [ConfigFile("Config/Logger.json", 0, "FAYELF-CONFIG-LOGGER", ConfigFormat.Json)]
    public class LoggerConfig : ConfigSet<LoggerConfig>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public LoggerConfig()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// 开启日志
        /// </summary>
        [Description("开启日志")]
        public Boolean OpenLog { get; set; } = true;
        /// <summary>
        /// 日志存储天数
        /// </summary>
        [Description("日志存储天数")]
        public int StorageDays { get; set; } = 30;
        /// <summary>
        /// 日志存储路径
        /// </summary>
        [Description("日志存储路径")]
        public string Path { get; set; } = "Log";
        /// <summary>
        /// 单文件最大内容长度 单位字节
        /// </summary>
        [Description("单文件最大内容长度 0不限制 单位字节")]
        public int FileLength { get; set; } = 10000000;
        /// <summary>
        /// 最大线程数
        /// </summary>
        private int _MaxThreads = 3;
        /// <summary>
        /// 最大线程数
        /// </summary>
        [Description("最大线程数")]
        public int MaxThreads
        {
            get
            {
                if (this._MaxThreads <= 0) this._MaxThreads = 1;
                return this._MaxThreads;
            }
            set
            {
                this._MaxThreads = value <= 0 ? 1 : value;
            }
        }
        /// <summary>
        /// 日志级别
        /// </summary>
        [Description("日志级别")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LogType LogLevel { get; set; } = LogType.Trace;
        /// <summary>
        /// 输出控制台标识
        /// </summary>
        [Description("输出控制台标识")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LogType ConsoleFlags { get; set; } = LogType.Info | LogType.Debug | LogType.Error | LogType.Trace | LogType.Task | LogType.Warn;
        /// <summary>
        /// 输出文件标识
        /// </summary>
        [Description("输出文件标识")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LogType FileFlags { get; set; } = LogType.Info | LogType.Debug | LogType.Error | LogType.Trace | LogType.Task | LogType.Warn;
        /// <summary>
        /// 输出数据库标识
        /// </summary>
        [Description("输出数据库标识")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LogType DataBaseFlags { get; set; } = LogType.Info | LogType.Debug | LogType.Error | LogType.Trace | LogType.Task | LogType.Warn;
        /// <summary>
        /// 存储类型
        /// </summary>
        [Description("存储类型")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StorageType StorageType { get; set; } = StorageType.Console | StorageType.File;
        /// <summary>
        /// 数据库连接字符串KEY
        /// </summary>
        [Description("数据库连接字符串KEY")]
        public string ConnectionStringKey { get; set; }
        /// <summary>
        /// 插入SQL 数据标识 日志类型 {LogType} 日志消息 {Message} 日志错误ID {ErrorID}
        /// 错误堆栈 {StackTrace} 堆栈跟踪 {Tracking} 错误源 {DataSource} 类名 {ClassName} 方法名 {FunctionName} 访问地址 {RequestUrl} 日志时间 {AddTime}
        /// </summary>
        [Description("插入SQL数据标识 日志类型 {LogType} 日志消息 {Message} 日志错误ID {ErrorID} 错误堆栈 {StackTrace} 堆栈跟踪 {Tracking} 错误源 {DataSource} 类名 {ClassName} 方法名 {FunctionName} 访问地址 {RequestUrl} 日志时间 {AddTime}")]
        public string InsertSql { get; set; } = "INSERT INTO ZW_Tb_Log(LogType,Message,StackTrace,Tracking,DataSource,AddTime) values('{LogType}','{Message}','{StackTrace}','{Tracking}','{DataSource}','{AddTime}');";
        /// <summary>
        /// 记录信息字段 不填是记录所有 
        /// </summary>
        [Description("记录信息字段")]
        public string Fields { get; set; } = "LogType,ErrorID,FunctionName,ClassName,Message,DataSource,StackTrace,Tracking,RequestUrl,AddTime";
        /// <summary>
        /// 消息类型展示
        /// </summary>
        [Description("消息类型展示")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EnumValueType MessageType { get; set; } = EnumValueType.Name;
        #endregion

        #region 方法

        #endregion
    }
}