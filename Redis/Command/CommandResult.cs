using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-06-20 上午 02:43:32                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 命令响应结果
    /// </summary>
    public class CommandResult
    {
        #region 构造器
        /// <summary>
        /// 设置结果行
        /// </summary>
        /// <param name="commandType">命令</param>
        /// <param name="line">结果行</param>
        public CommandResult(CommandType commandType, string line)
        {
            /*
             * +OK\r\n 成功
             * -ERR 出错  -开头的为错误
             * $1\r\n数据\r\n 单条数据
             * *2\r\n 多条数据
             * :2\r\n 整型数字
             */
            this.Line = line;
            this.CommandType = commandType;
            var cmdTypeName = this.CommandType.ToString();
            var msg = line;
            if (msg.IsMatch(@"^-"))
            {
                this.Message = line;
                this.OK = false;
                return;
            }
            if (cmdTypeName == "PING" && msg.StartsWith("+PONG"))
            {
                this.Value = "PONG";
                this.OK = true;
                return;
            }
            this.OK = true;
            if (msg.IsMatch(@"^\$-\d+\r\n"))
            {
                this.Value = String.Empty;
                return;
            }
            if (msg.IsMatch(@"^:\d+\r\n$"))
            {
                this.Value = msg.Trim(new char[] { '\r', '\n', ':' }).ToCast<int>();
                return;
            }
            if (msg.IsMatch(@"^\$\d+\r\n"))
            {
                var _val = msg;
                switch (cmdTypeName)
                {
                    case "CLIENT":
                        if (_val.IndexOf(" qbuf-free=") > -1)/*LIST*/
                        {
                            var list = new List<ClientInfo>();
                            _val = _val.RemovePattern(@"^\$\d+\r\n").RemovePattern(@"\r\n$");
                            _val.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Each(l =>
                            {
                                list.Add(l.Replace(" qbuf-free=", "qbuffree=").ReplacePattern(@"\s", "&").JsonToObject<ClientInfo>());
                            });
                            this.Value = list;
                        }
                        break;
                    default:
                        if (msg.IsMatch(@"^\$(?<b>\d+)\r\n(?<a>[\s\S]*)\r\n"))
                        {
                            this.Value = msg.GetMatch(@"^\$(?<b>\d+)\r\n(?<a>[\s\S]*)\r\n$");
                        }
                        else this.Message = "网络不稳定";
                        break;
                }
                return;
            }
            if (msg.IsMatch(@"^\*\d+\r\n"))
            {
                var _val = msg;
                switch (cmdTypeName)
                {
                    case "ROLE":
                        this.Value = msg.GetMatch(@"\r\n(?<a>(master|slave|sentinel))\r\n:");
                        break;
                    case "GEOPOS":
                        var geoList = new List<GeoModel>();
                        _val = _val.RemovePattern(@"^\*\d+\r\n");
                        _val.GetMatches(@"\*(?<a>-?\d+)\r\n(\$\d+\r\n(?<b>[^\r\n]*)\r\n\$\d+\r\n(?<c>[^\r\n]*)\r\n)?").Each(g =>
                        {
                            var a = g["a"];
                            geoList.Add(a.StartsWith("-") ? null : new GeoModel
                            {
                                Longitude = g["b"].ToCast<double>(),
                                Latitude = g["c"].ToCast<double>()
                            });
                        });
                        this.Value = geoList;
                        break;
                    case "GEORADIUS":
                    case "GEORADIUSBYMEMBER":
                        var geoRadius = new List<GeoRadiusModel>();
                        _val = _val.RemovePattern(@"^\*\r+\r\n");
                        _val.GetMatches(@"\*\d+\r\n\$\d+\r\n(?<address>[^\r\n]+)\r\n((\$\d+\r\n(?<dist>[^\r\n]*)\r\n)?(\:(?<hash>[^\r\n]*)\r\n)?)?\*\d+\r\n\$\d+\r\n(?<long>[^\r\n]+)\r\n\$\d+\r\n(?<lat>[^\r\n]+)\r\n").Each(g =>
                        {
                            geoRadius.Add(new GeoRadiusModel
                            {
                                Address = g["address"],
                                Hash = g["hash"] ?? String.Empty,
                                Dist = g["dist"].ToCast<double>(),
                                Longitude = g["long"].ToCast<double>(),
                                Latitude = g["lat"].ToCast<double>()
                            });
                        });
                        this.Value = geoRadius;
                        break;
                    case "GEOHASH":
                    case "MGET":
                    case "SCAN":
                    case "KEYS":
                    case "SORT":
                    case "HKEYS":
                    case "HMGET":
                    case "HVALS":
                    case "LRANGE":
                    case "SPOP":
                    case "SMEMBERS":
                    case "SINTER":
                    case "SDIFF":
                    case "SRANDMEMBER":
                    case "SUNION":
                    case "SSCAN":
                    case "ZRANGE":
                    case "ZREVRANGE":
                    case "ZRANGEBYLEX":
                    case "ZRANGEBYSCORE":
                    case "ZREVRANGEBYSCORE":
                        if (",SCAN,SSCAN,".IndexOf("," + cmdTypeName + ",") > -1) _val = _val.RemovePattern(@"^\*\d+\r\n\$\d+\r\n\d+\r\n\*\d+\r\n");
                        var list = new List<string>();
                        _val = _val.RemovePattern(@"^\*\d+\r\n");
                        //_val.GetMatches(@"\$(?<a>-?\d+)\r\n(?<b>[^\r\n]*)\r\n").Each(a => list.Add(a["a"].StartsWith("-") ? string.Empty : a["b"]));
                        this.GetString(_val, ref list);
                        this.Value = list;
                        break;
                    case "HGETALL":
                    case "HSCAN":
                    case "BLPOP":
                    case "BRPOP":
                    case "ZSCAN":
                    case "CONFIG":
                        if (",HSCAN,ZSCAN,".IndexOf("," + cmdTypeName + ",") > -1) _val = _val.RemovePattern(@"^\*\d+\r\n\$\d+\r\n\d+\r\n\*\d+\r\n");
                        else if (",CONFIG,HGETALL,".IndexOf("," + cmdTypeName + ",") > -1)
                            _val = _val.RemovePattern(@"^\*\d+\r\n");
                        var dict = new Dictionary<string, string>();
                        //_val.GetMatches(@"\$(?<a>-?\d+)\r\n(?<b>[a-z0-9_-]*)\r\n\$(?<c>-?\d+)\r\n(?<d>[\s\S]*?)").Each(a => dict.Add(a["b"], a["d"]));
                        this.GetString(_val, ref dict);
                        this.Value = dict;
                        break;
                }
            }
            this.Message = msg;
            if (this.CommandType == CommandType.LINSERT)
            {
                var val = msg.GetMatch(@"^:(?<a>-?\d+)\r\n").ToCast<int>();
                this.OK = val > 0;
                if (val == -1)
                    this.Message = "没有找到指定元素";
                else if (val == 0)
                    this.Message = " key 不存在或为空列表";
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 命令
        /// </summary>
        public string Line { get; set; }
        /// <summary>
        /// 命令
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public Boolean OK { get; set; } = false;
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 设置结果行
        /// </summary>
        /// <param name="commandType">命令</param>
        /// <param name="bytes">结果集字节</param>
        public CommandResult(CommandType commandType, byte[] bytes) : this(commandType, bytes.GetString()) { }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="str">结果</param>
        /// <param name="dict">数据</param>
        private void GetString(string str, ref Dictionary<string, string> dict)
        {
            if (str.Length == 0) return;
            var KeyLength = str.GetMatch(@"^\$(?<a>\d+)\r\n").ToCast<int>(-1);
            if (KeyLength == -1) return;
            str = str.RemovePattern(@"^\$(?<a>\d+)\r\n");
            var key = str.GetBytes().GetString(Encoding.UTF8, 0, KeyLength);
            str = str.RemovePattern(@"^" + key.ToRegexEscape() + @"\r\n");
            var lastLength = str.GetMatch(@"^\$(?<a>\d+)\r\n").ToCast<int>();
            str = str.RemovePattern(@"^\$(?<a>\d+)\r\n");
            if (lastLength == 0)
            {
                str = str.RemovePattern(@"^\r\n");
                dict.Add(key, "");
            }
            else
            {
                var value = str.GetBytes().GetString(Encoding.UTF8, 0, lastLength);
                str = str.RemovePattern(@"^" + value.ToRegexEscape() + @"\r\n");
                dict.Add(key, value);
            }
            GetString(str, ref dict);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="str">结果</param>
        /// <param name="list">数据</param>
        private void GetString(string str, ref List<string> list)
        {
            if (str.Length == 0) return;
            var KeyLength = str.GetMatch(@"^\$(?<a>\d+)\r\n").ToCast<int>(-1);
            if (KeyLength == -1) return;
            str = str.RemovePattern(@"^\$(?<a>\d+)\r\n"); 
            if (KeyLength == 0)
            {
                str = str.RemovePattern(@"^\r\n");
                list.Add("");
            }
            else
            {
                var key = str.GetBytes().GetString(Encoding.UTF8, 0, KeyLength);
                str = str.RemovePattern(@"^" + key.ToRegexEscape() + @"\r\n");
                list.Add(key);
            }
            GetString(str, ref list);
        }
        #endregion
    }
}