using System;
using System.IO;
using System.Text;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 10:00:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// SM3国密摘要算法
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 10:00:00
    /// Description : 国家密码管理局发布的商用密码摘要算法，输出256位(32字节)摘要值，对应标准 GB/T 32905-2016。
    /// </summary>
    public class SM3Encryption : BaseCrypto
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SM3Encryption()
        {
        }
        #endregion

        #region 属性
        /// <summary>
        /// 摘要长度(字节)
        /// </summary>
        public static int HashSize => 32;
        /// <summary>
        /// 初始值IV
        /// </summary>
        private static readonly uint[] IV = new uint[]
        {
            0x7380166Fu, 0x4914B2B9u, 0x172442D7u, 0xDA8A0600u,
            0xA96F30BCu, 0x163138AAu, 0xE38DEE4Du, 0xB0FB0E4Eu
        };
        #endregion

        #region 方法

        #region 摘要
        /// <summary>
        /// 获取摘要值
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>32字节摘要</returns>
        public static byte[] Hash(byte[] data)
        {
            if (data == null) data = Array.Empty<byte>();
            var message = Padding(data);
            var v = (uint[])IV.Clone();
            for (var i = 0; i < message.Length; i += 64)
                v = CF(v, message, i);
            var result = new byte[32];
            for (var i = 0; i < 8; i++)
                WriteBE32(result, i * 4, v[i]);
            return result;
        }
        /// <summary>
        /// 获取摘要值
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="encoding">编码</param>
        /// <returns>32字节摘要</returns>
        public static byte[] Hash(string data, Encoding encoding = null) => Hash((encoding ?? Encoding.UTF8).GetBytes(data ?? ""));
        /// <summary>
        /// 获取流的摘要值
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <returns>32字节摘要</returns>
        public static byte[] Hash(Stream stream)
        {
            if (stream == null) return Hash(Array.Empty<byte>());
            using (var ms = new MemoryStream())
            {
                if (stream.CanSeek) stream.Position = 0;
                stream.CopyTo(ms);
                return Hash(ms.ToArray());
            }
        }
        /// <summary>
        /// 获取摘要值并转成字符串
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="outputMode">输出模式</param>
        /// <returns>摘要字符串</returns>
        public string HashString(byte[] data, OutputMode outputMode = OutputMode.Hex) => this.OutputString(Hash(data), outputMode);
        /// <summary>
        /// 获取摘要值并转成字符串
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="outputMode">输出模式</param>
        /// <returns>摘要字符串</returns>
        public string HashString(string data, OutputMode outputMode = OutputMode.Hex) => this.OutputString(Hash(data), outputMode);
        #endregion

        #region 密码方法
        ///<inheritdoc/>
        public override byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, System.Security.Cryptography.CipherMode cipherMode = System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode paddingMode = System.Security.Cryptography.PaddingMode.PKCS7) => Hash(data);
        ///<inheritdoc/>
        public override string Encrypt(string data, string slatKey = "", string vectorKey = "", OutputMode outputMode = OutputMode.Hex) => this.OutputString(Hash(data), outputMode);
        ///<inheritdoc/>
        public override string Encrypt(string data, OutputMode outputMode) => this.OutputString(Hash(data), outputMode);
        #endregion

        #region 消息填充
        /// <summary>
        /// 消息填充
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>填充后的数据，长度为64的倍数</returns>
        private static byte[] Padding(byte[] data)
        {
            var bitLength = (ulong)data.Length * 8;
            var remain = (data.Length + 1) % 64;
            var padLength = remain <= 56 ? 56 - remain : 120 - remain;
            var result = new byte[data.Length + 1 + padLength + 8];
            Array.Copy(data, 0, result, 0, data.Length);
            result[data.Length] = 0x80;
            for (var i = 0; i < 8; i++)
                result[result.Length - 1 - i] = (byte)((bitLength >> (8 * i)) & 0xFF);
            return result;
        }
        #endregion

        #region 压缩函数
        /// <summary>
        /// 压缩函数CF
        /// </summary>
        /// <param name="v">上一轮结果</param>
        /// <param name="block">数据块</param>
        /// <param name="offset">偏移量</param>
        /// <returns>本轮结果</returns>
        private static uint[] CF(uint[] v, byte[] block, int offset)
        {
            var w = new uint[68];
            var w1 = new uint[64];
            for (var j = 0; j < 16; j++)
                w[j] = ReadBE32(block, offset + j * 4);
            for (var j = 16; j < 68; j++)
                w[j] = P1(w[j - 16] ^ w[j - 9] ^ RotateLeft(w[j - 3], 15)) ^ RotateLeft(w[j - 13], 7) ^ w[j - 6];
            for (var j = 0; j < 64; j++)
                w1[j] = w[j] ^ w[j + 4];

            var a = v[0]; var b = v[1]; var c = v[2]; var d = v[3];
            var e = v[4]; var f = v[5]; var g = v[6]; var h = v[7];
            for (var j = 0; j < 64; j++)
            {
                var ss1 = RotateLeft(RotateLeft(a, 12) + e + RotateLeft(T(j), j), 7);
                var ss2 = ss1 ^ RotateLeft(a, 12);
                var tt1 = FF(a, b, c, j) + d + ss2 + w1[j];
                var tt2 = GG(e, f, g, j) + h + ss1 + w[j];
                d = c;
                c = RotateLeft(b, 9);
                b = a;
                a = tt1;
                h = g;
                g = RotateLeft(f, 19);
                f = e;
                e = P0(tt2);
            }
            return new uint[] { a ^ v[0], b ^ v[1], c ^ v[2], d ^ v[3], e ^ v[4], f ^ v[5], g ^ v[6], h ^ v[7] };
        }
        #endregion

        #region 置换函数
        /// <summary>
        /// 常量T
        /// </summary>
        /// <param name="j">轮数</param>
        /// <returns></returns>
        private static uint T(int j) => j < 16 ? 0x79CC4519u : 0x7A879D8Au;
        /// <summary>
        /// 布尔函数FF
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <param name="j">轮数</param>
        /// <returns></returns>
        private static uint FF(uint x, uint y, uint z, int j) => j < 16 ? (x ^ y ^ z) : ((x & y) | (x & z) | (y & z));
        /// <summary>
        /// 布尔函数GG
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <param name="j">轮数</param>
        /// <returns></returns>
        private static uint GG(uint x, uint y, uint z, int j) => j < 16 ? (x ^ y ^ z) : ((x & y) | (~x & z));
        /// <summary>
        /// 置换函数P0
        /// </summary>
        /// <param name="x">x</param>
        /// <returns></returns>
        private static uint P0(uint x) => x ^ RotateLeft(x, 9) ^ RotateLeft(x, 17);
        /// <summary>
        /// 置换函数P1
        /// </summary>
        /// <param name="x">x</param>
        /// <returns></returns>
        private static uint P1(uint x) => x ^ RotateLeft(x, 15) ^ RotateLeft(x, 23);
        /// <summary>
        /// 循环左移
        /// </summary>
        /// <param name="x">数据</param>
        /// <param name="n">位数</param>
        /// <returns></returns>
        private static uint RotateLeft(uint x, int n) => (x << n) | (x >> (32 - n));
        #endregion

        #region 字节与整型互转
        /// <summary>
        /// 大端读取4字节
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        private static uint ReadBE32(byte[] data, int offset) =>
            ((uint)data[offset] << 24) | ((uint)data[offset + 1] << 16) | ((uint)data[offset + 2] << 8) | data[offset + 3];
        /// <summary>
        /// 大端写入4字节
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移量</param>
        /// <param name="value">值</param>
        private static void WriteBE32(byte[] data, int offset, uint value)
        {
            data[offset] = (byte)(value >> 24);
            data[offset + 1] = (byte)(value >> 16);
            data[offset + 2] = (byte)(value >> 8);
            data[offset + 3] = (byte)value;
        }
        #endregion

        #endregion
    }
}
