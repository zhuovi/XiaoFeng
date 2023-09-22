using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using XiaoFeng.Memcached.Attributes;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 11:39:52                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol
{
    /// <summary>
    /// 命令类型
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// 获取key的value值，若key不存在，返回空，支持多个key。
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="get &lt;key&gt;*"/></para>
        /// </remarks>
        [Description("获取key的value值，若key不存在，返回空。支持多个key")]
        [ProtocolBinary, ProtocolText]
        [EnumName("get")]
        [CommandGroup(CommandFlags.Get)]
        Get = 0x00,
        /// <summary>
        /// 给key设置一个值。
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="set &lt;key&gt; &lt;flags&gt; &lt;exptime> &lt;bytes&gt; [noreply]"/></para>
        /// </remarks>
        [Description("给key设置一个值")]
        [ProtocolBinary, ProtocolText]
        [CommandGroup(CommandFlags.Store)]
        Set = 0x01,
        /// <summary>
        /// 如果key不存在的话，就添加。
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="add &lt;key&gt; &lt;flags&gt; &lt;exptime> &lt;bytes&gt; [noreply]"/></para>
        /// </remarks>
        [Description("如果key不存在的话，就添加")]
        [ProtocolBinary, ProtocolText]
        [CommandGroup(CommandFlags.Store)]
        Add = 0x02,
        /// <summary>
        /// 用来替换已知key的value
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="replace &lt;key&gt; &lt;flags&gt; &lt;exptime> &lt;bytes&gt; [noreply]"/></para>
        /// </remarks>
        [Description("用来替换已知key的value")]
        [ProtocolBinary, ProtocolText]
        [CommandGroup(CommandFlags.Store)]
        Replace = 0x03,
        /// <summary>
        /// 删除Key
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="delete &lt;key&gt; [noreply]"/></para>
        /// </remarks>
        [Description("删除Key")]
        [EnumName("delete")]
        [ProtocolBinary, ProtocolText]
        [CommandGroup(CommandFlags.Get)]
        Delete = 0x04,
        /// <summary>
        /// 递增
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="incr &lt;key&gt; &lt;increment_value&gt; [noreply]"/></para>
        /// </remarks>
        [Description("递增")]
        [EnumName("Incr")]
        [ProtocolBinary, ProtocolText]
        [CommandGroup(CommandFlags.Get)]
        Increment = 0x05,
        /// <summary>
        /// 递减
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="decr &lt;key&gt; &lt;increment_value&gt; [noreply]"/></para>
        /// </remarks>
        [Description("递减")]
        [EnumName("Decr")]
        [ProtocolBinary, ProtocolText]
        [CommandGroup(CommandFlags.Get)]
        Decrement = 0x06,
        /// <summary>
        /// 退出
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="quit"/></para>
        /// </remarks>
        [Description("退出")]
        [EnumName("quit")]
        [ProtocolBinary, ProtocolText]
        Quit = 0x07,
        /// <summary>
        /// 用于清理缓存中的所有 key=>value(键=>值) 对
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="flush_all [time] [noreply]"/></para>
        /// </remarks>
        [Description("用于清理缓存中的所有 key=>value(键=>值) 对")]
        [EnumName("flush_all")]
        [ProtocolBinary, ProtocolText]
        [CommandGroup(CommandFlags.Stats)]
        FlushAll = 0x08,
        /// <summary>
        /// GetQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("GetQ")]
        [EnumName("GetQ")]
        [ProtocolBinary]
        GetQ = 0x09,
        /// <summary>
        /// No-op
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="mn"/> 返回结果为:MN\r\n</para>
        /// </remarks>
        [Description("No-op")]
        [EnumName("No-op")]
        [ProtocolBinary, ProtocolText]
        Noop = 0x0A,
        /// <summary>
        /// 版本
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="version"/></para>
        /// </remarks>
        [Description("版本")]
        [EnumName("version")]
        [ProtocolBinary, ProtocolText]
        Version = 0x0B,
        /// <summary>
        /// GetK
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("GetK")]
        [EnumName("GetK")]
        [ProtocolBinary]
        GetK = 0x0C,
        /// <summary>
        /// GetKQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("GetKQ")]
        [EnumName("GetKQ")]
        [ProtocolBinary]
        GetKQ = 0x0D,
        /// <summary>
        /// 表示将提供的值附加到现有key的value之后，是一个附加操作
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="append &lt;key&gt; &lt;flags&gt; &lt;exptime> &lt;bytes&gt; [noreply]"/></para>
        /// </remarks>
        [Description("表示将提供的值附加到现有key的value之后，是一个附加操作")]
        [ProtocolBinary, ProtocolText]
        [CommandGroup(CommandFlags.Store)]
        Append = 0x0E,
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="prepend &lt;key&gt; &lt;flags&gt; &lt;exptime> &lt;bytes&gt; [noreply]"/></para>
        /// </remarks>
        [Description("将此数据添加到现有数据之前的现有键中")]
        [ProtocolBinary, ProtocolText]
        [CommandGroup(CommandFlags.Store)]
        Prepend = 0x0F,
        /// <summary>
        /// 返回统计信息
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="stats"/></para>
        /// </remarks>
        [Description("返回统计信息")]
        [ProtocolBinary, ProtocolText]
        [EnumName("stats")]
        [CommandGroup(CommandFlags.Stats)]
        Stats = 0x10,
        /// <summary>
        /// SetQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("SetQ")]
        [EnumName("SetQ")]
        [ProtocolBinary]
        SetQ = 0x11,
        /// <summary>
        /// AddQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("AddQ")]
        [EnumName("AddQ")]
        [ProtocolBinary]
        AddQ = 0x12,
        /// <summary>
        /// ReplaceQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("ReplaceQ")]
        [EnumName("ReplaceQ")]
        [ProtocolBinary]
        ReplaceQ = 0x13,
        /// <summary>
        /// DeleteQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("DeleteQ")]
        [EnumName("DeleteQ")]
        [ProtocolBinary]
        DeleteQ = 0x14,
        /// <summary>
        /// IncrementQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("IncrementQ")]
        [EnumName("IncrementQ")]
        [ProtocolBinary]
        IncrementQ = 0x15,
        /// <summary>
        /// DecrementQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("DecrementQ")]
        [EnumName("DecrementQ")]
        [ProtocolBinary]
        DecrementQ = 0x16,
        /// <summary>
        /// QuitQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("QuitQ")]
        [EnumName("QuitQ")]
        [ProtocolBinary]
        QuitQ = 0x17,
        /// <summary>
        /// FlushQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("FlushQ")]
        [EnumName("FlushQ")]
        [ProtocolBinary]
        FlushQ = 0x18,
        /// <summary>
        /// AppendQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("AppendQ")]
        [EnumName("AppendQ")]
        [ProtocolBinary]
        AppendQ = 0x19,
        /// <summary>
        /// PrependQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("PrependQ")]
        [EnumName("PrependQ")]
        [ProtocolBinary]
        PrependQ = 0x1A,
        /// <summary>
        /// Verbosity
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="verbosity"/></para>
        /// </remarks>
        [Description("Verbosity")]
        [EnumName("Verbosity")]
        [ProtocolBinary, ProtocolText]
        Verbosity = 0x1B,
        /// <summary>
        /// 修改key过期时间
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="touch &lt;key&gt; &lt;exptime> [noreply]"/></para>
        /// </remarks>
        [Description("修改key过期时间")]
        [EnumName("touch")]
        [ProtocolBinary, ProtocolText]
        [CommandGroup(CommandFlags.Get)]
        Touch = 0x1C,
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <remarks>
        /// <para>同时支持 <see langword="Binary"/> , <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="gat &lt;exptime> &lt;key&gt;*"/></para>
        /// </remarks>
        [Description("获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间")]
        [ProtocolBinary, ProtocolText]
        [EnumName("gat")]
        [CommandGroup(CommandFlags.Get)]
        Gat = 0x1D,
        /// <summary>
        /// GATQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("GATQ")]
        [EnumName("GATQ")]
        [ProtocolBinary]
        GATQ = 0x1E,
        /// <summary>
        /// SASL list mechs
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("SASL list mechs")]
        [EnumName("SASL list mechs")]
        [ProtocolBinary]
        SASLListMechs = 0x20,
        /// <summary>
        /// SASL Auth
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("SASL Auth")]
        [EnumName("SASL Auth")]
        [ProtocolBinary]
        SASLAuth = 0x21,
        /// <summary>
        /// SASL Step
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("SASL Step")]
        [EnumName("SASL Step")]
        [ProtocolBinary]
        SASLStep = 0x22,
        /// <summary>
        /// RGet
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RGet")]
        [EnumName("RGet")]
        [ProtocolBinary]
        RGet = 0x30,
        /// <summary>
        /// RSet
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RSet")]
        [EnumName("RSet")]
        [ProtocolBinary]
        RSet = 0x31,
        /// <summary>
        /// RSetQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RSetQ")]
        [EnumName("RSetQ")]
        [ProtocolBinary]
        RSetQ = 0x32,
        /// <summary>
        /// RAppend
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RAppend")]
        [EnumName("RAppend")]
        [ProtocolBinary]
        RAppend = 0x33,
        /// <summary>
        /// RAppendQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RAppendQ")]
        [EnumName("RAppendQ")]
        [ProtocolBinary]
        RAppendQ = 0x34,
        /// <summary>
        /// RPrepend
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RPrepend")]
        [EnumName("RPrepend")]
        [ProtocolBinary]
        RPrepend = 0x35,
        /// <summary>
        /// RPrependQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RPrependQ")]
        [EnumName("RPrependQ")]
        [ProtocolBinary]
        RPrependQ = 0x36,
        /// <summary>
        /// RDelete
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RDelete")]
        [EnumName("RDelete")]
        [ProtocolBinary]
        RDelete = 0x37,
        /// <summary>
        /// RDeleteQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RDeleteQ")]
        [EnumName("RDeleteQ")]
        [ProtocolBinary]
        RDeleteQ = 0x38,
        /// <summary>
        /// RIncr
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RIncr")]
        [EnumName("RIncr")]
        [ProtocolBinary]
        RIncr = 0x39,
        /// <summary>
        /// RIncrQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RIncrQ")]
        [EnumName("RIncrQ")]
        [ProtocolBinary]
        RIncrQ = 0x3A,
        /// <summary>
        /// RDecr
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RDecr")]
        [EnumName("RDecr")]
        [ProtocolBinary]
        RDecr = 0x3B,
        /// <summary>
        /// RDecrQ
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("RDecrQ")]
        [EnumName("RDecrQ")]
        [ProtocolBinary]
        RDecrQ = 0x3C,
        /// <summary>
        /// Set VBucket
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("Set VBucket")]
        [EnumName("Set VBucket")]
        [ProtocolBinary]
        SetVBucket = 0x3D,
        /// <summary>
        /// Get VBucket
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("Get VBucket")]
        [EnumName("Get VBucket")]
        [ProtocolBinary]
        GetVBucket = 0x3E,
        /// <summary>
        /// Del VBucket
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("Del VBucket")]
        [EnumName("Del VBucket")]
        [ProtocolBinary]
        DelVBucket = 0x3F,
        /// <summary>
        /// TAP Connect
        /// </summary>
        /// <remarks>
        /// 仅支持 <see langword="Binary"/> 传输协议
        /// </remarks>
        [Description("TAP Connect")]
        [EnumName("TAP Connect")]
        [ProtocolBinary]
        TAPConnect = 0x40,
        /*
         * 下边为自定义数据 主要应用于Text通讯
         */
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key
        /// </summary>
        /// <remarks>
        /// <para>仅支持 <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="gets &lt;key&gt;*"/></para>
        /// </remarks>
        [Description("用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key")]
        [ProtocolText]
        [EnumName("gets")]
        [CommandGroup(CommandFlags.Get)]
        Gets = 0x90,
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        /// <remarks>
        /// <para>仅支持 <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="gats &lt;exptime> &lt;key&gt;*"/></para>
        /// </remarks>
        [Description("用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间")]
        [ProtocolText]
        [CommandGroup(CommandFlags.Get)]
        Gats = 0x91,
        /// <summary>
        /// 一个原子操作，只有当casunique匹配的时候，才会设置对应的值
        /// </summary>
        /// <remarks>
        /// <para>仅支持 <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="cas &lt;key&gt; &lt;flags&gt; &lt;exptime> &lt;bytes&gt; unique_cas_token [noreply]"/></para>
        /// </remarks>
        [Description("一个原子操作，只有当casunique匹配的时候，才会设置对应的值")]
        [ProtocolText]
        [CommandGroup(CommandFlags.Store)]
        Cas = 0x92,
        /// <summary>
        /// 显示各个 slab 中 item 的数目和存储时长(最后一次访问距离现在的秒数)
        /// </summary>
        /// <remarks>
        /// <para>仅支持 <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="stats items"/></para>
        /// </remarks>
        [Description("显示各个 slab 中 item 的数目和存储时长")]
        [EnumName("stats items")]
        [ProtocolText]
        [CommandGroup(CommandFlags.Stats)]
        StatsItems = 0x93,
        /// <summary>
        /// 显示各个slab的信息，包括chunk的大小、数目、使用情况等
        /// </summary>
        /// <remarks>
        /// <para>仅支持 <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="stats slabs"/></para>
        /// </remarks>
        [Description("显示各个slab的信息，包括chunk的大小、数目、使用情况等")]
        [EnumName("stats slabs")]
        [ProtocolText]
        [CommandGroup(CommandFlags.Stats)]
        StatsSlabs = 0x94,
        /// <summary>
        /// 显示所有item的大小和个数
        /// </summary>
        /// <remarks>
        /// <para>仅支持 <see langword="Text"/> 传输协议。</para>
        /// <para><see langword="Text"/> 传输协议命令行: <see langword="stats sizes"/></para>
        /// </remarks>
        [Description("显示所有item的大小和个数")]
        [EnumName("stats sizes")]
        [ProtocolText]
        [CommandGroup(CommandFlags.Stats)]
        StatsSizes = 0x95
    }
}