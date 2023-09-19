using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-13 17:09:15                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Binary
{
    /// <summary>
    /// 命令码
    /// </summary>
    public enum CommandOpcode
    {
        /// <summary>
        /// 获取key的value值
        /// </summary>
        [Description("获取key的value值")]
        [EnumName("Get")]
        Get = 0x00,
        /// <summary>
        /// 给key设置一个值
        /// </summary>
        [Description("给key设置一个值")]
        [EnumName("Set")]
        Set = 0x01,
        /// <summary>
        /// 如果key不存在的话，就添加
        /// </summary>
        [Description("如果key不存在的话，就添加")]
        [EnumName("Add")]
        Add = 0x02,
        /// <summary>
        /// 用来替换已知key的value
        /// </summary>
        [Description("用来替换已知key的value")]
        [EnumName("Replace")]
        Replace = 0x03,
        /// <summary>
        /// 删除Key
        /// </summary>
        [Description("删除Key")]
        [EnumName("Delete")]
        Delete = 0x04,
        /// <summary>
        /// 递增
        /// </summary>
        [Description("递增")]
        [EnumName("Increment")]
        Increment = 0x05,
        /// <summary>
        /// 递减
        /// </summary>
        [Description("递减")]
        [EnumName("Decrement")]
        Decrement = 0x06,
        /// <summary>
        /// 退出
        /// </summary>
        [Description("退出")]
        [EnumName("Quit")]
        Quit = 0x07,
        /// <summary>
        /// 清理缓存
        /// </summary>
        [Description("清理缓存")]
        [EnumName("FlushAll")]
        FlushAll = 0x08,
        /// <summary>
        /// GetQ
        /// </summary>
        [Description("GetQ")]
        [EnumName("GetQ")]
        GetQ = 0x09,
        /// <summary>
        /// No-op
        /// </summary>
        [Description("No-op")]
        [EnumName("No-op")]
        Noop = 0x0A,
        /// <summary>
        /// 版本
        /// </summary>
        [Description("版本")]
        [EnumName("Version")]
        Version = 0x0B,
        /// <summary>
        /// GetK
        /// </summary>
        [Description("GetK")]
        [EnumName("GetK")]
        GetK = 0x0C,
        /// <summary>
        /// GetKQ
        /// </summary>
        [Description("GetKQ")]
        [EnumName("GetKQ")]
        GetKQ = 0x0D,
        /// <summary>
        /// Append
        /// </summary>
        [Description("Append")]
        [EnumName("Append")]
        Append = 0x0E,
        /// <summary>
        /// Prepend
        /// </summary>
        [Description("Prepend")]
        [EnumName("Prepend")]
        Prepend = 0x0F,
        /// <summary>
        /// Stat
        /// </summary>
        [Description("Stat")]
        [EnumName("Stat")]
        Stat = 0x10,
        /// <summary>
        /// SetQ
        /// </summary>
        [Description("SetQ")]
        [EnumName("SetQ")]
        SetQ = 0x11,
        /// <summary>
        /// AddQ
        /// </summary>
        [Description("AddQ")]
        [EnumName("AddQ")]
        AddQ = 0x12,
        /// <summary>
        /// ReplaceQ
        /// </summary>
        [Description("ReplaceQ")]
        [EnumName("ReplaceQ")]
        ReplaceQ = 0x13,
        /// <summary>
        /// DeleteQ
        /// </summary>
        [Description("DeleteQ")]
        [EnumName("DeleteQ")]
        DeleteQ = 0x14,
        /// <summary>
        /// IncrementQ
        /// </summary>
        [Description("IncrementQ")]
        [EnumName("IncrementQ")]
        IncrementQ = 0x15,
        /// <summary>
        /// DecrementQ
        /// </summary>
        [Description("DecrementQ")]
        [EnumName("DecrementQ")]
        DecrementQ = 0x16,
        /// <summary>
        /// QuitQ
        /// </summary>
        [Description("QuitQ")]
        [EnumName("QuitQ")]
        QuitQ = 0x17,
        /// <summary>
        /// FlushQ
        /// </summary>
        [Description("FlushQ")]
        [EnumName("FlushQ")]
        FlushQ = 0x18,
        /// <summary>
        /// AppendQ
        /// </summary>
        [Description("AppendQ")]
        [EnumName("AppendQ")]
        AppendQ = 0x19,
        /// <summary>
        /// PrependQ
        /// </summary>
        [Description("PrependQ")]
        [EnumName("PrependQ")]
        PrependQ = 0x1A,
        /// <summary>
        /// Verbosity
        /// </summary>
        [Description("Verbosity")]
        [EnumName("Verbosity")]
        Verbosity = 0x1B,
        /// <summary>
        /// 更新过期时间
        /// </summary>
        [Description("更新过期时间")]
        [EnumName("Touch")]
        Touch = 0x1C,
        /// <summary>
        /// GAT
        /// </summary>
        [Description("GAT")]
        [EnumName("GAT")]
        GAT = 0x1D,
        /// <summary>
        /// GATQ
        /// </summary>
        [Description("GATQ")]
        [EnumName("GATQ")]
        GATQ = 0x1E,
        /// <summary>
        /// SASL list mechs
        /// </summary>
        [Description("SASL list mechs")]
        [EnumName("SASL list mechs")]
        SASLListtMechs = 0x20,
        /// <summary>
        /// SASL Auth
        /// </summary>
        [Description("SASL Auth")]
        [EnumName("SASL Auth")]
        SASLAuth = 0x21,
        /// <summary>
        /// SASL Step
        /// </summary>
        [Description("SASL Step")]
        [EnumName("SASL Step")]
        SASLStep = 0x22,
        /// <summary>
        /// RGet
        /// </summary>
        [Description("RGet")]
        [EnumName("RGet")]
        RGet = 0x30,
        /// <summary>
        /// RSet
        /// </summary>
        [Description("RSet")]
        [EnumName("RSet")]
        RSet = 0x31,
        /// <summary>
        /// RSetQ
        /// </summary>
        [Description("RSetQ")]
        [EnumName("RSetQ")]
        RSetQ = 0x32,
        /// <summary>
        /// RAppend
        /// </summary>
        [Description("RAppend")]
        [EnumName("RAppend")]
        RAppend = 0x33,
        /// <summary>
        /// RAppendQ
        /// </summary>
        [Description("RAppendQ")]
        [EnumName("RAppendQ")]
        RAppendQ = 0x34,
        /// <summary>
        /// RPrepend
        /// </summary>
        [Description("RPrepend")]
        [EnumName("RPrepend")]
        RPrepend = 0x35,
        /// <summary>
        /// RPrependQ
        /// </summary>
        [Description("RPrependQ")]
        [EnumName("RPrependQ")]
        RPrependQ = 0x36,
        /// <summary>
        /// RDelete
        /// </summary>
        [Description("RDelete")]
        [EnumName("RDelete")]
        RDelete = 0x37,
        /// <summary>
        /// RDeleteQ
        /// </summary>
        [Description("RDeleteQ")]
        [EnumName("RDeleteQ")]
        RDeleteQ = 0x38,
        /// <summary>
        /// RIncr
        /// </summary>
        [Description("RIncr")]
        [EnumName("RIncr")]
        RIncr = 0x39,
        /// <summary>
        /// RIncrQ
        /// </summary>
        [Description("RIncrQ")]
        [EnumName("RIncrQ")]
        RIncrQ = 0x3A,
        /// <summary>
        /// RDecr
        /// </summary>
        [Description("RDecr")]
        [EnumName("RDecr")]
        RDecr = 0x3B,
        /// <summary>
        /// RDecrQ
        /// </summary>
        [Description("RDecrQ")]
        [EnumName("RDecrQ")]
        RDecrQ = 0x3C,
        /// <summary>
        /// Set VBucket
        /// </summary>
        [Description("Set VBucket")]
        [EnumName("Set VBucket")]
        SetVBucket = 0x3D,
        /// <summary>
        /// Get VBucket
        /// </summary>
        [Description("Get VBucket")]
        [EnumName("Get VBucket")]
        GetVBucket = 0x3E,
        /// <summary>
        /// Del VBucket
        /// </summary>
        [Description("Del VBucket")]
        [EnumName("Del VBucket")]
        DelVBucket = 0x3F,
        /// <summary>
        /// TAP Connect
        /// </summary>
        [Description("TAP Connect")]
        [EnumName("TAP Connect")]
        TAPConnect = 0x40,
    }
}