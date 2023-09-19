using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using XiaoFeng.Memcached.Transform;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-07 11:12:13                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// 帮助类
    /// </summary>
    public static class Helper
    {
        #region 属性

        #endregion

        #region 方法

        #region 设置数据
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="type">类型</param>
        /// <param name="compressionLength">压缩大小</param>
        /// <returns>压缩后的数据</returns>
        public static byte[] Serialize(object value, out ValueType type, uint compressionLength)
        {
            if (value == null)
            {
                type = ValueType.Object;
                return Array.Empty<byte>();
            }
            byte[] bytes;
            if (value is byte[] bsval)
            {
                bytes = bsval;
                type = ValueType.ByteArray;
                if (bytes.Length > compressionLength)
                {
                    bytes = Compression(bytes);
                    type = ValueType.CompressedByteArray;
                }
            }
            else if (value is string sval)
            {
                bytes = sval.IsNullOrEmpty() ? Array.Empty<byte>() : sval.GetBytes(Encoding.UTF8);
                type = ValueType.String;
                if (bytes.Length > compressionLength)
                {
                    bytes = Compression(bytes);
                    type = ValueType.CompressedString;
                }
            }
            else if (value is DateTime dtval)
            {
                bytes = BitConverter.GetBytes(dtval.Ticks);
                type = ValueType.Datetime;
            }
            else if (value is bool bval)
            {
                bytes = new byte[] { (byte)(bval ? 1 : 0) };
                type = ValueType.Boolean;
            }
            else if (value is byte byval)
            {
                bytes = new byte[] { byval };
                type = ValueType.Byte;
            }
            else if (value is short shval)
            {
                bytes = BitConverter.GetBytes(shval);
                type = ValueType.Short;
            }
            else if (value is ushort usval)
            {
                bytes = BitConverter.GetBytes(usval);
                type = ValueType.UShort;
            }
            else if (value is int ival)
            {
                bytes = BitConverter.GetBytes(ival);
                type = ValueType.Int;
            }
            else if (value is uint uival)
            {
                bytes = BitConverter.GetBytes(uival);
                type = ValueType.UInt;
            }
            else if (value is long lval)
            {
                bytes = BitConverter.GetBytes(lval);
                type = ValueType.Long;
            }
            else if (value is ulong uval)
            {
                bytes = BitConverter.GetBytes(uval);
                type = ValueType.ULong;
            }
            else if (value is float fval)
            {
                bytes = BitConverter.GetBytes(fval);
                type = ValueType.Float;
            }
            else if (value is double dval)
            {
                bytes = BitConverter.GetBytes(dval);
                type = ValueType.Double;
            }
            else
            {
                type = ValueType.Object;
                var json = value.GetType().AssemblyQualifiedName + "\r\n" + value.ToJson();
                bytes = json.GetBytes();
                if (bytes.Length > compressionLength)
                {
                    bytes = Compression(bytes);
                    type = ValueType.CompressedObject;
                }
                /*
                using (MemoryStream ms = new MemoryStream())
                {
                    new BinaryFormatter().Serialize(ms, value);
                    bytes = ms.ToArray();
                    type = ValueType.Object;
                    if (bytes.Length > compressionLength)
                    {
                        bytes = Compression(bytes);
                        type = ValueType.CompressedObject;
                    }
                }*/
            }
            return bytes;
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object Deserialize(byte[] bytes, ValueType type)
        {
            switch (type)
            {
                case ValueType.String:
                    return bytes == null || bytes.Length == 0 ? string.Empty : bytes.GetString(Encoding.UTF8);
                case ValueType.Datetime:
                    return new DateTime(BitConverter.ToInt64(bytes, 0));
                case ValueType.Boolean:
                    return bytes[0] == 1;
                case ValueType.Byte:
                    return bytes[0];
                case ValueType.Short:
                    return BitConverter.ToInt16(bytes, 0);
                case ValueType.UShort:
                    return BitConverter.ToUInt16(bytes, 0);
                case ValueType.Int:
                    return BitConverter.ToInt32(bytes, 0);
                case ValueType.UInt:
                    return BitConverter.ToUInt32(bytes, 0);
                case ValueType.Long:
                    return BitConverter.ToInt64(bytes, 0);
                case ValueType.ULong:
                    return BitConverter.ToUInt64(bytes, 0);
                case ValueType.Float:
                    return BitConverter.ToSingle(bytes, 0);
                case ValueType.Double:
                    return BitConverter.ToDouble(bytes, 0);
                case ValueType.Object:
                    using (var oReader = new StreamReader(new MemoryStream(bytes)))
                    {
                        var otypeName = oReader.ReadLine();
                        var otype = Type.GetType(otypeName);
                        return oReader.ReadToEnd().JsonToObject(otype);
                    }
                case ValueType.CompressedByteArray:
                    return Deserialize(Decompression(bytes), ValueType.ByteArray);
                case ValueType.CompressedString:
                    return Deserialize(Decompression(bytes), ValueType.String);
                case ValueType.CompressedObject:
                    using (var coReader = new StreamReader(new MemoryStream(Decompression(bytes))))
                    {
                        var cotypeName = coReader.ReadLine();
                        var cotype = Type.GetType(cotypeName);
                        return coReader.ReadToEnd().JsonToObject(cotype);
                    }
                //return Deserialize(Decompression(bytes), ValueType.Object);
                case ValueType.ByteArray:
                default:
                    return bytes;
            }
        }
        #endregion

        #region 压缩数据
        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns></returns>
        private static byte[] Compression(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return Array.Empty<byte>();
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress, false))
                {
                    ds.Write(bytes, 0, bytes.Length);
                }
                ms.Close();
                return ms.ToArray();
            }
        }
        #endregion

        #region 解压数据
        /// <summary>
        /// 解压数据
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns></returns>
        private static byte[] Decompression(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return Array.Empty<byte>();
            using (MemoryStream ms = new MemoryStream(bytes, false))
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress, false))
                {
                    using (MemoryStream dest = new MemoryStream())
                    {
                        byte[] tmp = new byte[bytes.Length];
                        int read;
                        while ((read = ds.Read(tmp, 0, tmp.Length)) != 0)
                            dest.Write(tmp, 0, read);
                        dest.Close();
                        return dest.ToArray();
                    }
                }
            }
        }
        #endregion

        #region 检查Key
        /// <summary>
        /// 检查Key
        /// </summary>
        /// <param name="key">key</param>
        /// <exception cref="ArgumentNullException">空异常</exception>
        /// <exception cref="ArgumentException">参数异常</exception>
        public static void CheckKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException("KEY不能为空.");
            if (key.Length == 0)
                throw new ArgumentException("KEY不能为空.");
            if (key.Length > 250)
                throw new ArgumentException("KEY的长度不能超过250个字符.");
            key.Each(c =>
            {
                if (c <= 32)
                    throw new ArgumentException("KEY不能包含空白字符或控制字符.");
            });
        }
        #endregion

        #endregion
    }
}