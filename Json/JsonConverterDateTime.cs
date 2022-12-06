using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-25 11:59:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
#if (NETCORE && !NETSTANDARD)
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XiaoFeng.Json
{
    /// <summary>
    /// 日期转换器
    /// </summary>
    public class JsonConverterDateTime : JsonConverter<DateTime>
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public JsonConverterDateTime() { this.Format = "yyyy-MM-dd HH:mm:ss.ffffff"; }
        /// <summary>
        /// 设置格式
        /// </summary>
        /// <param name="format">格式</param>
        public JsonConverterDateTime(string format)
        {
            this.Format = format;
        }
        /// <summary>
        /// 格式
        /// </summary>
        private string Format { get; set; }
        /// <summary>
        /// 读JSON
        /// </summary>
        /// <param name="reader">JsonReader</param>
        /// <param name="typeToConvert">类型</param>
        /// <param name="options">配置</param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            return reader.GetString().ToCast<DateTime>();
        }
        /// <summary>
        /// 写JSON
        /// </summary>
        /// <param name="writer">JsonWriter</param>
        /// <param name="value">值</param>
        /// <param name="options">配置</param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}
#endif