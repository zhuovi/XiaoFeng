﻿using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using XiaoFeng.Json;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-08 10:43:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 输出消息
    /// Version : 1.0
    /// Create Date : 2016-12-23
    /// </summary>
    [DataContract]
    [XmlRoot("Root")]
    public class ResponseMessage<T>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ResponseMessage() : this(ResponseState.success, "", default) { }
        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="message">信息</param>
        /// <param name="data">数据</param>
        public ResponseMessage(ResponseState state, string message = "", T data = default)
        {
            this.Status = state; this.Data = data; this.Message = message;
        }
        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="data">数据</param>
        /// <param name="counts">条数</param>
        /// <param name="other">其它数据</param>
        public ResponseMessage(ResponseState state, T data, int counts, object other = null)
        {
            this.Status = state; this.Data = data; this.Counts = counts;
            this.Other = other;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 状态
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [Xml.XmlConverter(typeof(Xml.StringEnumConverter))]
        [DataMember]
        [JsonElement("status")]
        [XmlElement("status")]
        [Description("状态")]
        public ResponseState Status { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        [JsonElement("message")]
        [XmlElement("message")]
        [Description("消息")]
        public string Message { get; set; }
        /// <summary>
        /// 数据 如果是DataTable 一定要有TableName值
        /// </summary>
        [DataMember]
        [JsonElement("data")]
        [XmlElement("data")]
        [Description("数据")]
        public T Data { get; set; }
        /// <summary>
        /// 备用数据字段
        /// </summary>
        [DataMember]
        [JsonElement("other")]
        [OmitEmptyNode]
        [XmlElement("other")]
        [Description("备用数据")]
        public object Other { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        private int _code = -1;
        /// <summary>
        /// 状态码
        /// </summary>
        [DataMember]
        [JsonElement("code")]
        [XmlElement("code")]
        [Description("状态码")]
        public int Code
        {
            get { return this._code == -1 ? (int)Status : this._code; }
            set { this._code = value; }
        }
        /// <summary>
        /// 条数
        /// </summary>
        [DataMember]
        [JsonElement("counts")]
        [OmitEmptyNode]
        [XmlElement("counts")]
        [Description("条数")]
        public int? Counts { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        [DataMember]
        [JsonElement("time")]
        [XmlElement("time")]
        [Description("时间")]
        public int Time { get { return DateTime.Now.ToTimeStamp(); } }
        /// <summary>
        /// 输出编码
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        #endregion

        #region 方法
        /// <summary>
        /// 输出成功数据
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">数据</param>
        /// <param name="counts">条目</param>
        /// <param name="other">其它数据</param>
        /// <returns></returns>
        public static ResponseMessage<T> Success(string message, T data, int? counts=null, object other=null)
        {
            return new ResponseMessage<T>()
            {
                Status = ResponseState.success,
                Message = message,
                Data = data,
                Counts = counts,
                Other = other
            };
        }
        /// <summary>
        /// 输出成功数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="counts">条目</param>
        /// <param name="other">其它数据</param>
        /// <returns></returns>
        public static ResponseMessage<T> Success(T data, int? counts = null, object other = null)
        {
            return new ResponseMessage<T>()
            {
                Status = ResponseState.success,
                Data = data,
                Counts = counts,
                Other = other
            };
        }
        /// <summary>
        /// 输出错误数据
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static ResponseMessage<T> Error(string message, T data)
        {
            return new ResponseMessage<T>
            {
                Status = ResponseState.error,
                Message = message,
                Data = data
            };
        }
        /// <summary>
        /// 输出警告数据
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static ResponseMessage<T> Warn(string message, T data)
        {
            return new ResponseMessage<T>
            {
                Status = ResponseState.warning,
                Message = message,
                Data = data
            };
        }
        #endregion
    }

    #region 输出消息
    /// <summary>
    /// 输出消息
    /// Version : 1.0
    /// Create Date : 2016-12-23
    /// Author : jacky
    /// Site : www.zhuovi.com
    /// </summary>
    [Serializable]
    [XmlRoot("Root")]
    public class ResponseMessage : ResponseMessage<string>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ResponseMessage() : this(ResponseState.success, "", "") { }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="message">消息</param>
        /// <param name="data">数据</param>
        public ResponseMessage(ResponseState state, string message = "", string data = "") : base(state, message, data) { }
        #endregion
    }
    #endregion
}