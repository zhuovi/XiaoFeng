using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/****************************************************************
*  Copyright © (2024) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2024-12-20 11:09:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// ELF 文件
    /// </summary>
    public class ElfFile : Disposable
    {
        #region 构造器
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public ElfFile(string filePath) : this(filePath, null, string.Empty) { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="encoding">编码</param>
        public ElfFile(string filePath, Encoding encoding) : this(filePath, encoding, string.Empty) { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="key">密钥</param>
        public ElfFile(string filePath, string key) : this(filePath, null, key) { }
        /// <summary>
        /// 初始化一个新实例
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="encoding">编码</param>
        /// <param name="key">密钥</param>
        public ElfFile(string filePath, Encoding encoding, string key)
        {
            this.FilePath = filePath;
            this.Key = key;
            if (encoding != null) this.Encoding = encoding;
            this.Stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 8192, true);
            this.Data = new MemoryStream();
            if (this.Stream.Length > 0)
            {
                if (this.Stream.Length >= 10)
                {
                    var HeaderBytes = new byte[10];
                    this.Stream.Read(HeaderBytes, 0, HeaderBytes.Length);
                    var header = HeaderBytes.ToHexString(false);
                    if (header.Substring(0,12) == "454C46580D0A" && header.Substring(14, 6) == "0D0A1A")
                    {
                        this.IsEncrypt = header.Substring(12, 2) != "00";
                        var _bytes = new byte[this.Stream.Length - 10];
                        this.Stream.Read(_bytes, 0, _bytes.Length);
                        var length = _bytes.Length;
                        if (_bytes[_bytes.Length-1] == 10 && _bytes[_bytes.Length - 2] == 13) length -= 2;

                        this.Data.Write(_bytes, 0, length);
                        this.Data.Seek(0, SeekOrigin.Begin);
                        var cms = new MemoryStream();
                        var ms = new MemoryStream(this.Data.ToArray());
                        using (var zip = new GZipStream(ms, CompressionMode.Decompress, true))
                        {
                            zip.CopyTo(cms);
                        }
                        this.Data.SetLength(0);
                        this.Data.Write(cms.ToArray(), 0, (int)cms.Length);
                        cms.Close();
                        cms.Dispose();
                        if (this.IsEncrypt) this.Decrypt(this.Key);
                        return;
                    }
                }

                var bytes = new byte[this.Stream.Length];
                this.Stream.Read(bytes, 0, bytes.Length);

                this.Data.Write(bytes, 0, bytes.Length);
                this.Data.Seek(0, SeekOrigin.Begin);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 内容类型
        /// </summary>
        public const string ContentType = "application/elf";
        /// <summary>
        /// 后缀名
        /// </summary>
        public const string Extension = ".elfx";
        /// <summary>
        /// 文件头 XELF\r\n00\r\n(SUB) 00未加密 01加密
        /// </summary>
        private const string HeaderX = "454C46580D0A__0D0A1A";
        /// <summary>
        /// 文件尾 \r\n
        /// </summary>
        private const string FooterX = "0D0A";
        /// <summary>
        /// 文件路径
        /// </summary>
        private string FilePath { get; set; }
        /// <summary>
        /// 是否压缩
        /// </summary>
        public Boolean IsEncrypt { get; private set; } = false;
        /// <summary>
        /// 文件编码
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 文件流
        /// </summary>
        private FileStream Stream { get; set; }
        /// <summary>
        /// 缓存流
        /// </summary>
        private MemoryStream Data { get; set; }
        /// <summary>
        /// 获取流长度（以字节为单位）
        /// </summary>
        public long Length => this.Data.Length;
        /// <summary>
        /// 获取或设置当前流中的位置。
        /// </summary>
        public long Position
        {
            get => this.Data.Position;
            set => this.Data.Position = value;
        }
        /// <summary>
        /// 是否是流结尾
        /// </summary>
        public Boolean EndOfStream => this.Length == this.Position;
        /// <summary>
        /// 密钥
        /// </summary>
        public string Key { get; set; }
        #endregion

        #region 方法

        #region 将字节序列写入当前流，并将流的当前位置提升写入的字节数。
        /// <summary>
        /// 将字节序列写入当前流，并将流的当前位置提升写入的字节数。
        /// </summary>
        /// <param name="buffer">从中写入数据的内存区域</param>
        /// <param name="offset"><see langword="buffer"/> 中的从零开始的字节偏移量，从此处开始将字节复制到该流。</param>
        /// <param name="count">最多写入的字节数。</param>
        /// <returns></returns>
        public void Write(byte[] buffer, int offset, int count)
        {
            this.Data.Write(buffer, offset, count);
        }
        /// <summary>
        /// 将字节序列异步写入当前流，并将流的当前位置提升写入的字节数。
        /// </summary>
        /// <param name="buffer">从中写入数据的内存区域</param>
        /// <param name="offset"><see langword="buffer"/> 中的从零开始的字节偏移量，从此处开始将字节复制到该流。</param>
        /// <param name="count">最多写入的字节数。</param>
        /// <returns></returns>
        public async Task WriteAsync(byte[] buffer, int offset, int count)
        {
            await this.Data.WriteAsync(buffer, offset, count).ConfigureAwait(false);
        }
        /// <summary>
        /// 将字节序列写入当前流，并将流的当前位置提升写入的字节数。
        /// </summary>
        /// <param name="buffer">从中写入数据的内存区域</param>
        public void Write(byte[] buffer)
        {
            this.Write(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// 将字节序列异步写入当前流，并将流的当前位置提升写入的字节数。
        /// </summary>
        /// <param name="buffer">从中写入数据的内存区域</param>
        public async Task WriteAsync(byte[] buffer) => await this.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        #endregion

        #region 将一个字节写入流内的当前位置，并将流内的位置向前提升一个字节。
        /// <summary>
        ///  将一个字节写入流内的当前位置，并将流内的位置向前提升一个字节。
        /// </summary>
        /// <param name="value">要写入流中的字节。</param>
        public void WriteByte(byte value)
        {
            this.Data.WriteByte(value);
        }
        /// <summary>
        ///  将一个字节异步写入流内的当前位置，并将流内的位置向前提升一个字节。
        /// </summary>
        /// <param name="value">要写入流中的字节。</param>
        public async Task WriteByteAsync(byte value)
        {
            await Task.Run(() => this.Data.WriteByte(value)).ConfigureAwait(false);
        }
        #endregion

        #region 将字符串写入当前流，并将流的当前位置提升写入的字节数。
        /// <summary>
        /// 将字符串写入当前流，并将流的当前位置提升写入的字节数。
        /// </summary>
        /// <param name="value">字符串</param>
        public void Write(string value) => this.Write(value.GetBytes(this.Encoding));
        /// <summary>
        /// 将字符串写入当前流，并将流的当前位置提升写入的字节数。
        /// </summary>
        /// <param name="value">字符串</param>
        public async Task WriteAsync(string value) => await this.WriteAsync(value.GetBytes(this.Encoding)).ConfigureAwait(false);
        #endregion

        #region 读取所有数据内容
        /// <summary>
        /// 读取所有数据内容
        /// </summary>
        /// <returns></returns>
        public string ReadToEnd()
        {
            return this.Data.ToArray().GetString(this.Encoding);
        }
        #endregion

        #region 读取一行数据
        /// <summary>
        /// 读取一行数据
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            if (this.EndOfStream) return string.Empty;
            var data = new List<byte>();
            var r = false;
            while (!this.EndOfStream)
            {
                var a = this.ReadByte();
                if (a == 13)
                {
                    if (!r)
                        r = true;
                    else
                        data.Add((byte)13);
                    continue;
                }
                if (a == 10)
                {
                    if (r) break;
                    else
                        data.Add((byte)10);
                    continue;
                }
                data.Add((byte)a);
            }
            return data.ToArray().GetString(this.Encoding);
        }
        #endregion

        #region 从流中读取一个字节，并将流内的位置向前提升一个字节，或者如果已到达流结尾，则返回 -1。
        /// <summary>
        /// 从流中读取一个字节，并将流内的位置向前提升一个字节，或者如果已到达流结尾，则返回 -1。
        /// </summary>
        /// <returns></returns>
        public int ReadByte()
        {
            return this.Data.ReadByte();
        }
        #endregion

        #region 从当前流读取字节序列，并将流中的位置提升读取的字节数。
        /// <summary>
        /// 从当前流读取字节序列，并将流中的位置提升读取的字节数。
        /// </summary>
        /// <param name="buffer">要将数据写入的内存区域。</param>
        /// <param name="offset"><see langword="buffer"/> 中的字节偏移量，从该偏移量开始写入从流中读取的数据。</param>
        /// <param name="count">最多读取的字节数。</param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            return this.Data.Read(buffer, offset, count);
        }
        /// <summary>
        /// 从当前流异步读取字节序列，并将流中的位置提升读取的字节数。
        /// </summary>
        /// <param name="buffer">要将数据写入的内存区域。</param>
        /// <param name="offset"><see langword="buffer"/> 中的字节偏移量，从该偏移量开始写入从流中读取的数据。</param>
        /// <param name="count">最多读取的字节数。</param>
        /// <returns>读取的字节数</returns>
        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return await this.Data.ReadAsync(buffer, offset, count).ConfigureAwait(false);
        }
        #endregion

        #region 设置当前流中的位置
        /// <summary>
        /// 设置当前流中的位置。
        /// </summary>
        /// <param name="offset">相对于 <see langword="origin"/> 参数的字节偏移量。</param>
        /// <param name="origin">指示用于获取新位置的参考点</param>
        /// <returns>当前流中的新位置。</returns>
        public long Seek(int offset, SeekOrigin origin)
        {
            return this.Data.Seek(offset, origin);
        }
        #endregion

        #region 将该流的长度设置为指定值
        /// <summary>
        /// 将该流的长度设置为指定值
        /// </summary>
        /// <param name="value">所需的当前流的长度（以字节表示）。</param>
        public void SetLength(long value)
        {
            this.Data.SetLength(value);
        }
        #endregion

        #region 将清除该流的所有缓冲区，并使得所有缓冲数据被写入到基础设备。
        /// <summary>
        /// 将清除该流的所有缓冲区，并使得所有缓冲数据被写入到基础设备。
        /// </summary>
        public void Flush()
        {
            this.FlushCore();
        }
        /// <summary>
        /// 将清除该流的所有缓冲区，并使得所有缓冲数据被写入到基础设备。
        /// </summary>
        /// <returns></returns>
        public async Task FlushAsync()
        {
            await this.FlushCoreAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// 将清除该流的所有缓冲区，并使得所有缓冲数据被写入到基础设备。
        /// </summary>
        private void FlushCore()
        {
            this.Data.Flush();
            this.Stream.SetLength(0);
            this.Stream.Write(HeaderX.Replace("__", this.Key.IsNullOrEmpty() ? "00" : "01").HexStringToBytes());
            var cms = new MemoryStream();
            if (this.Key.IsNotNullOrEmpty())
            {
                cms.Write(this.Encrypt(this.Data.ToArray()));
            }
            else cms.Write(this.Data.ToArray());
            var ms = new MemoryStream();
            using (var zip = new GZipStream(ms, CompressionLevel.Fastest, true))
            {
                zip.Write(cms.ToArray());
            }
            this.Stream.Write(ms.ToArray());
            ms.Close();
            ms.Dispose();
            cms.Close();
            cms.Dispose();
            this.Stream.Write(FooterX.HexStringToBytes());
            this.Stream.Flush();
        }
        /// <summary>
        /// 将清除该流的所有缓冲区，并使得所有缓冲数据被写入到基础设备。
        /// </summary>
        /// <returns></returns>
        private async Task FlushCoreAsync()
        {
            this.Stream.SetLength(0);
            var headerBytes = HeaderX.Replace("__", this.Key.IsNullOrEmpty() ? "00" : "01").HexStringToBytes();
            await this.Stream.WriteAsync(headerBytes, 0, headerBytes.Length).ConfigureAwait(false);
            var cms = new MemoryStream();
            if (this.Key.IsNotNullOrEmpty())
            {
                cms.Write(this.Encrypt(this.Data.ToArray()));
            }
            else cms.Write(this.Data.ToArray());
            var ms = new MemoryStream();
            using (var zip = new GZipStream(ms, CompressionLevel.Optimal, true))
            {
                var bytes = cms.ToArray();
                await zip.WriteAsync(bytes,0,bytes.Length).ConfigureAwait(false);
            }
            var msBytes = ms.ToArray();
            await this.Stream.WriteAsync(msBytes,0, msBytes.Length).ConfigureAwait(false);
            ms.Close();
            ms.Dispose();
            cms.Close();
            cms.Dispose();
            var footerBytes = FooterX.HexStringToBytes();
            await this.Stream.WriteAsync(footerBytes,0,footerBytes.Length).ConfigureAwait(false);
            await this.Stream.FlushAsync().ConfigureAwait(false);
        }
        #endregion

        #region 加密
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="key">密钥</param>
        public void Encrypt(string key)
        {
            if (key.IsNotNullOrEmpty()) { this.Key = key; this.IsEncrypt = true; }
            var bytes = this.Data.ToArray();
            bytes = Encrypt(bytes);
            this.Data.SetLength(0);
            this.Data.Write(bytes, 0, bytes.Length);
        }
        #endregion

        #region 解密
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="key">密钥</param>
        public void Decrypt(string key)
        {
            if (key.IsNotNullOrEmpty()) { this.Key = key; this.IsEncrypt = true; }
            var bytes = this.Data.ToArray();
            bytes = Decrypt(bytes);
            this.Data.SetLength(0);
            this.Data.Write(bytes, 0, bytes.Length);
        }
        #endregion

        #region 加密文件
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        private byte[] Encrypt(byte[] data)
        {
            var des = Aes.Create();
            var keyIV = this.GetKeyIV();
            des.Key = new byte[32].Write(0, keyIV.Item1);
            des.IV = new byte[16].Write(0, keyIV.Item2);
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            using (var ms = new MemoryStream())
            {
                using (var crypt = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    crypt.Write(data, 0, data.Length);
                    crypt.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }
        #endregion

        #region 解密文件
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        private byte[] Decrypt(byte[] data)
        {
            var des = Aes.Create();
            var keyIV = this.GetKeyIV();
            des.Key = new byte[32].Write(0, keyIV.Item1);
            des.IV = new byte[16].Write(0, keyIV.Item2);
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            using (var ms = new MemoryStream())
            {
                using (var crypt = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    crypt.Write(data, 0, data.Length);
                    crypt.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }
        #endregion

        #region 获取Key IV
        /// <summary>
        /// 获取Key IV
        /// </summary>
        /// <returns></returns>
        private (byte[], byte[]) GetKeyIV()
        {
            var bytes = this.Key.GetBytes();
            var key = bytes;
            var iv = bytes;
            return (key, iv);
        }
        #endregion

        #endregion

        #region 关闭
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            this.Data.Close();
            this.Stream.Close();
        }
        #endregion

        #region 释放
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            base.Dispose(true);
        }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">释放标识</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing, () =>
            {
                if (Data != null)
                {
                    Data?.Flush();
                    Data?.Close();
                    Data?.Dispose();
                    Data = null;
                }
                if (Stream != null)
                {
                    Stream?.Flush();
                    Stream?.Close();
                    Stream?.Dispose();
                    Stream = null;
                }
            });
        }
        /// <summary>
        /// 析构器
        /// </summary>
        ~ElfFile()
        {
            Dispose(false);
        }
        #endregion
    }
}