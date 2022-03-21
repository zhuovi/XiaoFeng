using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Json
{
    /// <summary>
    /// Json解析器
    /// Description:
    /// 2018-10-18
    /// 1.优化字符串开始也可以用'\''或'"'
    /// 2020-12-16
    /// 1.新增DataTable序列化与反序列化
    /// </summary>
    public static class JsonParser
    {
        #region 常量
        /// <summary>
        /// 对象容量
        /// </summary>
        private const int JsonObjectInitCapacity = 28;
        /// <summary>
        /// 数组容量
        /// </summary>
        private const int JsonArrayInitCapacity = 28;
        #endregion

        #region 全局属性
        /// <summary>
        /// 全局默认设置
        /// </summary>
        public static JsonSerializerSetting DefaultSettings = new JsonSerializerSetting();
        #endregion

        #region 转换Json对象
        /// <summary>
        /// 转换Json对象
        /// </summary>
        public static JsonValue Parse(string json)
        {
            return ParseValue(new JsonData(json));
        }
        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="json">json串</param>
        /// <returns></returns>
        public static JsonValue DeserializeObject(string json)
        {
            return DeserializeObject<JsonValue>(json);
        }
        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="json">json串</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string json)
        {
            using (var js = new JsonReader())
                return js.Read<T>(json);
        }
        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="json">json串</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object DeserializeObject(string json, Type type)
        {
            using (JsonReader js = new JsonReader())
                return js.Read(json, type);
        }
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="formatterSetting">Json格式设置</param>
        /// <returns></returns>
        public static string SerializeObject(object value, JsonSerializerSetting formatterSetting = null)
        {
            if (value.IsNullOrEmpty()) return "{}";
            if (value is string && value.ToString().IsMatch(@"^([^=&]+=[^&]*&?)+$"))
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                value.ToString().GetMatches(@"(?<a>[^=&]+)=(?<b>[^&]*)").Each(a =>
                {
                    var Key = a["a"];
                    var Value = a["b"];
                    if (!dic.ContainsKey(Key))
                        dic.Add(Key, Value);
                    else
                        dic[Key] = Value;
                });
                value = dic;
            }
            formatterSetting = formatterSetting ?? DefaultSettings;
            using (JsonWriter writer = new JsonWriter(formatterSetting))
            {
                writer.WriteValue(value);
                //Console.WriteLine("object:"+writer.ObjectTimes);
                var json = writer.Builder.ToString();
                //var ft = System.Diagnostics.Stopwatch.StartNew();
                //ft.Start();
                if (formatterSetting.Indented) json = writer.Format(json);
                //ft.Stop();
                //Console.WriteLine("format:"+ft.ElapsedMilliseconds);
                return json;
            }
        }
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="indented">是否格式化</param>
        /// <returns></returns>
        public static string SerializeObject(object value, bool indented)
        {
            return SerializeObject(value, new JsonSerializerSetting { Indented = indented });
        }
        /// <summary>
        /// 开始字符 开始字符是'"'或'\''
        /// </summary>
        private static char StartChar = '"';
        /// <summary>
        /// 转换Json对象
        /// </summary>
        public static JsonValue ParseValue(JsonData data)
        {
            SkipWhiteSpace(data);
            switch (data.JsonString[data.Index])
            {
                case '{':
                    return ParseObject(data);
                case '[':
                    return ParseArray(data);
                case '\'':
                    StartChar = '\'';
                    return ParseString(data);
                case '"':
                    StartChar = '"';
                    return ParseString(data);
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return ParseNumber(data);
                case 'F':
                case 'f':
                    if
                    (
                        (data.JsonString[data.Index + 1] == 'a' || data.JsonString[data.Index + 1] == 'A') &&
                        (data.JsonString[data.Index + 2] == 'l' || data.JsonString[data.Index + 2] == 'L') &&
                        (data.JsonString[data.Index + 3] == 's' || data.JsonString[data.Index + 3] == 'S') &&
                        (data.JsonString[data.Index + 4] == 'e' || data.JsonString[data.Index + 4] == 'E')
                    )
                    {
                        data.Index += 5;
                        return new JsonValue(false);
                    }
                    break;
                case 'T':
                case 't':
                    if
                    (
                        (data.JsonString[data.Index + 1] == 'r' || data.JsonString[data.Index + 1] == 'R') &&
                        (data.JsonString[data.Index + 2] == 'u' || data.JsonString[data.Index + 2] == 'U') &&
                        (data.JsonString[data.Index + 3] == 'e' || data.JsonString[data.Index + 3] == 'E')
                    )
                    {
                        data.Index += 4;
                        return new JsonValue(true);
                    }
                    break;
                case 'N':
                case 'n':
                    if
                    (
                        (data.JsonString[data.Index + 1] == 'u' || data.JsonString[data.Index + 1] == 'U') &&
                        (data.JsonString[data.Index + 2] == 'l' || data.JsonString[data.Index + 2] == 'L') &&
                        (data.JsonString[data.Index + 3] == 'l' || data.JsonString[data.Index + 3] == 'L')
                    )
                    {
                        data.Index += 4;
                        return new JsonValue();
                    }
                    break;
            }
            throw new JsonException
            (
                string.Format
                (
                    "Json ParseValue error on char '{0}' index at '{1}' ",
                    data.JsonString[data.Index],
                    data.Index
                )
            );
        }
        #endregion

        #region 转换成对象
        /// <summary>
        /// 转换成对象
        /// </summary>
        /// <param name="data">json数据</param>
        private static JsonValue ParseObject(JsonData data)
        {
            var jsonObject = new Dictionary<string, JsonValue>(JsonObjectInitCapacity);
            // 跳过 '{'
            data.Index++;
            do
            {
                SkipWhiteSpace(data);
                if (data.JsonString[data.Index] == '}')
                {
                    break;
                }
                DebugTool.Assert
                (
                    data.JsonString[data.Index] == '"' || data.JsonString[data.Index] == '\'',
                    "Json转换对象出错,字符 '{0}' 应该是 '\"'或'\'' ",
                    data.JsonString[data.Index]
                );
                // 跳过 '"' 或 '\''
                StartChar = data.JsonString[data.Index];
                data.Index++;
                var start = data.Index;
                while (true)
                {
                    char val = data.JsonString[data.Index++];
                    switch (val)
                    {
                        // 检测结尾 '\''
                        case '\'':
                        // 检测结尾 '"'
                        case '"':
                            if (val == StartChar)
                                break;
                            else
                                continue;
                        case '\\':
                            // 跳过转义
                            data.Index++;
                            continue;
                        default:
                            continue;
                    }
                    // 跳过结尾 '"'
                    break;
                }
                StartChar = '"';
                // 获取对象key
                var key = data.JsonString.Substring(start, data.Index - start - 1);
                SkipWhiteSpace(data);
                DebugTool.Assert
                (
                    data.JsonString[data.Index] == ':',
                    "Json转换对象出错,后一个key = {0}, 字符 '{1}' 应该是 ':' ",
                    key,
                    data.JsonString[data.Index]
                );
                // 跳过 ':'
                data.Index++;
                // 设置Json对象Key和Value
                jsonObject.Add(key, ParseValue(data));
                SkipWhiteSpace(data);
                if (data.JsonString[data.Index] == ',')
                {
                    data.Index++;
                }
                else
                {
                    DebugTool.Assert
                    (
                        data.JsonString[data.Index] == '}',
                        "Json转换对象出错,后一个key = {0}, 字符 '{1}' 应该是 '}' ",
                        key,
                        data.JsonString[data.Index]
                    );
                    break;
                }
            }
            while (true);
            // 跳过 '}' 返回最后的 '}'
            data.Index++;
            return new JsonValue(JsonType.Object, jsonObject);
        }
        #endregion

        #region 转换成数组
        /// <summary>
        /// 转换成数组
        /// </summary>
        /// <param name="data">json数据</param>
        private static JsonValue ParseArray(JsonData data)
        {
            var jsonArray = new List<JsonValue>(JsonArrayInitCapacity);
            // 跳过 '['
            data.Index++;
            do
            {
                SkipWhiteSpace(data);
                if (data.JsonString[data.Index] == ']')
                {
                    break;
                }
                //添加Json数组项 
                jsonArray.Add(ParseValue(data));
                SkipWhiteSpace(data);
                if (data.JsonString[data.Index] == ',')
                {
                    data.Index++;
                }
                else
                {
                    DebugTool.Assert
                    (
                        data.JsonString[data.Index] == ']',
                        "Json转换数组出错, 字符 '{0}' 应该是 ']' ",
                        data.JsonString[data.Index]
                    );
                    break;
                }
            }
            while (true);
            // 跳过 ']'
            data.Index++;
            return new JsonValue(JsonType.Array, jsonArray);
        }
        #endregion

        #region 转换成字符串
        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <param name="data">json数据</param>
        private static JsonValue ParseString(JsonData data)
        {
            // 跳过 '"'
            data.Index++;
            var start = data.Index;
            string str;
            while (true)
            {
                char val = data.JsonString[data.Index++];
                switch (val)
                {
                    // 检测字符串结尾 '\'' 
                    case '\'':
                    case '"':
                        //判断是否是开始字符
                        if (val != StartChar) continue;
                        if (data.Data.Length == 0)
                        {
                            // 没有转义字符就substring字符
                            str = data.JsonString.Substring(start, data.Index - start - 1);
                        }
                        else
                        {
                            str = data.Data.Append(data.JsonString, start, data.Index - start - 1).ToString();
                            // 清除下一个字符
                            data.Data.Length = 0;
                        }
                        break;
                    // 检查转义字符
                    case '\\':
                        {
                            var escapedIndex = data.Index;
                            char c;
                            switch (data.JsonString[data.Index++])
                            {
                                case '"':
                                    c = '"';
                                    break;
                                case '\\':
                                    c = '\\';
                                    break;
                                case '/':
                                    c = '/';
                                    break;
                                case '\'':
                                    c = '\'';
                                    break;
                                case 'b':
                                    c = '\b';
                                    break;
                                case 'f':
                                    c = '\f';
                                    break;
                                case 'n':
                                    c = '\n';
                                    break;
                                case 'r':
                                    c = '\r';
                                    break;
                                case 't':
                                    c = '\t';
                                    break;
                                case 'u':
                                    c = GetUnicodeCodePoint
                                        (
                                            data.JsonString[data.Index],
                                            data.JsonString[data.Index + 1],
                                            data.JsonString[data.Index + 2],
                                            data.JsonString[data.Index + 3]
                                        );
                                    // 跳过代码点
                                    data.Index += 4;
                                    break;
                                default:
                                    // 不支持只添加前字符串
                                    continue;
                            }
                            // 添加前字符和转义字符
                            data.Data.Append(data.JsonString, start, escapedIndex - start - 1).Append(c);
                            // 更新前字符串起始索引
                            start = data.Index;
                            continue;
                        }
                    default:
                        continue;
                }
                // 已经跳过了结束 '"'
                break;
            }
            JsonValue value;
            /*if (str.IsDateOrTime())
            {
                value.Type = JsonType.DateTime;
                value.Value = str.ToCast<DateTime>();
            }else if (str.isNumberic())
            {
                value.Type = JsonType.Number;
                value.Value = str.ToCast<int>();
            }else if (str.isFloat())
            {
                value.Type = JsonType.Float;
                value.Value = str.ToCast<float>();
            }else if (str.isGUID())
            {
                value.Type = JsonType.Guid;
                value.Value = str.ToCast<Guid>();
            }
            else*/
            if (str.Trim().IsMatch(@"[^,]+,\s*[^,]+,\s*Version=[\d\.]+,\s+Culture=[^,]+,\s*PublicKeyToken=[a-z0-9]*?"))
                value = new JsonValue(Type.GetType(str));
            /*else if (str.IsDateOrTime())
                value = new JsonValue(str.ToCast<DateTime>());
            else if (str.IsMatch(@"^\{?[a-z0-9]{8}(-)[a-z0-9]{4}\1[a-z0-9]{4}\1[a-z0-9]{4}\1[a-z0-9]{12}\}?$"))
                value = new JsonValue(str.ToCast<Guid>());*/
            else
                value = new JsonValue(str);
            StartChar = '"';
            return value;
        }
        #endregion

        #region 转换成数字
        /// <summary>
        /// 转换成数字
        /// </summary>
        /// <param name="data">json数据</param>
        private static JsonValue ParseNumber(JsonData data)
        {
            var start = data.Index;
            while (true)
            {
                switch (data.JsonString[++data.Index])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '-':
                    case '+':
                    case '.':
                    case 'e':
                    case 'E':
                        continue;
                }
                break;
            }
            var strNum = data.JsonString.Substring(start, data.Index - start);
            if (strNum.IsMatch(@"^(\+|-)?\d+[.]\d+$"))
            {
                if (float.TryParse(strNum, out float fValue))
                {
                    return new JsonValue(fValue);
                }
                else
                {
                    throw new JsonException("Json转换浮点,不能强转字符 [{0}]".format(strNum));
                }
            }
            else
            {
                if (long.TryParse(strNum, out long iValue))
                {
                    return new JsonValue(iValue);
                }
                else
                {
                    throw new JsonException("Json转换数字,不能强转字符 [{0}]".format(strNum));
                }
            }
        }
        #endregion

        #region 跳过空格回车串
        /// <summary>
        /// 跳过空格回车串
        /// </summary>
        /// <param name="data">json数据</param>
        private static void SkipWhiteSpace(JsonData data)
        {
            while (true)
            {
                switch (data.JsonString[data.Index])
                {
                    case (char)65279:
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        data.Index++;
                        continue;
                }
                break;
            }
        }
        #endregion

        #region 获取unicode代码
        /// <summary>
        /// 获取unicode代码
        /// </summary>
        /// <param name="c1">第一个字符</param>
        /// <param name="c2">第二个字符</param>
        /// <param name="c3">第三个字符</param>
        /// <param name="c4">第四个字符</param>
        private static char GetUnicodeCodePoint(char c1, char c2, char c3, char c4)
        {
            return (char)
                   (
                       UnicodeCharToInt(c1) * 0x1000 +
                       UnicodeCharToInt(c2) * 0x100 +
                       UnicodeCharToInt(c3) * 0x10 +
                       UnicodeCharToInt(c4)
                   );
        }
        #endregion

        #region 单个unicode字符转int
        /// <summary>
        /// 单个unicode字符转int
        /// </summary>
        /// <param name="c">字符</param>
        private static int UnicodeCharToInt(char c)
        {
            switch (c)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                case 'a':
                    return 10;
                case 'B':
                case 'b':
                    return 11;
                case 'C':
                case 'c':
                    return 12;
                case 'D':
                case 'd':
                    return 13;
                case 'E':
                case 'e':
                    return 14;
                case 'F':
                case 'f':
                    return 15;
            }
            throw new JsonException("Json Unicode 字符 '{0}' 出错".format(c));
        }
        #endregion
    }

    #region 调试
    /// <summary>
    /// 调试
    /// </summary>
    internal static class DebugTool
    {
        /// <summary>
        /// 断言问题
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="msg">消息</param>
        /// <param name="args">参数</param>
        public static void Assert(bool condition, string msg, params object[] args)
        {
            if (!condition)
            {
                throw new JsonException(string.Format(msg, args));
            }
        }
    }
    #endregion
}