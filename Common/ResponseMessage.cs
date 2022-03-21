using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using XiaoFeng;
using XiaoFeng.Json;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using XiaoFeng.Web;
using HttpContext = XiaoFeng.Web.HttpContext;
using System.ComponentModel;

namespace XiaoFeng
{
    /// <summary>
    /// 输出消息
    /// Version : 1.0
    /// Create Date : 2016-12-23
    /// Author : jacky
    /// Site : www.zhuovi.com
    /// </summary>
    [DataContract]
    [XmlRoot("Root")]
    public class ResponseMessage<T>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ResponseMessage() : this(ResponseState.success, "", default(T)) { }
        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="message">信息</param>
        /// <param name="data">数据</param>
        public ResponseMessage(ResponseState state, string message = "", T data = default(T))
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
        [XmlElement("counts")]
        [Description("条数")]
        public int Counts { get; set; } = -1;
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

        #region 输出数据

        #region 输出JSON
        /// <summary>
        /// 输出JSON
        /// </summary>
        public void ToJSON() { this.Write(WriteType.JSON); }
        #endregion

        #region 输出XML
        /// <summary>
        /// 输出XML
        /// </summary>
        public void ToXML() { this.Write(WriteType.XML); }
        #endregion

        #region 输出成功
        /// <summary>
        /// 输出成功
        /// </summary>
        /// <param name="writeType">输出类型</param>
        public void Success(WriteType writeType = WriteType.JSON)
        {
            this.Status = ResponseState.success; this.Write(writeType);
        }
        /// <summary>
        /// 输出成功
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="writeType">输出类型</param>
        public void Success(T data, WriteType writeType = WriteType.JSON)
        {
            this.Data = data; this.Success(writeType);
        }
        #endregion

        #region 输出错误
        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="writeType">输出类型</param>
        public void Error(WriteType writeType = WriteType.JSON)
        {
            this.Status = ResponseState.error; this.Write(writeType);
        }
        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="writeType">输出类型</param>
        public void Error(string message, WriteType writeType = WriteType.JSON)
        {
            this.Message = message; this.Error(writeType);
        }
        #endregion

        #region 输出警告
        /// <summary>
        /// 输出警告
        /// </summary>
        /// <param name="WriteType">输出类型</param>
        public void Warning(WriteType WriteType = WriteType.JSON)
        {
            this.Status = ResponseState.warning; this.Write(WriteType);
        }
        /// <summary>
        /// 输出警告
        /// </summary>
        /// <param name="message">警告信息</param>
        /// <param name="writeType">输出类型</param>
        public void Warning(string message, WriteType writeType = WriteType.JSON)
        {
            this.Message = message; this.Warning(writeType);
        }
        #endregion

        #region 输出
        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="writeType">输出类型</param>
        public void Write(T data, WriteType writeType = WriteType.JSON)
        {
            this.Data = data; this.Write(writeType);
        }
        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="writeType">输出类型 默认 JSON ,XML,String</param>
        public void Write(WriteType writeType = WriteType.JSON)
        {
            var Response= HttpContext.Current.Response;
            //Response.Clear();
            //Response.ClearContent();
            //Response.ClearHeaders();
           // Response.ContentEncoding = this.Encoding;
            StringBuilder sbr = new StringBuilder();
            switch (writeType)
            {
                case WriteType.JSON:
                    sbr.Append(JsonParser.SerializeObject(this));
                    Response.ContentType = "application/json";
                    break;
                case WriteType.XML:
                    Response.ContentType = "text/xml";
                    if (this.Data.GetType() == typeof(DataTable))
                    {
                        if ((this.Data as DataTable).TableName.IsNullOrEmpty())
                            (this.Data as DataTable).TableName = "item";
                    }
                    sbr.Append(XmlConvert.SerializerObject(this, "UTF-8", true));
                    sbr.Append("\n<!--生成于[{0}]-->".format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                    break;
                case WriteType.String:
                    Response.ContentType = "text/plain";
                    sbr.Append(String.Format("state:{0}\nmessage:{1}", this.Status.ToString(), this.Message.ToString()));
                    break;
            }
            //Response.Body.Write()
            //Response.Write(sbr.ToString());
            //Response.Flush();
            //Response.End();
        }
        #endregion

        #endregion

        #region 返回序列化数据
        /// <summary>
        /// 返回序列化数据
        /// </summary>
        /// <param name="writeType">输出类型 默认 JSON ,XML,String</param>
        /// <returns></returns>
        public string GetData(WriteType writeType = WriteType.JSON)
        {
            switch (writeType)
            {
                case WriteType.JSON:
                    return JsonParser.SerializeObject(this);
                case WriteType.XML:
                    if (this.Data.GetType() == typeof(DataTable))
                    {
                        if ((this.Data as DataTable).TableName.IsNullOrEmpty())
                            (this.Data as DataTable).TableName = "item";
                    }
                    return XmlConvert.SerializerObject(this, "UTF-8", true);
                case WriteType.String:
                    return String.Format("state:{0}\nmessage:{1}", this.Status.ToString(), this.Message.ToString());
                default:
                    return "";
            }
        }
        #endregion

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