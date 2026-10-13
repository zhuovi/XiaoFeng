using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using XiaoFeng.Asn1;
/****************************************************************
*  Copyright © (2026) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2026-01-01 10:40:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Cryptography
{
    /// <summary>
    /// SM2国密非对称加密算法
    /// Version : 1.0.0
    /// Create Time : 2026-01-01 10:40:00
    /// Description : 国家密码管理局发布的椭圆曲线公钥密码算法，对应标准 GB/T 32918.1~32918.5，摘要算法使用SM3。
    /// 签名用私钥   验证用公钥   加密用公钥   解密用私钥
    /// </summary>
    public class SM2Encryption : BaseCrypto
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public SM2Encryption()
        {
        }
        #endregion

        #region 常量与曲线参数
        /// <summary>
        /// 默认用户ID
        /// </summary>
        public static readonly string DefaultUserId = "1234567812345678";
        /// <summary>
        /// 椭圆曲线素数p
        /// </summary>
        public static readonly BigInteger P = HexToBigInteger("FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFF");
        /// <summary>
        /// 曲线参数a
        /// </summary>
        public static readonly BigInteger A = HexToBigInteger("FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFC");
        /// <summary>
        /// 曲线参数b
        /// </summary>
        public static readonly BigInteger B = HexToBigInteger("28E9FA9E9D9F5E344D5A9E4BCF6509A7F39789F515AB8F92DDBCBD414D940E93");
        /// <summary>
        /// 阶n
        /// </summary>
        public static readonly BigInteger N = HexToBigInteger("FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFF7203DF6B21C6052B53BBF40939D54123");
        /// <summary>
        /// 基点Gx
        /// </summary>
        public static readonly BigInteger GX = HexToBigInteger("32C4AE2C1F1981195F9904466A39C9948FE30BBFF2660BE1715A4589334C74C7");
        /// <summary>
        /// 基点Gy
        /// </summary>
        public static readonly BigInteger GY = HexToBigInteger("BC3736A2F4F6779C59BDCEE36B692153D0A9877CC62A474002DF32E52139F0A0");
        /// <summary>
        /// 余因子h
        /// </summary>
        public static readonly BigInteger H = BigInteger.One;
        /// <summary>
        /// 基点G
        /// </summary>
        public static BigInteger[] G => new BigInteger[] { GX, GY };
        /// <summary>
        /// 私钥长度(字节)
        /// </summary>
        public static readonly int PrivateKeyLength = 32;
        /// <summary>
        /// 公钥长度(字节) 未压缩
        /// </summary>
        public static readonly int PublicKeyLength = 65;
        #endregion

        #region 方法

        #region 产生密钥
        /// <summary>
        /// 产生SM2密钥对
        /// </summary>
        /// <returns>SM2密钥对</returns>
        public static SM2KeyPair GenerateKeyPair()
        {
            var d = RandomBigInteger(N - BigInteger.One) + BigInteger.One;
            var point = PointMultiply(G, d);
            return new SM2KeyPair
            {
                PrivateKey = ToHex(ToFixedBytes(d, 32)),
                PublicKey = "04" + ToHex(ToFixedBytes(point[0], 32)) + ToHex(ToFixedBytes(point[1], 32))
            };
        }
        #endregion

        #region 签名
        /// <summary>
        /// 签名 返回64字节 r||s
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="privateKey">私钥 64个十六进制字符</param>
        /// <param name="publicKey">公钥 130个十六进制字符</param>
        /// <param name="userId">用户ID</param>
        /// <returns>64字节签名值</returns>
        public static byte[] Sign(byte[] data, string privateKey, string publicKey, string userId = null)
        {
            if (data == null) data = Array.Empty<byte>();
            var d = HexToBigInteger(privateKey);
            var e = Digest(data, publicKey, userId);
            BigInteger r = BigInteger.Zero, s = BigInteger.Zero;
            while (true)
            {
                var k = RandomBigInteger(N - BigInteger.One) + BigInteger.One;
                var point = PointMultiply(G, k);
                r = Mod(e + point[0], N);
                if (r.Sign == 0 || r + k == N) continue;
                s = Mod(ModInverse(BigInteger.One + d, N) * Mod(k - r * d, N), N);
                if (s.Sign == 0) continue;
                break;
            }
            var result = new byte[64];
            Array.Copy(ToFixedBytes(r, 32), 0, result, 0, 32);
            Array.Copy(ToFixedBytes(s, 32), 0, result, 32, 32);
            return result;
        }
        /// <summary>
        /// 签名 返回DER格式 SEQUENCE{r,s}
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="userId">用户ID</param>
        /// <returns>DER签名值</returns>
        public static byte[] SignToDer(byte[] data, string privateKey, string publicKey, string userId = null)
        {
            var signature = Sign(data, privateKey, publicKey, userId);
            var r = new byte[32];
            var s = new byte[32];
            Array.Copy(signature, 0, r, 0, 32);
            Array.Copy(signature, 32, s, 0, 32);
            return DerWriter.Sequence(DerWriter.Integer(r), DerWriter.Integer(s));
        }
        #endregion

        #region 验签
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="signature">64字节签名值 r||s</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="userId">用户ID</param>
        /// <returns>是否一致</returns>
        public static Boolean Verify(byte[] data, byte[] signature, string publicKey, string userId = null)
        {
            if (data == null || signature == null || signature.Length != 64) return false;
            var r = FromUnsignedBigEndian(SubBytes(signature, 0, 32));
            var s = FromUnsignedBigEndian(SubBytes(signature, 32, 32));
            if (r.Sign <= 0 || r >= N || s.Sign <= 0 || s >= N) return false;
            var e = Digest(data, publicKey, userId);
            var t = Mod(r + s, N);
            if (t.Sign == 0) return false;
            var point = PointAdd(PointMultiply(G, s), PointMultiply(DecodePoint(publicKey), t));
            if (point == null) return false;
            var R = Mod(e + point[0], N);
            return R == r;
        }
        /// <summary>
        /// 验证DER格式签名
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="signature">DER签名值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="userId">用户ID</param>
        /// <returns>是否一致</returns>
        public static Boolean VerifyDer(byte[] data, byte[] signature, string publicKey, string userId = null)
        {
            if (signature == null || signature.Length == 0) return false;
            if (signature.Length == 64 && signature[0] != 0x30) return Verify(data, signature, publicKey, userId);
            var node = DerReader.Parse(signature);
            if (node.Count < 2) return false;
            var r = ToFixedBytesBigEndian(node[0].GetIntegerBytes(), 32);
            var s = ToFixedBytesBigEndian(node[1].GetIntegerBytes(), 32);
            var sign = new byte[64];
            Array.Copy(r, 0, sign, 0, 32);
            Array.Copy(s, 0, sign, 32, 32);
            return Verify(data, sign, publicKey, userId);
        }
        #endregion

        #region 加密
        /// <summary>
        /// 加密 输出 C1||C3||C2
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="publicKey">公钥 130个十六进制字符</param>
        /// <returns>密文</returns>
        public static byte[] Encrypt(byte[] data, string publicKey)
        {
            if (data == null) data = Array.Empty<byte>();
            var point = DecodePoint(publicKey);
            byte[] c1, c2, c3;
            while (true)
            {
                var k = RandomBigInteger(N - BigInteger.One) + BigInteger.One;
                var c1Point = PointMultiply(G, k);
                c1 = ConcatBytes(new byte[] { 0x04 }, ToFixedBytes(c1Point[0], 32), ToFixedBytes(c1Point[1], 32));
                var sPoint = PointMultiply(point, k);
                if (sPoint == null) continue;
                var x2 = ToFixedBytes(sPoint[0], 32);
                var y2 = ToFixedBytes(sPoint[1], 32);
                var t = KDF(ConcatBytes(x2, y2), data.Length);
                if (IsZeroBytes(t)) continue;
                c2 = new byte[data.Length];
                for (var i = 0; i < data.Length; i++) c2[i] = (byte)(data[i] ^ t[i]);
                c3 = SM3Encryption.Hash(ConcatBytes(x2, data, y2));
                break;
            }
            return ConcatBytes(c1, c3, c2);
        }
        #endregion

        #region 解密
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">密文 C1||C3||C2</param>
        /// <param name="privateKey">私钥</param>
        /// <returns>明文</returns>
        public static byte[] Decrypt(byte[] data, string privateKey)
        {
            if (data == null || data.Length < 97) return Array.Empty<byte>();
            var c1 = SubBytes(data, 0, 65);
            var c3 = SubBytes(data, 65, 32);
            var c2 = SubBytes(data, 97, data.Length - 97);
            var point = DecodePoint(ToHex(c1));
            if (point == null) return Array.Empty<byte>();
            if (!IsOnCurve(point)) return Array.Empty<byte>();
            var d = HexToBigInteger(privateKey);
            var sPoint = PointMultiply(point, d);
            if (sPoint == null) return Array.Empty<byte>();
            var x2 = ToFixedBytes(sPoint[0], 32);
            var y2 = ToFixedBytes(sPoint[1], 32);
            var t = KDF(ConcatBytes(x2, y2), c2.Length);
            if (IsZeroBytes(t)) return Array.Empty<byte>();
            var m = new byte[c2.Length];
            for (var i = 0; i < c2.Length; i++) m[i] = (byte)(c2[i] ^ t[i]);
            var u = SM3Encryption.Hash(ConcatBytes(x2, m, y2));
            if (ToHex(u) != ToHex(c3)) return Array.Empty<byte>();
            return m;
        }
        #endregion

        #region 摘要
        /// <summary>
        /// 计算ZA||M的摘要值
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="userId">用户ID</param>
        /// <returns>摘要大整数</returns>
        public static BigInteger Digest(byte[] data, string publicKey, string userId = null)
        {
            if (data == null) data = Array.Empty<byte>();
            if (userId.IsNullOrEmpty()) userId = DefaultUserId;
            var idBytes = Encoding.UTF8.GetBytes(userId);
            var bitLength = idBytes.Length * 8;
            var ms = new MemoryStream();
            ms.WriteByte((byte)((bitLength >> 8) & 0xFF));
            ms.WriteByte((byte)(bitLength & 0xFF));
            ms.Write(idBytes, 0, idBytes.Length);
            ms.Write(ToFixedBytes(A, 32), 0, 32);
            ms.Write(ToFixedBytes(B, 32), 0, 32);
            ms.Write(ToFixedBytes(GX, 32), 0, 32);
            ms.Write(ToFixedBytes(GY, 32), 0, 32);
            var point = DecodePoint(publicKey);
            ms.Write(ToFixedBytes(point[0], 32), 0, 32);
            ms.Write(ToFixedBytes(point[1], 32), 0, 32);
            var za = SM3Encryption.Hash(ms.ToArray());
            var m = new byte[za.Length + data.Length];
            Array.Copy(za, 0, m, 0, za.Length);
            Array.Copy(data, 0, m, za.Length, data.Length);
            return FromUnsignedBigEndian(SM3Encryption.Hash(m));
        }
        #endregion

        #region 密钥派生函数
        /// <summary>
        /// 密钥派生函数KDF
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="keyLength">密钥长度(字节)</param>
        /// <returns>派生密钥</returns>
        public static byte[] KDF(byte[] data, int keyLength)
        {
            if (keyLength <= 0) return Array.Empty<byte>();
            var count = (keyLength + 31) / 32;
            var ms = new MemoryStream();
            for (var i = 1; i <= count; i++)
            {
                var counter = new byte[] { (byte)((i >> 24) & 0xFF), (byte)((i >> 16) & 0xFF), (byte)((i >> 8) & 0xFF), (byte)(i & 0xFF) };
                ms.Write(SM3Encryption.Hash(ConcatBytes(data, counter)), 0, 32);
            }
            var result = new byte[keyLength];
            Array.Copy(ms.ToArray(), 0, result, 0, keyLength);
            return result;
        }
        #endregion

        #region 椭圆曲线运算
        /// <summary>
        /// 取模
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="modulus">模数</param>
        /// <returns>结果</returns>
        public static BigInteger Mod(BigInteger value, BigInteger modulus)
        {
            var result = value % modulus;
            if (result.Sign < 0) result += modulus;
            return result;
        }
        /// <summary>
        /// 取模
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>结果</returns>
        public static BigInteger Mod(BigInteger value) => Mod(value, P);
        /// <summary>
        /// 模逆
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="modulus">模数</param>
        /// <returns>模逆</returns>
        public static BigInteger ModInverse(BigInteger value, BigInteger modulus)
        {
            var r0 = Mod(value, modulus);
            var r1 = modulus;
            var s0 = BigInteger.One;
            var s1 = BigInteger.Zero;
            while (r1.Sign != 0)
            {
                var q = BigInteger.Divide(r0, r1);
                var r = r0 - q * r1;
                r0 = r1;
                r1 = r;
                var s = s0 - q * s1;
                s0 = s1;
                s1 = s;
            }
            var result = s0 % modulus;
            if (result.Sign < 0) result += modulus;
            return result;
        }
        /// <summary>
        /// 点加
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <returns>结果点 无穷远点返回null</returns>
        public static BigInteger[] PointAdd(BigInteger[] p1, BigInteger[] p2)
        {
            if (p1 == null) return p2;
            if (p2 == null) return p1;
            BigInteger lambda;
            if (p1[0] == p2[0])
            {
                if (Mod(p1[1] + p2[1], P).Sign == 0) return null;
                lambda = Mod((3 * p1[0] * p1[0] + A) * ModInverse(2 * p1[1], P), P);
            }
            else
            {
                lambda = Mod((p2[1] - p1[1]) * ModInverse(p2[0] - p1[0], P), P);
            }
            var x3 = Mod(lambda * lambda - p1[0] - p2[0], P);
            var y3 = Mod(lambda * (p1[0] - x3) - p1[1], P);
            return new BigInteger[] { x3, y3 };
        }
        /// <summary>
        /// 倍点
        /// </summary>
        /// <param name="point">点</param>
        /// <returns>结果点</returns>
        public static BigInteger[] PointDouble(BigInteger[] point) => PointAdd(point, point);
        /// <summary>
        /// 标量乘
        /// </summary>
        /// <param name="point">点</param>
        /// <param name="k">倍数</param>
        /// <returns>结果点</returns>
        public static BigInteger[] PointMultiply(BigInteger[] point, BigInteger k)
        {
            if (point == null || k.Sign == 0) return null;
            BigInteger[] result = null;
            var q = point;
            var e = k;
            while (e.Sign > 0)
            {
                if ((e & BigInteger.One).Sign > 0) result = PointAdd(result, q);
                q = PointAdd(q, q);
                e = e >> 1;
            }
            return result;
        }
        /// <summary>
        /// 判断点是否在曲线上
        /// </summary>
        /// <param name="point">点</param>
        /// <returns>是否在曲线上</returns>
        public static Boolean IsOnCurve(BigInteger[] point)
        {
            if (point == null) return false;
            var x = point[0];
            var y = point[1];
            if (x.Sign < 0 || x >= P || y.Sign < 0 || y >= P) return false;
            var left = Mod(y * y, P);
            var right = Mod(x * x * x + A * x + B, P);
            return left == right;
        }
        /// <summary>
        /// 解码公钥点
        /// </summary>
        /// <param name="publicKey">公钥 130或128个十六进制字符</param>
        /// <returns>点坐标</returns>
        public static BigInteger[] DecodePoint(string publicKey)
        {
            if (publicKey.IsNullOrEmpty()) return null;
            var hex = publicKey.Replace(" ", "").Replace("-", "");
            if (hex.StartsWith("04", StringComparison.OrdinalIgnoreCase)) hex = hex.Substring(2);
            if (hex.Length != 64 * 2) return null;
            var bytes = FromHex(hex);
            var x = FromUnsignedBigEndian(SubBytes(bytes, 0, 32));
            var y = FromUnsignedBigEndian(SubBytes(bytes, 32, 32));
            return new BigInteger[] { x, y };
        }
        #endregion

        #region 数据转换
        /// <summary>
        /// 十六进制转大整数
        /// </summary>
        /// <param name="hex">十六进制字符串</param>
        /// <returns>大整数</returns>
        public static BigInteger HexToBigInteger(string hex)
        {
            if (hex.IsNullOrEmpty()) return BigInteger.Zero;
            var value = hex.Replace(" ", "").Replace("-", "").ToUpper();
            if (value.StartsWith("0X", StringComparison.OrdinalIgnoreCase)) value = value.Substring(2);
            return BigInteger.Parse("0" + value, NumberStyles.HexNumber);
        }
        /// <summary>
        /// 大整数转固定长度字节
        /// </summary>
        /// <param name="value">大整数</param>
        /// <param name="length">长度</param>
        /// <returns>大端字节</returns>
        public static byte[] ToFixedBytes(BigInteger value, int length)
        {
            var bytes = value.ToByteArray();
            var list = new List<byte>(bytes);
            while (list.Count > 1 && list[list.Count - 1] == 0) list.RemoveAt(list.Count - 1);
            list.Reverse();
            var result = new byte[length];
            if (list.Count > length) throw new ArgumentException("数据长度超出限制.");
            Array.Copy(list.ToArray(), 0, result, length - list.Count, list.Count);
            return result;
        }
        /// <summary>
        /// 大端无符号字节转大整数
        /// </summary>
        /// <param name="data">大端字节</param>
        /// <returns>大整数</returns>
        public static BigInteger FromUnsignedBigEndian(byte[] data)
        {
            if (data == null || data.Length == 0) return BigInteger.Zero;
            var bytes = new byte[data.Length + 1];
            Array.Copy(data, 0, bytes, 1, data.Length);
            Array.Reverse(bytes);
            return new BigInteger(bytes);
        }
        /// <summary>
        /// 大端字节补齐到指定长度
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">长度</param>
        /// <returns>补齐后的数据</returns>
        public static byte[] ToFixedBytesBigEndian(byte[] data, int length)
        {
            var result = new byte[length];
            if (data == null || data.Length == 0) return result;
            if (data.Length > length) Array.Copy(data, data.Length - length, result, 0, length);
            else Array.Copy(data, 0, result, length - data.Length, data.Length);
            return result;
        }
        /// <summary>
        /// 字节转十六进制
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>十六进制字符串</returns>
        public static string ToHex(byte[] data)
        {
            if (data == null || data.Length == 0) return string.Empty;
            var sb = new StringBuilder();
            foreach (var b in data) sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
        /// <summary>
        /// 十六进制转字节
        /// </summary>
        /// <param name="hex">十六进制字符串</param>
        /// <returns>字节</returns>
        public static byte[] FromHex(string hex)
        {
            if (hex.IsNullOrEmpty()) return Array.Empty<byte>();
            var value = hex.Replace(" ", "").Replace("-", "");
            if (value.StartsWith("0X", StringComparison.OrdinalIgnoreCase)) value = value.Substring(2);
            if (value.Length % 2 != 0) value = "0" + value;
            var result = new byte[value.Length / 2];
            for (var i = 0; i < result.Length; i++)
                result[i] = byte.Parse(value.Substring(i * 2, 2), NumberStyles.HexNumber);
            return result;
        }
        /// <summary>
        /// 截取字节
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移量</param>
        /// <param name="length">长度</param>
        /// <returns>字节</returns>
        public static byte[] SubBytes(byte[] data, int offset, int length)
        {
            if (data == null || length <= 0) return Array.Empty<byte>();
            if (offset + length > data.Length) length = data.Length - offset;
            if (length <= 0) return Array.Empty<byte>();
            var result = new byte[length];
            Array.Copy(data, offset, result, 0, length);
            return result;
        }
        /// <summary>
        /// 拼接字节
        /// </summary>
        /// <param name="items">字节集</param>
        /// <returns>字节</returns>
        public static byte[] ConcatBytes(params byte[][] items)
        {
            var ms = new MemoryStream();
            foreach (var item in items)
            {
                if (item == null || item.Length == 0) continue;
                ms.Write(item, 0, item.Length);
            }
            return ms.ToArray();
        }
        /// <summary>
        /// 是否全为零
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>是否全为零</returns>
        private static Boolean IsZeroBytes(byte[] data)
        {
            if (data == null) return true;
            foreach (var b in data)
                if (b != 0) return false;
            return true;
        }
        /// <summary>
        /// 生成指定范围内的随机数
        /// </summary>
        /// <param name="max">最大值(不含)</param>
        /// <returns>随机数</returns>
        private static BigInteger RandomBigInteger(BigInteger max)
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return FromUnsignedBigEndian(bytes) % max;
        }
        #endregion

        #region 密码方法
        ///<inheritdoc/>
        public override byte[] Encode(byte[] data, byte[] slatKey, byte[] vector, CryptographyType type = CryptographyType.Encrypt, System.Security.Cryptography.CipherMode cipherMode = System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode paddingMode = System.Security.Cryptography.PaddingMode.PKCS7)
        {
            var key = this.GetString(slatKey);
            return type == CryptographyType.Encrypt ? Encrypt(data, key) : Decrypt(data, key);
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="outputMode">输出模式</param>
        /// <returns>密文</returns>
        public override string Encrypt(string data, string publicKey, OutputMode outputMode) => this.OutputString(Encrypt(this.GetBytes(data), publicKey), outputMode);
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="outputMode">输出模式</param>
        /// <returns>明文</returns>
        public override string Decrypt(string data, string privateKey, OutputMode outputMode) => this.GetString(Decrypt(this.InputBytes(data, outputMode), privateKey));
        #endregion

        #endregion
    }
}
