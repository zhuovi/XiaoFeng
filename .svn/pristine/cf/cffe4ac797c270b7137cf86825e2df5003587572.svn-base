using System;
using XiaoFeng.Json;
#if (NETCORE && !NETSTANDARD)
using System.Text.Unicode;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
#endif
namespace XiaoFeng
{
    /// <summary>
    /// 扩展JSON对象
    /// </summary>
    public static partial class PrototypeHelper
    {
#region 对象转JSON串
        /// <summary>
        /// 对象转JSON串
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="formatting">Json格式设置</param>
        /// <returns></returns>
        public static string ToJson(this object o, JsonSerializerSetting formatting = null)
        {
            if (o == null) return string.Empty;
            try
            {
                return JsonParser.SerializeObject(o, formatting);
            }
            catch(Exception ex)
            {
                LogHelper.Error(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 对象转JSON串
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="indented">是否格式化</param>
        /// <returns></returns>
        public static string ToJson(this object o, bool indented)
        {
            return o.ToJson(new JsonSerializerSetting { Indented = indented });
        }
#endregion

#region JSON串转对象
        /// <summary>
        /// JSON串转对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="_">JSON串</param>
        /// <returns></returns>
        public static T JsonToObject<T>(this string _)
        {
            if (_.IsNullOrEmpty()) return default(T);
            try
            {
                return JsonParser.DeserializeObject<T>(_);
            }
            catch(Exception ex)
            {
                LogHelper.Error(ex, "JSON解析对象出错,JSON字符串为:" + _);
                return default(T);
            }
        }
        /// <summary>
        /// JSON串转对象
        /// </summary>
        /// <param name="_">JSON串</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object JsonToObject(this string _, Type type)
        {
            if (_.IsNullOrEmpty()) return null;
            try
            {
                return JsonParser.DeserializeObject(_, type);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "JSON解析对象出错,JSON字符串为:" + _);
                return Activator.CreateInstance(type);
            }
        }
        /// <summary>
        /// JSON串转对象
        /// </summary>
        /// <param name="_">JSON串</param>
        /// <returns></returns>
        public static JsonValue JsonToObject(this string _)
        {
            return _.JsonToObject<JsonValue>();
        }
#endregion
#if (NETCORE && !NETSTANDARD)

#region System.Text.Json对象转换
        /// <summary>
        /// 对象转JSON串
        /// </summary>
        /// <param name="o">对象</param>
        /// <param name="formatting">Json格式设置</param>
        /// <returns></returns>
        public static string EntityToJson(this object o, JsonSerializerSetting formatting = null)
        {
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs)
            };
            options.Converters.Add(new JsonConverterType());
            if (formatting != null)
            {
                if (formatting.DateTimeFormat.IsNotNullOrEmpty())
                    options.Converters.Add(new JsonConverterDateTime(formatting.DateTimeFormat));
                if (formatting.GuidFormat.IsNotNullOrEmpty())
                    options.Converters.Add(new JsonConverterGuid(formatting.GuidFormat));
                options.PropertyNameCaseInsensitive = formatting.IgnoreCase;
                options.WriteIndented = formatting.Indented;
                options.IgnoreNullValues = formatting.OmitEmptyNode;
                options.MaxDepth = formatting.MaxDepth;
            }
            return JsonSerializer.Serialize(o, options);
        }
        #endregion

        #region JSON串转对象
        /// <summary>
        /// JSON串转对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="_">JSON串</param>
        /// <param name="formatting">配置</param>
        /// <returns></returns>
        public static T JsonToEntity<T>(this string _, JsonSerializerSetting formatting = null)
        {
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs)
            };
            options.Converters.Add(new JsonConverterType());
            if (formatting != null)
            {
                if (formatting.DateTimeFormat.IsNotNullOrEmpty())
                    options.Converters.Add(new JsonConverterDateTime(formatting.DateTimeFormat));
                if (formatting.GuidFormat.IsNotNullOrEmpty())
                    options.Converters.Add(new JsonConverterGuid(formatting.GuidFormat));
                options.PropertyNameCaseInsensitive = formatting.IgnoreCase;
                options.WriteIndented = formatting.Indented;
                options.IgnoreNullValues = formatting.OmitEmptyNode;
                options.MaxDepth = formatting.MaxDepth;
            }
            return JsonSerializer.Deserialize<T>(_, options);
        }
        #endregion
#endif
    }
}