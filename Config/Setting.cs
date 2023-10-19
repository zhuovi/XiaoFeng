using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Cache;
using XiaoFeng.Json;
using XiaoFeng.Log;
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
    /// XiaoFeng总配置
    /// </summary>
    [ConfigFile("Config/XiaoFeng.json", 0, "FAYELF-CONFIG-XIAOFENG", ConfigFormat.Json)]
    public class Setting : ConfigSet<Setting>, ISetting
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Setting() : base() { }
        /// <summary>
        /// 设置配置文件名
        /// </summary>
        /// <param name="fileName"></param>
        public Setting(string fileName) : base(fileName) { }
        #endregion

        #region 属性
        /// <summary>
        /// 是否启用调试
        /// </summary>
        [Description("是否启用调试")]
        public bool Debug { get; set; } = true;
        /// <summary>
        /// 最大线程数量
        /// </summary>
        [Description("最大线程数量")]
        public int MaxWorkerThreads { get; set; } = 100;
        /// <summary>
        /// 消费日志空闲时长
        /// </summary>
        [Description("消费日志空闲时长")]
        public int IdleSeconds { get; set; } = 60;
        /// <summary>
        /// 调度最小等待时长 单位为秒
        /// </summary>
        private int _JobSchedulerWaitTimeout = 60 * 60;
        /// <summary>
        /// 调度最小等待时长 单位为秒
        /// </summary>
        [Description("调度最小等待时长")]
        public int JobSchedulerWaitTimeout
        {
            get
            {
                if (this._JobSchedulerWaitTimeout <= 0) this._JobSchedulerWaitTimeout = 60 * 60;
                return this._JobSchedulerWaitTimeout;
            }
            set
            {
                if (value <= 0) value = 60 * 60;
                this._JobSchedulerWaitTimeout = value;
            }
        }
        /// <summary>
        /// 作业调度日志等级
        /// </summary>
        [Description("作业调度日志等级")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LogType? JobLogLevel { get; set; } = LogType.Trace;
        /// <summary>
        /// 作业记录记录次数
        /// </summary>
        private int _JobMessageCount = 0;
        /// <summary>
        /// 作业记录记录次数
        /// </summary>
        [Description("作业记录记录次数")]
        public int JobMessageCount
        {
            get
            {
                if (this._JobMessageCount == 0) this._JobMessageCount = 10;
                return this._JobMessageCount;
            }
            set => this._JobMessageCount = value;
        }
        /// <summary>
        /// 任务队列执行任务超时时间 单位为秒
        /// </summary>
        private int _TaskWaitTimeout = 5 * 60;
        /// <summary>
        /// 任务队列执行任务超时时间 单位为秒
        /// </summary>
        [Description("任务队列执行任务超时时间")]
        public int TaskWaitTimeout {
            get
            {
                if (this._TaskWaitTimeout == 0)
                    this._TaskWaitTimeout = 300;
                return this._TaskWaitTimeout ;
            }
            set
            {
                this._TaskWaitTimeout  = value;
            }
        }
        /// <summary>
        /// 是否启用数据加密
        /// </summary>
        [Description("是否启用数据加密")]
        public bool DataEncrypt { get; set; } = false;
        /// <summary>
        /// 加密数据key
        /// </summary>
        [Description("加密数据key")]
        public string DataKey { get; set; } = "7092734";
        /// <summary>
        /// 是否开启请求日志
        /// </summary>
        [Description("是否开启请求日志")]
        public bool ServerLogging { get; set; }
        /// <summary>
        /// 是否拦截
        /// </summary>
        [Description("是否拦截")]
        public bool IsIntercept { get; set; }
        /// <summary>
        /// SQL注入串
        /// </summary>
        [Description("SQL注入串")]
        public string SQLInjection { get; set; } = @"insert\s+into |update |delete |select | union | join |exec |execute | exists|'|truncate |create |drop |alter |column |table |dbo\.|sys\.|alert\(|<scr|ipt>|<script|confirm\(|console\.|\.js|<\/\s*script>|now\(\)|getdate\(\)|time\(\)| Directory\.| File\.|FileStream |\.Write\(|\.Connect\(|<\?php|show tables |echo | outfile |Request[\.\(]|Response[\.\(]|eval\s*\(|\$_GET|\$_POST|cast\(|Server\.CreateObject|VBScript\.Encode|replace\(|location|\-\-";
        #endregion
    }
}