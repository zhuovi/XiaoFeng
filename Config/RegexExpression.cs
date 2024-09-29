using System.ComponentModel;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-04 10:40:14                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Config
{
    /// <summary>
    /// 正则表达式配置
    /// </summary>
    [ConfigFile("/Config/RegexExpression.json", 0, "FAYELF-CONFIG-REGEXEXPRESSION", ConfigFormat.Json)]
    public class RegexExpression : ConfigSet<RegexExpression>
    {
        /// <summary>
        /// 汉字正则
        /// </summary>
        [Description("汉字正则")]
        public string Chinese { get; set; } = @"^[\u4e00-\u9fa5？，“”‘’。、；：（）·！￥]+$";
        /// <summary>
        /// FTP正则
        /// </summary>
        [Description("FTP正则")]
        public string Ftp { get; set; } = @"^ftp:\/\/[\w\-_]+(\.[\w\-_]+)*(:\d+)?([\s\S]*)$";
        /// <summary>
        /// 网址正则
        /// </summary>
        [Description("网址正则")]
        public string Site { get; set; } = @"^http(s)?:\/\/[\w\-_]+(\.[\w\-_]+)*(:\d+)?([\s\S]*)$";
        /// <summary>
        /// GUID正则
        /// </summary>
        [Description("GUID正则")]
        public string @Guid { get; set; } = @"^(([a-z0-9]{8}(-?)[a-z0-9]{4}\3[a-z0-9]{4}\3[a-z0-9]{4}\3[a-z0-9]{12})|(0{8}(-?)[0]{4}\5[0]{4}\5[0]{4}\5[0]{12}))$";
        /// <summary>
        /// Email正则
        /// </summary>
        [Description("Email正则")]
        public string Email { get; set; } = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        /// <summary>
        /// 固话正则
        /// </summary>
        [Description("固话正则")]
        public string Tel { get; set; } = @"^([+0]?(86[-/]?)?)?(((0\d{2,3})|\d{2,3})[-/]?)?[1-9]\d{6,7}$";
        /// <summary>
        /// 手机正则
        /// </summary>
        [Description("手机正则")]
        public string Phone { get; set; } = @"^([+0]?(86[-/]?)?)?1[3456789]\d{9}$";
        /// <summary>
        /// 日期正则
        /// </summary>
        [Description("日期正则")]
        public string Date { get; set; } = @"^((\d{2}|\d{4})(-|\/)(\d{1,2})\3(\d{1,2})|((\d{2}|\d{4})年(\d{1,2})月(\d{1,2})日))$";
        /// <summary>
        /// 时间正则
        /// </summary>
        [Description("时间正则")]
        public string Time { get; set; } = @"^\d{1,2}\:\d{1,2}(\:\d{1,2}(\.\d+)?)?$";
        /// <summary>
        /// 完整日期时间正则
        /// </summary>
        [Description("完整日期时间正则")]
        public string DateTime { get; set; } = @"^(((\d{2}|\d{4})(-|\/)(\d{1,2})\4(\d{1,2})(\s+|T)\d{1,2}\:\d{1,2}(\:\d{1,2}(\.\d+)?)?Z?)|((\d{2}|\d{4})年(\d{1,2})月(\d{1,2})日(\s*)\d{1,2}(时|点)\d{1,2}分(\d{1,2}秒)?))$";
        /// <summary>
        /// 日期或日期+时间正则
        /// </summary>
        [Description("日期或日期+时间正则")]
        public string DateOrTime { get; set; } = @"^((\d{2}|\d{4})(-|\/)(\d{1,2})\3(\d{1,2})|((\d{2}|\d{4})年(\d{1,2})月(\d{1,2})日))((\s*|T)((\d{1,2}\:\d{1,2}(\:\d{1,2}(\.\d+)?)?Z?)|(\d{1,2}(时|点)\d{1,2}分(\d{1,2}秒)?)))?$";
        /// <summary>
        /// IP正则
        /// </summary>
        [Description("IP正则")]
        public string IP { get; set; } = @"^\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}$";
        /// <summary>
        /// 布尔值正则
        /// </summary>
        [Description("布尔值正则")]
        public string Boolean { get; set; } = @"^(true|false)$";
        /// <summary>
        /// 参数正则
        /// </summary>
        [Description("参数正则")]
        public string Query { get; set; } = @"^([^=&]+=[^&]*&?)*$";
        /// <summary>
        /// this索引器
        /// </summary>
        [Description("this索引器")] 
        public string Indexer { get; set; } = @"\s+Item\s+\[[^\[\]]+\]$";
        /// <summary>
        /// 内网IP正则
        /// </summary>
        [Description("内网IP正则")] 
        public string IntranetIp { get; set; } = @"(^(192\.168|10|172\.(1[6789]|2[0-9]|3[0-1]))\.|^(::1|127\.0\.0\.1)$)";
        /// <summary>
        /// 本地IP
        /// </summary>
        [Description("本地IP")] 
        public string LocalIp { get; set; } = @"^(::1|127.0.0.1)$";
    }
}