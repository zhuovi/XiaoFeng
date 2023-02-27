using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-07 15:41:46                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// 读取器
    /// </summary>
    public class MemcachedReader : IMemcachedReader
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public MemcachedReader() { }
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="stream">网络流</param>
        /// <param name="args">参数</param>
        public MemcachedReader(CommandType commandType, Stream stream, object[] args = null)
        {
            this.CommandType = commandType;
            this.Arguments = args;
            this.Reader = stream;
            this.GetValue();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 参数
        /// </summary>
        public object[] Arguments { get; set; }
        /// <summary>
        /// 锁
        /// </summary>
        public object StreamLock { get; set; } = new object();
        /// <summary>
        /// 换行
        /// </summary>
        private const string EOF = "\r\n";
        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// 流
        /// </summary>
        public Stream Reader { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public List<MemcachedValue> Value { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        public Dictionary<string, string> DictonaryValue { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public Boolean OK { get; set; }
        #endregion

        #region 方法

        #region 读取一行数据
        /// <summary>
        /// 读取一行数据
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            MemoryStream buffer = new MemoryStream();
            int c;
            bool Return = false;
            var reader = this.Reader as System.Net.Sockets.NetworkStream;
            while ((c = Reader.ReadByte()) != -1)
            {
                if (Return)
                {
                    if (c == 10)
                        break;
                    else
                    {
                        buffer.WriteByte(13);
                        Return = false;
                    }
                }
                if (c == 13)
                    Return = true;
                else
                    buffer.WriteByte((byte)c);
            }
            return buffer.ToArray().GetString(Encoding.UTF8);
        }
        #endregion

        #region 读取响应
        /// <summary>
        /// 读取响应
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MemcachedException">异常</exception>
        public string ReadResponse()
        {
            var response = this.ReadLine();

            if (response.IsNullOrEmpty())
                throw new MemcachedException("空的响应.");
            return response;
        }
        #endregion

        #region 获取值
        /// <summary>
        /// 获取值
        /// </summary>
        public void GetValue()
        {
            if (this.CommandType.Flags == CommandFlags.Store)
            {
                var result = ReadResponse();
                this.OK = result.StartsWith(ReturnResult.STORED.GetEnumName(),StringComparison.OrdinalIgnoreCase);
            }
            else if (this.CommandType.Flags == CommandFlags.Get)
            {
                var line = this.ReadLine();
                
                if(line.StartsWith(ReturnResult.END.GetEnumName(), StringComparison.OrdinalIgnoreCase))
                {
                    this.OK = true;
                    this.Value = null;
                    return;
                }
                if(line.StartsWith(ReturnResult.DELETED.GetEnumName(), StringComparison.OrdinalIgnoreCase)||
                    line.StartsWith(ReturnResult.TOUCHED.GetEnumName(), StringComparison.OrdinalIgnoreCase))
                {
                    this.OK = true;
                    return;
                }
                if (line.StartsWith(ReturnResult.ERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith(ReturnResult.CLIENTERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith(ReturnResult.NOT_FOUND.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith(ReturnResult.SERVERERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase))
                    return;
                if (this.CommandType == CommandType.INCREMENT || this.CommandType == CommandType.DECREMENT)
                {
                    if (line.IsNumberic())
                    {
                        this.OK = true;
                        if (this.Value == null) this.Value = new List<MemcachedValue>();
                        this.Value.Add(new MemcachedValue("", ValueType.ULong, line.ToCast<ulong>(), 0));
                    }
                    return;
                }
                while (line.StartsWith("VALUE", StringComparison.OrdinalIgnoreCase) && !line.EqualsIgnoreCase(ReturnResult.END.GetEnumName()))
                {
                    var value = new MemcachedValue();
                    var args = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (args[0].EqualsIgnoreCase("VALUE"))
                    {
                        this.OK = true;
                        if (args.Length > 4)
                            value.Unique = args[4].ToCast<ulong>();
                        value.Key = args[1];
                        value.Type = args[2].ToEnum<ValueType>();
                        var length = args[3].ToCast<int>();
                        var bytes = new byte[length];
                        this.Reader.Read(bytes, 0, bytes.Length);
                        value.Value = Helper.Deserialize(bytes, value.Type);
                        this.ReadLine();
                        if (this.Value == null) this.Value = new List<MemcachedValue>();
                        this.Value.Add(value);
                    }
                    line = this.ReadLine();
                }
            }
            else if(this.CommandType.Flags == CommandFlags.Stats)
            {
                var line = this.ReadLine();
                if (line.StartsWith(ReturnResult.ERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith(ReturnResult.CLIENTERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith(ReturnResult.NOT_FOUND.GetEnumName(), StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith(ReturnResult.SERVERERROR.GetEnumName(), StringComparison.OrdinalIgnoreCase))
                    return;
                this.OK = true;
                while (line.StartsWith("STAT", StringComparison.OrdinalIgnoreCase) && !line.EqualsIgnoreCase(ReturnResult.END.GetEnumName()))
                {
                    if (this.DictonaryValue == null) this.DictonaryValue = new Dictionary<string, string>();
                    var args = line.RemovePattern(@"^STAT\s+(Items:\d+:|\d+:)?").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (args.Length == 1)
                    {
                        if (!this.DictonaryValue.ContainsKey(args[0]))
                            this.DictonaryValue.Add(args[0], string.Empty);
                    }
                    else if (args.Length == 2)
                    {
                        if (this.CommandType == CommandType.STATSSIZES)
                        {
                            this.DictonaryValue.Add("Item Size", args[0]);
                            this.DictonaryValue.Add("Item Count", args[1]);
                        }
                        else
                        {
                            if (!this.DictonaryValue.ContainsKey(args[0]))
                                this.DictonaryValue.Add(args[0], args[1]);
                        }
                    }
                    line = this.ReadLine();
                }
            }
            
        }
        #endregion
        #endregion
    }
}