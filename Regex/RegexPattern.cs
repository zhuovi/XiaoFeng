using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-08-11 17:22:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 正则格式
    /// Verstion : 1.0.0
    /// Create Time : 2017/8/11 17:22:57
    /// Update Time : 2017/8/11 17:22:57
    /// </summary>
    public static class RegexPattern
    {
        #region 属性
        
        #region 格式正则
        /// <summary>
        /// 物理路径
        /// </summary>
        public const string BasePath = @"^[a-z]+:(\\|\/)";
        /// <summary>
        /// 汉字格式
        /// </summary>
        public const string Chinese = @"^[\u4e00-\u9fa5？，“”‘’。、；：（）·！￥]+$";
        /// <summary>
        /// 字母格式
        /// </summary>
        public const string Letter = @"^[a-zA-Z]+$";
        /// <summary>
        /// FTP格式
        /// </summary>
        public const string Ftp = @"^ftp:\/\/[\w\-_]+(\.[\w\-_]+)*(:\d+)?([\s\S]*)$";
        /// <summary>
        /// 网址格式
        /// </summary>
        public const string Site = @"^http(s)?:\/\/[\w\-_]+(\.[\w\-_]+)*(:\d+)?([\s\S]*)$";
        /// <summary>
        /// GUID格式
        /// </summary>
        //public const string guid = @"^(([a-z0-9]{8}(-?)[a-z0-9]{4}\3[4][a-z0-9]{3}\3[a-z0-9]{4}\3[a-z0-9]{12})|(0{8}(-?)[0]{4}\5[0]{4}\5[0]{4}\5[0]{12}))$";
        public const string Guid = @"^(([a-z0-9]{8}(-?)[a-z0-9]{4}\3[a-z0-9]{4}\3[a-z0-9]{4}\3[a-z0-9]{12})|(0{8}(-?)[0]{4}\5[0]{4}\5[0]{4}\5[0]{12}))$";
        /// <summary>
        /// Email格式
        /// </summary>
        public const string Email = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        /// <summary>
        /// 数字格式
        /// </summary>
        public const string Numberic = @"^(\+|-)?\d+$";
        /// <summary>
        /// 浮点格式
        /// </summary>
        public const string Float = @"^(\+|-)?\d+([.]\d+)?$";
        /// <summary>
        /// 固话格式
        /// </summary>
        public const string Tel = @"^([+0]?(86[-/]?)?)?(((0\d{2,3})|\d{2,3})[-/]?)?[1-9]\d{6,7}$";
        /// <summary>
        /// 手机格式
        /// </summary>
        public const string Phone = @"^([+0]?(86[-/]?)?)?1[3456789]\d{9}$";
        /// <summary>
        /// 日期格式
        /// </summary>
        public const string Date = @"^((\d{2}|\d{4})(-|\/)(\d{1,2})\3(\d{1,2})|((\d{2}|\d{4})年(\d{1,2})月(\d{1,2})日))$";
        /// <summary>
        /// 时间格式
        /// </summary>
        public const string Time = @"^\d{1,2}\:\d{1,2}(\:\d{1,2}(\.\d+)?)?$";
        /// <summary>
        /// 完整日期时间格式
        /// </summary>
        public const string DateTime = @"^(((\d{2}|\d{4})(-|\/)(\d{1,2})\4(\d{1,2})(\s+|T)\d{1,2}\:\d{1,2}(\:\d{1,2}(\.\d+)?)?Z?)|((\d{2}|\d{4})年(\d{1,2})月(\d{1,2})日(\s*)\d{1,2}(时|点)\d{1,2}分(\d{1,2}秒)?))$";
        /// <summary>
        /// 日期格式 日期或日期+时间
        /// </summary>
        public const string DateOrTime = @"^((\d{2}|\d{4})(-|\/)(\d{1,2})\3(\d{1,2})|((\d{2}|\d{4})年(\d{1,2})月(\d{1,2})日))((\s*|T)((\d{1,2}\:\d{1,2}(\:\d{1,2}(\.\d+)?)?Z?)|(\d{1,2}(时|点)\d{1,2}分(\d{1,2}秒)?)))?$";
        /// <summary>
        /// IP格式
        /// </summary>
        public const string IP = @"^\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}$";
        /// <summary>
        /// Boolean格式
        /// </summary>
        public const string Boolean = @"^(true|false)$";
        /// <summary>
        /// 参数
        /// </summary>
        public const string Query = @"^([^=&]+=[^&]*&?)*$";
        /// <summary>
        /// Json
        /// </summary>
        public const string Json = @"^[\s\r\n\t]*(({[\s\S]+})|(\[[\s\S]+\]))[\s\r\n\t]*$";
        /// <summary>
        /// Xml
        /// </summary>
        public const string Xml = @"((^<\?xml[^>]+?\?>)|(<([a-z0-9]+?)(\s+[^>]*?)>[\s\S]*?</\3>)|(<[a-z0-9]+?[^>]*?/>))";
        /// <summary>
        /// this索引器
        /// </summary>
        public const string Indexer = @"\s+Item\s+\[[^\[\]]+\]$";
        #endregion

        #endregion

        #region 方法
        
        #endregion
    }
}