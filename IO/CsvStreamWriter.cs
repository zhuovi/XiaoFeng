using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-09-09 09:13:25                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// CSVStreamWriter 类说明
    /// </summary>
    public class CsvStreamWriter:Disposable
    {
        #region 构造器
        /// <summary>
        /// 设置流
        /// </summary>
        /// <param name="stream">文件流</param>
        public CsvStreamWriter(Stream stream)
        {
            this.Writer = new StreamWriter(stream);
        }
        /// <summary>
        /// 设置文件路径
        /// </summary>
        /// <param name="path">文件路径</param>
        public CsvStreamWriter(string path)
        {
            this.Writer = new StreamWriter(path);
        }
        /// <summary>
        /// 设置流
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="encoding">编码</param>
        public CsvStreamWriter(Stream stream,Encoding encoding)
        {
            this.Writer = new StreamWriter(stream, encoding);
            this.Encoding = encoding;
        }
        /// <summary>
        /// 设置文件路径
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="append">是否附加</param>
        /// <param name="encoding">编码</param>
        public CsvStreamWriter(string path,Boolean append,Encoding encoding)
        {
            this.Writer = new StreamWriter(path, append, encoding);
            this.Encoding = encoding;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 文件编码
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 写入器
        /// </summary>
        private StreamWriter Writer { get; set; }
        /// <summary>
        /// 基础流
        /// </summary>
        private Stream BaseStream => this.Writer.BaseStream;
        #endregion

        #region 方法
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="list">数据列表</param>
        public void Write(List<List<string>> list)
        {
            list.Each(line =>
            {
                var vals = new List<string>();
                line.Each(a =>
                {
                    if (a.StartsWith("\"") || a.IndexOf(",") > -1 || a.IndexOf("\r\n") > -1)
                        a = "\"" + a.Replace("\"", "\"\"") + "\"";
                    vals.Add(a);
                });
                var bytes = vals.Join(",").GetBytes();
                this.BaseStream.Write(bytes, 0, bytes.Length);
                this.BaseStream.WriteByte((byte)'\r');
                this.BaseStream.WriteByte((byte)'\n');
                this.BaseStream.Flush();
            });
        }
        /// <summary>
        /// 关闭流
        /// </summary>
        public void Close()
        {
            this.Writer.Close();
        }
        /// <summary>
        /// 释放流
        /// </summary>
        /// <param name="disposing">状态</param>
        protected override void Dispose(bool disposing)
        {
            this.Writer.Dispose();
            base.Dispose(disposing);
        }
        /// <summary>
        /// 析构
        /// </summary>
        ~CsvStreamWriter()
        {
            this.Dispose(true);
            GC.Collect();
        }
        #endregion
    }
}