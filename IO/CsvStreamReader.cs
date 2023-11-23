using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-09-08 16:17:58                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// CSV读流器
    /// </summary>
    public class CsvStreamReader : Disposable
    {
        #region 构造器
        /// <summary>
        /// 设置文件流
        /// </summary>
        /// <param name="stream">文件流</param>
        public CsvStreamReader(Stream stream)
        {
            this.Reader = new StreamReader(stream);
        }
        /// <summary>
        /// 设置文件路径
        /// </summary>
        /// <param name="path">文件路径</param>
        public CsvStreamReader(string path)
        {
            this.Reader = new StreamReader(new MemoryStream(FileHelper.OpenBytes(path)));
        }
        /// <summary>
        /// 设置文件路径
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码</param>
        public CsvStreamReader(string path, Encoding encoding)
        {
            this.Reader = new StreamReader(path, encoding);
            this.Encoding = encoding;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 文件流
        /// </summary>
        public StreamReader Reader { get; set; }
        /// <summary>
        /// 基础流
        /// </summary>
        public Stream BaseStream => this.Reader.BaseStream;
        /// <summary>
        /// 列分隔符
        /// </summary>
        public Char Separator { get; set; } = ',';
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        #endregion

        #region 方法
        /// <summary>
        /// 是否结束
        /// </summary>
        public Boolean EndOfStream { get => this.BaseStream.Position == this.BaseStream.Length; }
        /// <summary>
        /// 读取一行
        /// </summary>
        /// <returns></returns>
        public List<string> ReadLine()
        {
            var list = new List<string>();
            var bytes = new List<byte>();
            var nextline = false;
            var ClosureQuotes = false;
            var FirstQuotes = false;
            var stream = this.BaseStream;
            while (stream.Position < stream.Length)
            {
                var b = stream.ReadByte();
                var c = (char)b;
                if (c == '\r')
                    nextline = true;
                else if (c == '\n' && nextline)
                {
                    /*一行结束*/
                    if (!ClosureQuotes)
                    {
                        if (bytes.Count > 0 && bytes.Last() == '"')
                            bytes.RemoveAt(bytes.Count - 1);
                        list.Add(bytes.ToArray().GetString(this.Encoding));
                        break;
                    }
                    else
                    {
                        bytes.Add((byte)'\r');
                        bytes.Add((byte)'\n');
                    }
                    nextline = false;
                }
                else
                {
                    if (c == '"')
                    {
                        if (bytes.Count > 0 || FirstQuotes)
                            bytes.Add((byte)b);
                        else
                        {
                            FirstQuotes = true;
                        }
                        if (FirstQuotes) ClosureQuotes = !ClosureQuotes;
                    }
                    else if (c == ',')
                    {
                        if (FirstQuotes)
                        {
                            if (!ClosureQuotes)
                            {
                                if (bytes.Count > 0 && bytes.Last() == '"')
                                    bytes.RemoveAt(bytes.Count - 1);
                                list.Add(bytes.ToArray().GetString(this.Encoding).ReplacePattern(@"""{2}", @""""));
                                bytes = new List<byte>();
                                FirstQuotes = false;
                            }
                            else
                            {
                                bytes.Add((byte)b);
                            }
                        }
                        else
                        {
                            list.Add(bytes.ToArray().GetString(this.Encoding));
                            bytes = new List<byte>();
                        }
                    }
                    else
                    {
                        bytes.Add((byte)b);
                    }
                    nextline = false;
                }
            }
            return list;
        }
        /// <summary>
        /// 读完
        /// </summary>
        /// <returns></returns>
        public string ReadToEnd() => this.Reader.ReadToEnd();
        /// <summary>
        /// 读取所有行
        /// </summary>
        /// <returns></returns>
        public List<List<string>> ReadLines()
        {
            var list = new List<List<string>>();
            while (!this.EndOfStream)
            {
                var line = this.ReadLine();
                list.Add(line);
            }
            return list;
        }
        /// <summary>
        /// 读取一行数据
        /// </summary>
        /// <returns></returns>
        public DataRow ReadRow()
        {
            var dt = new DataTable("csv");
            var dr = dt.NewRow();
            var line = this.ReadLine();
            var index = 0;
            line.Each(a =>
            {
                dt.Columns.Add($"csv--column-{index}");
                dr[index] = a;
                index++;
            });
            return dr;
        }
        /// <summary>
        /// 读取数据到DataTable
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public DataTable ReadTable(string tableName = "csv")
        {
            var dt = new DataTable(tableName);
            while (!this.EndOfStream)
            {
                var line = this.ReadLine();
                for (var i = dt.Columns.Count; i < line.Count; i++)
                {
                    dt.Columns.Add($"csv-column-{i}");
                }
                var dr = dt.NewRow();
                var index = 0;
                line.Each(a =>
                {
                    dr[index++] = a;
                });
                dt.Rows.Add(dr);
            }
            return dt;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            this.Reader.Close();
        }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">状态</param>
        protected override void Dispose(bool disposing)
        {
            this.Reader.Dispose();
            base.Dispose(disposing);
        }
        /// <summary>
        /// 析构
        /// </summary>
        ~CsvStreamReader()
        {
            this.Dispose(true);
            GC.Collect();
        }
        #endregion
    }
}