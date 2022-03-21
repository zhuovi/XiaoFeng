using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Json
{
    /// <summary>
    /// Json数据
    /// </summary>
    public class JsonData
    {
        #region 构造器
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="jsonString">json串数据</param>
        public JsonData(string jsonString)
        {
            this.JsonString = jsonString;
        }
        #endregion

        #region 属性
        /// <summary>
        /// Json串
        /// </summary>
        private string _JsonString = string.Empty;
        /// <summary>
        /// Json串
        /// </summary>
        public string JsonString
        {
            get { return this._JsonString; }
            set
            {
                string _value = value;
                //_value = _value.StartsWith("\ufeff") ? _value.Substring(1) : _value;
                if (_value.IsJson())
                    _value = _value.RemovePattern(@"\/\*[\s\S]*?\*\/");
                else if (_value.IsQuery())
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    _value.GetMatches(@"(?<a>[^=&]+)=(?<b>[^&]*)").Each(a =>
                    {
                        if (!dic.ContainsKey(a["a"]))
                            dic.Add(a["a"], a["b"]);
                    });
                    //_value = "{\"" + _value.ReplacePattern("=", "\":\"").ReplacePattern("&", "\",\"") + "\"}";
                    _value = dic.ToJson();
                }
                this._JsonString = _value;
            }
        }
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; } = 0;
        /// <summary>
        /// 结果集
        /// </summary>
        public StringBuilder Data { get; set; } = new StringBuilder();
        #endregion
    }
}