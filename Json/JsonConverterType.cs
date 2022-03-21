using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if (NETCORE && !NETSTANDARD)
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XiaoFeng.Json
{
    /// <summary>
    /// 类型转换器
    /// </summary>
    public class JsonConverterType : JsonConverter<Type>
    {
        /// <summary>
        /// 读JSON
        /// </summary>
        /// <param name="reader">JsonReader</param>
        /// <param name="typeToConvert">类型</param>
        /// <param name="options">配置</param>
        /// <returns></returns>
        public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Type.GetType(reader.GetString());
        }
        /// <summary>
        /// 写JSON
        /// </summary>
        /// <param name="writer">JsonWriter</param>
        /// <param name="value">值</param>
        /// <param name="options">配置</param>
        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.AssemblyQualifiedName);
        }
    }
}
#endif