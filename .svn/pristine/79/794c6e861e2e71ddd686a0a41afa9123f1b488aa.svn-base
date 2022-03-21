using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XiaoFeng.Validator
{
    /// <summary>
    /// 验证参数
    /// </summary>
    public class Condition
    {
        #region 验证结果
        /// <summary>
        /// 验证结果
        /// </summary>
        public List<string> Result { get; set; } = new List<string>();
        /// <summary>
        /// 是否验证
        /// </summary>
        public Boolean IsValid { get { return this.Result.Count == 0; } }
        #endregion

        #region 验证参数
        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="format">规则</param>
        /// <returns></returns>
        public ConditionValidator Requires(string name, object value, ValidateFormat format = null)
        {
            var Validator = new ConditionValidator(name, value, format);
            this.Result.AddRange(Validator.Result);
            return Validator;
        }
        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="func">Lambda表达式</param>
        /// <returns></returns>
        public ConditionValidator Requires(string name, object value, Expression<Func<ValidatorFormat, bool>> func = null)
        {
            if (name.IsNullOrEmpty() || value == null) return new ConditionValidator();
            if (func != null)
            {
                string _value = func.Body.ToString().ReplacePattern(@".Equals\(", " == ");
                List<Dictionary<string, string>> list = _value.GetMatches(@"\.(?<a>[a-z]+)(\s*==\s*(?<b>[\sa-z0-9\,\(""]+))?");
                Dictionary<string, string> data = new Dictionary<string, string>();
                list.Each(a =>
                {
                    if (!data.ContainsKey(a["a"]))
                    {
                        string _key = a["a"], _val = a["b"].Trim('"').RemovePattern(@"(new BetweenValue|Convert)\(");
                        if (_key.IsMatch(@"^Is[a-z]+$"))
                        {
                            if (_key == "IsPattern")
                                _val = _value.GetMatch(@"IsPattern == ""(?<a>.*?)""(\)[\s\)]?)");
                            else
                            {
                                if (_value.IsMatch(@"\s+Not\([a-z]+\." + _key + @"+\)"))
                                    _val = "False";
                                else if (_val.IsNullOrEmpty()) _val = "True";
                            }
                        }
                        data.Add(_key, _val);
                    }
                });
                Type ValidatorType = Type.GetType("XiaoFeng.Validator.ConditionValidator");
                Type FormatType = Type.GetType("XiaoFeng.Validator.ValidateFormat");
                object format = Activator.CreateInstance(FormatType);
                data.Each(d =>
                {
                    PropertyInfo p = FormatType.GetProperty(d.Key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                    string val = d.Value;
                    object o = null;
                    if (val.IsNullOrEmpty())
                        o = null;
                    else if (d.Key == "Between")
                    {
                        string[] _val = val.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        Type between = Type.GetType("XiaoFeng.Validator.BetweenValue");
                        o = Activator.CreateInstance(between, new object[] { _val[0].Trim().ToDouble(), _val[1].Trim().ToDouble() });
                    }
                    else
                    {
                        Type _t = p.PropertyType;
                        if (_t.IsGenericType && _t.GetGenericTypeDefinition() == typeof(Nullable<>)) _t = _t.GetGenericArguments()[0];
                        o = Convert.ChangeType(val, _t);
                    }
                    p.SetValue(format, o);
                });
                object Validator = Activator.CreateInstance(ValidatorType, new object[] { name, value, format });
                PropertyInfo _Result = ValidatorType.GetProperty("Result", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                object result = _Result.GetValue(Validator, null);
                this.Result.AddRange(result as List<string>);
                return Validator as ConditionValidator;
            }
            return new ConditionValidator();
        }
        #endregion
    }
}