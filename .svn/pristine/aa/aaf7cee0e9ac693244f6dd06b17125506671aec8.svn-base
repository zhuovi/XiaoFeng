using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace XiaoFeng.IO
{
    /// <summary>
    /// CSV文件操作
    /// </summary>
    public class CsvFile : Disposable
    {
        #region 属性
        /// <summary>文件编码 默认为UTF-8</summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 数据流
        /// </summary>
        private readonly Stream _stream;
        /// <summary>分隔符 默认逗号</summary>
        public Char Separator { get; set; } = ',';
        #endregion

        #region 构造
        /// <summary>数据流实例化</summary>
        /// <param name="stream"></param>
        public CsvFile(Stream stream) => _stream = stream;

        /// <summary>Csv文件实例化</summary>
        /// <param name="file">文件路径</param>
        /// <param name="write">是否可写</param>
        public CsvFile(string file, bool write = false)
        {
            if (write)
                _stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            else
                _stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _reader.TryDispose();

            _writer?.Flush();

            if (_stream is FileStream fs)
            {
                _writer.TryDispose();
                fs.TryDispose();
            }
        }
        #endregion

        #region 读取
        /// <summary>读取一行</summary>
        /// <returns></returns>
        public string[] ReadLine()
        {
            EnsureReader();
            var line = _reader.ReadLine();
            if (line == null) return null;
            var list = new List<string>();
            // 直接分解，引号合并
            var arr = line.Split(Separator);
            for (var i = 0; i < arr.Length; i++)
            {
                var str = (arr[i] + "").Trim();
                if (str.StartsWith("\""))
                {
                    var txt = "";
                    if (str.EndsWith("\"") && !str.EndsWith("\"\""))
                        txt = str.Trim('\"');
                    else
                    {
                        // 找到下一个以引号结尾的项
                        for (var j = i + 1; j < arr.Length; j++)
                        {
                            if (arr[j].EndsWith("\""))
                            {
                                txt = arr.Skip(i).Take(j - i + 1).Join(Separator + "").Trim('\"');

                                // 跳过去一大步
                                i = j;
                                break;
                            }
                        }
                    }
                    // 两个引号是一个引号的转义
                    txt = txt.Replace("\"\"", "\"");
                    list.Add(txt);
                }
                else
                    list.Add(str);
            }
            return list.ToArray();
        }
        /// <summary>读取所有行</summary>
        /// <returns></returns>
        public string[][] ReadAll()
        {
            var list = new List<string[]>();
            while (true)
            {
                var line = ReadLine();
                if (line == null) break;
                list.Add(line);
            }
            return list.ToArray();
        }
        /// <summary>
        /// 读取流
        /// </summary>
        private StreamReader _reader;
        /// <summary>
        /// 确认读取流可用
        /// </summary>
        private void EnsureReader()
        {
            if (_reader == null) _reader = new StreamReader(_stream, Encoding);
        }
        #endregion

        #region 写入
        /// <summary>写入全部</summary>
        /// <param name="data">数据</param>
        public void WriteAll(IEnumerable<IEnumerable<object>> data)
        {
            data.Each(line => WriteLine(line));
        }
        /// <summary>写入一行</summary>
        /// <param name="line">行数据</param>
        public void WriteLine(IEnumerable<object> line)
        {
            EnsureWriter();
            var sb = new StringBuilder();
            line.Each(item =>
            {
                if (sb.Length > 0) sb.Append(Separator);
                if (!(item is String str)) str = "{0}".format(item);
                if (str.Contains("\""))
                    sb.AppendFormat("\"{0}\"", str.Replace("\"", "\"\""));
                else if (str.Contains(Separator) || str.Contains("\r") || str.Contains("\n"))
                    sb.AppendFormat("\"{0}\"", str);
                else
                    sb.Append(str);
            });
            _writer.WriteLine(sb);
        }
        /// <summary>
        /// 写入流
        /// </summary>
        private StreamWriter _writer;
        /// <summary>
        /// 确认写入流可用
        /// </summary>
        private void EnsureWriter()
        {
            if (_writer == null) _writer = new StreamWriter(_stream, Encoding);
        }
        #endregion
    }
}