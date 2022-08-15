using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-08-12 16:33:36                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 数据读
    /// </summary>
    public class RedisReader
    {
        /*
        * +OK\r\n 成功
        * -ERR 出错  -开头的为错误
        * $1\r\n数据\r\n 单条数据
        * *2\r\n 多条数据
        * :2\r\n 整型数字
        */
        #region 构造器
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="result">数据</param>
        public RedisReader(CommandType commandType, byte[] result)
        {
            this.CommandType = commandType;
            this.Data = result;
            this.Reader = new MemoryStream(result);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 换行
        /// </summary>
        private const string EOF = "\r\n";
        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// 流
        /// </summary>
        public Stream Reader { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 结果类型
        /// </summary>
        public ResultType Status { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            var type = this.ReadType();
            this.Status = type;
            switch (type)
            {
                case ResultType.Status:
                    return this.Value = this.ReadStatus().EqualsIgnoreCase("OK");
                case ResultType.Error:
                    return this.Value = this.ReadError();
                case ResultType.Int:
                    return this.Value = this.ReadInt();
                case ResultType.Bulk:
                    return this.Value = this.ReadBulkString();
                case ResultType.MultiBulk:
                    return this.ReadMultiBulkString();
            }
            return null;
        }
        /// <summary>
        /// 读取类型
        /// +OK\r\n 成功
        /// -ERR 出错  -开头的为错误
        /// $1\r\n数据\r\n 单条数据
        /// *2\r\n 多条数据
        /// :2\r\n 整型数字
        /// </summary>
        /// <returns>类型</returns>
        public ResultType ReadType() => this.Reader.ReadByte().ToEnum<ResultType>();
        /// <summary>
        /// 读取状态
        /// +OK\r\n 成功
        /// </summary>
        /// <returns>状态</returns>
        public String ReadStatus() => this.ReadLine();
        /// <summary>
        /// 读取错误信息
        /// -ERR 出错  -开头的为错误
        /// </summary>
        /// <returns>错误信息</returns>
        public String ReadError() => this.ReadLine().RemovePattern(@"ERR\s+");
        /// <summary>
        /// 读取整型
        /// :2\r\n 整型数字
        /// </summary>
        /// <returns>整型数</returns>
        public long ReadInt() => this.ReadLine().ToCast<long>();
        /// <summary>
        /// 是否结尾
        /// </summary>
        /// <exception cref="Exception">抛出异常</exception>
        public void ReadEOF()
        {
            var r = this.Reader.ReadByte();
            var n = this.Reader.ReadByte();
            if (r == -1 && n == -1) return;
            if (r != 13 || n != 10)
                throw new Exception($"结尾字符不是\r\n而是{(char)r}{(char)n}.");
        }
        /// <summary>
        /// 读取单条数据
        /// $1\r\n数据\r\n 单条数据
        /// </summary>
        /// <returns>单条数据</returns>
        public string ReadBulkString() => this.ReadBulkBytes().GetString();
        /// <summary>
        /// 读取单条数据
        /// $1\r\n数据\r\n 单条数据
        /// </summary>
        /// <returns>单条数据</returns>
        public byte[] ReadBulkBytes()
        {
            var size = this.ReadLine().ToCast<int>();
            var vals = this.ReadLineBytes();
            this.Value = vals;
            return vals;
        }
        /// <summary>
        /// 读取多行数据
        /// *2\r\n 多条数据
        /// </summary>
        /// <returns>数据</returns>
        public List<String> ReadMultiBulkString() => (from a in this.ReadMultiBulkBytes() select a.GetString()).ToList();
        /// <summary>
        /// 读取多行数据
        /// *2\r\n 多条数据
        /// </summary>
        /// <returns>数据</returns>
        public List<byte[]> ReadMultiBulkBytes()
        {
            var size = this.ReadInt().ToCast<int>();
            if (size <= -1) return null;
            var list = new List<byte[]>();
            for (int i = 0; i < size; i++)
                list.Add(this.ReadBulkBytes());
            this.Value = list;
            return list;
        }
        /// <summary>
        /// 读取内容
        /// </summary>
        /// <returns>一行数据</returns>
        public string ReadLine() => this.ReadLineBytes().GetString();
        /// <summary>
        /// 读取内容
        /// </summary>
        /// <returns>一行数据</returns>
        public byte[] ReadLineBytes()
        {
            var should_break = false;
            var i = 0;
            var length = this.Reader.Length;
            var list = new List<byte>();
            while (i <= length)
            {
                var b = this.Reader.ReadByte();
                var c = (char)b;
                if (c == '\r')
                    should_break = true;
                else if (c == '\n' && should_break)
                    break;
                else
                {
                    list.Add((byte)b);
                    should_break = false;
                }
                i++;
            }
            return list.ToArray();
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">错误</exception>
        //public (ResultType, object) Reada()
        //{
        //    ResultType type = this.ReadType();
        //    object vals;
        //    switch (type)
        //    {
        //        case ResultType.Bulk:
        //            vals= this.ReadBulkBytes();
        //            break;
        //        case ResultType.Int:
        //            vals = this.ReadInt();
        //            break;
        //        case ResultType.MultiBulk:
        //            vals = this.ReadMultiBulkBytes();
        //            break;
        //        case ResultType.Status:
        //        case ResultType.Error:
        //            vals = this.ReadStatus();
        //            break;
        //        default:
        //            throw new Exception("no support response type: " + type);
        //    }
        //    return ValueTuple.Create(type, vals);
        //}
        public long ToLong()
        {
            //var vals = this.Reada();
            //if (vals.Item1 == ResultType.Int) return (long)vals.Item2;
            return -1;
        }
        
        #endregion
    }
}