using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using XiaoFeng.IO;
using XiaoFeng.Log;
using XiaoFeng.Threading;
#if NETCORE
using Microsoft.AspNetCore.Http;
#endif
namespace XiaoFeng.IIS
{
    #region 日志Model
    /// <summary>
    /// 日志Model
    /// </summary>
    [Serializable]
    [XmlRoot("Server")]
    public class LoggingModel : Disposable
    {
        #region 构造器
        /// <summary>
        /// 初始化
        /// </summary>
        public LoggingModel()
        {
            this.DataPath = this.DataPath.GetBasePath();
        }
        /// <summary>
        /// 初始化Fields数据
        /// </summary>
        /// <param name="init">是否初始化</param>
        public LoggingModel(bool init) : this()
        {
            if (!init) return;
            this.Fields = new List<LoggingField>
            {
                new LoggingField{ Name="Start Time(开始时间)",Value="StartTime"},
                new LoggingField{ Name="End Time(结束时间)",Value="EndTime"},
                new LoggingField{ Name="Process ID(进程PID)",Value="PID" },
                new LoggingField{ Name="CPU Usage(CPU占用率)",Value="CPU" },
                new LoggingField{ Name="RAM(内存)",Value="RAM" },
                new LoggingField{ Name="Run RAM(内存)",Value="RunRAM" },
                new LoggingField{ Name="Client IP(客户端IP)",Value="ClientIP" },
                new LoggingField{ Name="Host(域名)",Value="Host" },
                new LoggingField{ Name="Port(端口)",Value="Port" },
                new LoggingField{ Name="Scheme(Uri方案)",Value="Scheme" },
                new LoggingField{ Name="Method(请求类型)",Value="Method" },
                new LoggingField{ Name="PathAndQuery(请求路径及参数)",Value="Path" },
                new LoggingField{ Name="Body(请求体)",Value="InputBody" },
                new LoggingField{ Name="Referrer(过来地址)",Value="Referrer" },
                new LoggingField{ Name="Headers(请求头)",Value="Headers" },
                new LoggingField{ Name="UserAgent(用户代理)",Value="UserAgent" },
                new LoggingField{ Name="Post Data(Post数据)",Value="Post" },
                new LoggingField{ Name="Cookies(请求Cookies)",Value="Cookies" },
                new LoggingField{ Name="Status(响应状态)",Value="Status" },
                new LoggingField{ Name="Times(响应时间)",Value="Times" },
                new LoggingField{ Name="Site Path(网站路径)",Value="SitePath" }
            };
        }
        /// <summary>
        /// 设置配置文件地址
        /// </summary>
        /// <param name="DataPath">配置文件地址</param>
        public LoggingModel(string DataPath)
        {
            this.DataPath = DataPath.GetBasePath();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 静态属性
        /// </summary>
        public static LoggingModel Current { get { return new LoggingModel(); } }
        /// <summary>
        /// 配置路径
        /// </summary>
        private string DataPath { get; set; } = "/Config/ServerLogging.xml";
        /// <summary>
        /// 数据库缓存键
        /// </summary>
        private const string CacheKey = "ZW_ServerLoggingConfig";
        /// <summary>
        /// 监控超过指定时长(超过这个时长则要存放另一个地方)
        /// </summary>
        [XmlElement("MonitorTime")]
        public long MonitorTime { get; set; } = 4000;
        /// <summary>
        /// 日志路径
        /// </summary>
        [XmlElement("LogPath")]
        public string LogPath { get; set; } = "LogFiles";
        /// <summary>
        /// 过滤文件后缀
        /// </summary>
        [XmlElement("Filter")]
        public string Filter { get; set; } = "doc|exe|xls|pdf|docx|xlsx|pdfx|mdb|rar|woff|woff2|svg|otf|eot|apk|ipa|mp3|mp4|wmv|avi|ico";
        /// <summary>
        /// 存储类型
        /// </summary>
        [XmlElement("Storage")]
        [XiaoFeng.Xml.XmlConverter(typeof(XiaoFeng.Xml.StringEnumConverter))]
        public StorageType Storage { get; set; } = StorageType.File;
        /// <summary>
        /// 显示字段
        /// </summary>
        [XmlArrayItem("Field")]
        [XmlElement("Fields")]
        public List<LoggingField> Fields { get; set; }
        /// <summary>
        /// 日志对象
        /// </summary>
        private static ILog Log { get; set; } = null;
        /// <summary>
        /// 日志队列
        /// </summary>
        private static IBackgroundTaskQueue LogQueue = new BackgroundTaskQueue("RequestLogQueue");
        #endregion

        #region 方法
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="context">请求对象</param>
        /// <param name="StartTime">开始请求时间</param>
        /// <param name="EndTime">响应结束时间</param>
        /// <param name="process">进程</param>
        /// <param name="processStartTime">进程开始时间</param>
        /// <param name="ResponseTime">响应时间</param>
        /// <param name="RunRAM">运行内存</param>
#if NETFRAMEWORK
        public void WriteLog(HttpApplication context, DateTime StartTime, DateTime EndTime, Process process, TimeSpan processStartTime, long ResponseTime, long RunRAM = 0)
        {
            System.Web.HttpRequest Request = context.Request;
            System.Web.HttpResponse Response = context.Response;
       
            Uri url = Request.Url;
            var config = this.Data;
            if (config.Filter.IsNotNullOrEmpty())
            {
                if (url.LocalPath.IsMatch(@"\.(" + config.Filter + @")$")) return;
            }
            string Refer = Request.Headers["Referrer"].ToString();
            string Cookies = string.Empty, PostData = string.Empty;
            Request.Cookies.Keys.ToList<string>().Each(c =>
            {
                Cookies += c + "=" + Request.Cookies[c] + "&";
            });
            if (Request.HttpMethod == "POST")
            {
                Request.Form.Keys.ToList<string>().Each(p =>
                {
                    PostData += p + "=" + Request.Form[p] + "&";
                });
            }
            string InputBody = string.Empty;
            try
            {
                if (Request.InputStream.Length > 0 && Request.InputStream.Length < 3000)
                {
                    Request.InputStream.Position = 0;
                    using (var sr = new StreamReader(Request.InputStream)) InputBody = sr.ReadToEnd();
                }
            }
            catch { }
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    { "StartTime", StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                    { "EndTime", EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                    { "PID", process.Id.ToString() },
                    { "CPU", (process.TotalProcessorTime - processStartTime).TotalMilliseconds / 1000 / Environment.ProcessorCount + "%" },
                    { "RAM", Math.Round(process.WorkingSet64 / (double)1024 / 1024, 2).ToString() +"M" },
                    { "RunRAM" , Math.Round(RunRAM/(double)1024/1024,4).ToString() +"M" },
                    { "Cookies", Cookies.TrimEnd('&') },
                    { "ClientIP", Device.Computer.GetIP4Address() },
                    { "Host", url.Host },
                    { "Port", url.Port.ToString() },
                    { "Scheme", url.Scheme },
                    { "Method", Request.HttpMethod },
                    { "Path", url.PathAndQuery },
                    { "InputBody", InputBody },
                    { "Referrer", Refer },
                    { "Headers",HttpUtility.UrlDecode(Request.Headers.ToString()) },
                    { "UserAgent", Request.Headers["User-Agent"] },
                    { "Post", PostData.TrimEnd('&') },
                    { "Status", Response.StatusCode + "[" + Response.StatusCode + "]" },
                    { "Times", ResponseTime +"毫秒"},
                    { "SitePath", OS.Platform.CurrentDirectory }
                };
            string RequestInfo = string.Empty;
            config.Fields.Each(a =>
            {
                RequestInfo += "{0}:{1}\r\n".format(a.Name, data.ContainsKey(a.Value) ? data[a.Value] : "");
            });
            string logPath = (config.LogPath.IsNullOrEmpty() ? "LogFiles" : config.LogPath).GetBasePath();
            if (ResponseTime >= config.MonitorTime) logPath += @"/Monitor";
            if (Log == null)
            {
                Log = LogFactory.Create(typeof(Logger), "IISLogTaskQueue");
                Log.LogPath = logPath;
            }
            LogQueue.AddWorkItem(() =>
            {
                Log.Info(RequestInfo);
            });
            //var log = LogFactory.Create(typeof(Logger));
            //log.LogPath = logPath;
            //log.Info(RequestInfo);
            //new Logger(logPath).Info(RequestInfo);
        }
#else
        public void WriteLog(HttpContext context, DateTime StartTime, DateTime EndTime, Process process, TimeSpan processStartTime, long ResponseTime, long RunRAM = 0)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
       
            Uri url = Request.GetUri();
            var config = this.Data;
            if (config.Filter.IsNotNullOrEmpty())
            {
                if (url.LocalPath.IsMatch(@"\.(" + config.Filter + @")$")) return;
            }
            string Refer = Request.Headers["Referrer"].ToString();
            string Cookies = string.Empty, PostData = string.Empty;
            Request.Cookies.Keys.Each(c =>
            {
                Cookies += c + "=" + Request.Cookies[c] + "&";
            });
            string InputBody = string.Empty;
            if (Request.Method == "POST")
            {
                if (Request.HasFormContentType)
                {
                    Request.Form.Keys.Each(p =>
                    {
                        PostData += p + "=" + Request.Form[p] + "&";
                    });
                }
                else
                {
                    try
                    {
                        if (Request.Body.Length > 0 && Request.Body.Length < 3000)
                        {
                            Request.Body.Position = 0;
                            using (var sr = new StreamReader(Request.Body)) InputBody = sr.ReadToEnd();
                        }
                    }
                    catch { }
                }
            }
            var Headers = "";
            Request.Headers.Keys.Each(k =>
            {
                Headers += k + "=" + Request.Headers[k].ToString() + "&";
            });
            Dictionary<string, string> data = new Dictionary<string, string>
                {
                    { "StartTime", StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                    { "EndTime", EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                    { "PID", process.Id.ToString() },
                    { "CPU", (process.TotalProcessorTime - processStartTime).TotalMilliseconds / 1000 / Environment.ProcessorCount + "%" },
                    { "RAM", Math.Round(process.WorkingSet64 / (double)1024 / 1024, 2).ToString() +"M" },
                    { "RunRAM" , Math.Round(RunRAM/(double)1024/1024,4).ToString() +"M" },
                    { "Cookies", Cookies.TrimEnd('&') },
                    { "ClientIP", Device.Computer.GetIP4Address() },
                    { "Host", url.Host },
                    { "Port", url.Port.ToString() },
                    { "Scheme", url.Scheme },
                    { "Method", Request.Method },
                    { "Path", url.PathAndQuery },
                    { "InputBody", InputBody },
                    { "Referrer", Refer },
                    { "Headers",HttpUtility.UrlDecode(Headers.TrimEnd('&')) },
                    { "UserAgent", Request.Headers["User-Agent"] },
                    { "Post", PostData.TrimEnd('&') },
                    { "Status", Response.StatusCode + "[" + Response.StatusCode + "]" },
                    { "Times", ResponseTime +"毫秒"},
                    { "SitePath", OS.Platform.CurrentDirectory }
                };
            string RequestInfo = string.Empty;
            config.Fields.Each(a =>
            {
                RequestInfo += "{0}:{1}\r\n".format(a.Name, data.ContainsKey(a.Value) ? data[a.Value] : "");
            });
            string logPath = (config.LogPath.IsNullOrEmpty() ? "LogFiles" : config.LogPath).GetBasePath();
            if (ResponseTime >= config.MonitorTime) logPath += FileHelper.AltDirectorySeparatorChar + "Monitor";
            if (Log == null)
            {
                Log = LogFactory.Create(typeof(Logger), "IISLogTaskQueue");
                Log.LogPath = logPath;
                Log.StorageType = config.Storage;
            }
            LogQueue.AddWorkItem(() =>
            {
                Log.Info(RequestInfo);
            });
            //var log = LogFactory.Create(typeof(Logger));
            //log.LogPath = logPath;
            //log.Info(RequestInfo);
            //new Logger(logPath).Info(RequestInfo);
        }
#endif
        /// <summary>
        /// 配置数据
        /// </summary>
        public LoggingModel Data
        {
            get
            {
                var cache = Cache.CacheFactory.Create(CacheType.Memory);
                var data = cache.Get<LoggingModel>(CacheKey);
                if (data == null)
                {
                    if (this.DataPath.IsNullOrEmpty() || !this.DataPath.IsBasePath() || !File.Exists(this.DataPath))
                    {
                        data = new LoggingModel(true);
                        using (StreamWriter sw = File.CreateText(this.DataPath))
                        {
                            sw.Write(data.EntityToXml());
                        }
                    }
                    else
                    {
                        try
                        {
                            XmlDocument xml = new XmlDocument();
                            xml.Load(this.DataPath);
                            data = xml.OuterXml.XmlToEntity<LoggingModel>();
                        }
                        catch
                        {
                            data = new LoggingModel();
                        }
                    }
                    cache.Set(CacheKey, data, this.DataPath);
                    return data as LoggingModel;
                }
                return data as LoggingModel;
            }
        }
#endregion
    }
#endregion

#region 字段属性
    /// <summary>
    /// 字段属性
    /// </summary>
    public class LoggingField
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        [XmlAttribute("Value")]
        public string Value { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        [XmlAttribute("IsShow")]
        public bool IsShow { get; set; } = true;
    }
#endregion
}