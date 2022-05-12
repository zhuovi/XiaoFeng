using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using XiaoFeng.Json;
using XiaoFeng.Model;

namespace XiaoFeng
{
    /// <summary>
    /// 实体基础类
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class EntityBase<T> : EntityBase, IEntityBase where T : new()
    {
        #region 通过字段名获取相应的值
        /* <summary>
        /// 通过字段名获取相应的值
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        [JsonIgnore]
        [XmlIgnore]
        public virtual object this[string name]
        {
            get
            {
                if (name.IsNullOrEmpty()) return null;
                var ms = typeof(T).GetMember(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
                if (ms == null || ms.Length == 0) return null;
                var m = ms[0];
                if (m.MemberType == MemberTypes.Property)
                {
                    var p = m as PropertyInfo;
                    if (p.IsIndexer()) return null;
                    return p.CanRead ? p.GetValue(this, null) : null;
                }
                else if (m.MemberType == MemberTypes.Field)
                {
                    var f = m as FieldInfo;
                    return f.GetValue(this);
                }
                return null;
            }
            set
            {
                if (name.IsNullOrEmpty()) return;
                var ms = typeof(T).GetMember(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
                if (ms == null || ms.Length == 0) return;
                var m = ms[0];
                if (m.MemberType == MemberTypes.Property)
                {
                    var p = m as PropertyInfo;
                    if (p.IsIndexer()) return;
                    if (p.CanWrite) p.SetValue(this, value.GetValue(p.PropertyType));
                }
                else if (m.MemberType == MemberTypes.Field)
                {
                    var f = m as FieldInfo;
                    f.SetValue(this, value.GetValue(f.FieldType));
                }
            }
        }*/
        #endregion
    }

    #region 定义属性值改变委托
    /// <summary>
    /// 定义属性值改变委托
    /// </summary>
    /// <param name="name">属性名称</param>
    /// <param name="oldValue">老值</param>
    /// <param name="newValue">新值</param>
    public delegate void ValueChange(string name, object oldValue, object newValue);
    #endregion

    #region 实体基础类
    /// <summary>
    /// 实体基础类
    /// </summary>
    public abstract class EntityBase : Disposable, IEntityBase
    {
        #region 定义属性值改变委托
        /// <summary>
        /// 委托事件
        /// </summary>
        public event ValueChange OnValueChange;
        #endregion

        #region 通过字段名获取相应的值
        /// <summary>
        /// 移除值
        /// </summary>
        /// <param name="key">key值</param>
        public void RemoveAllValues(string key)
        {
            if (this.AllValues == null) return;
            if (this.AllValues.ContainsKey(key))
                this.AllValues.Remove(key);
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        public void SetAllValues(string key, object value)
        {
            if (this.AllValues == null) this.AllValues = new Dictionary<string, object>();
            if (this.AllValues.ContainsKey(key))
                this.AllValues[key] = value;
            else this.AllValues.Add(key, value);
        }
        /// <summary>
        /// 键名与值字典
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        private Dictionary<string, object> AllValues = null;
        /// <summary>
        /// 通过字段名获取相应的值
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public virtual object this[string name]
        {
            get
            {
                if (name.IsNullOrEmpty()) return null;
                if (this.AllValues == null) this.AllValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                if (this.AllValues.ContainsKey(name))
                    return this.AllValues[name];
                else
                {
                    object value = null;
                    var ms = this.GetType().GetMember(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (ms == null || ms.Length == 0) return null;
                    var m = ms[0];
                    if (m.MemberType == MemberTypes.Property)
                    {
                        var p = m as PropertyInfo;
                        if (p.IsIndexer()) return null;
                        value = p.CanRead ? p.GetValue(this, null) : null;
                    }
                    else if (m.MemberType == MemberTypes.Field)
                    {
                        var f = m as FieldInfo;
                        value = f.GetValue(this);
                    }
                    this.SetAllValues(name, value);
                    if (value == null && value is String v) value = String.Empty;
                    return value;
                }
            }
            set
            {
                if (name.IsNullOrEmpty()) return;
                var ms = this.GetType().GetMember(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (ms == null || ms.Length == 0) return;
                var m = ms[0];
                if (m.MemberType == MemberTypes.Property)
                {
                    var p = m as PropertyInfo;
                    if (p.IsIndexer()) return;
                    object val = value.GetValue(p.PropertyType);
                    if (p.CanWrite) p.SetValue(this, val);
                    this.SetAllValues(name, val);
                }
                else if (m.MemberType == MemberTypes.Field)
                {
                    var f = m as FieldInfo;
                    var val = value.GetValue(f.FieldType);
                    f.SetValue(this, val);
                    this.SetAllValues(name, val);
                }
            }
        }
        #endregion

        #region 对象所有属性名集合
        /// <summary>
        /// 对象所有属性名集合
        /// </summary>
        private List<string> _AllKeys = null;
        /// <summary>
        /// 对象所有属性名集合
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public List<string> AllKeys
        {
            get
            {
                if (this._AllKeys == null)
                {
                    this._AllKeys = new List<string>();
                    var ms = this.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (ms == null || ms.Length == 0) return null;
                    ms.Each(m =>
                    {
                        if (m.Name.IsMatch(@"(AllKeys|AllKeys)")) return;
                        if (m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)
                        {
                            if (m.MemberType == MemberTypes.Property)
                            {
                                var p = m as PropertyInfo;
                                if (p.IsIndexer()) return;
                            }
                            this._AllKeys.Add(m.Name);
                        }
                    });
                }
                return this._AllKeys;
            }
        }
        #endregion

        #region 脏数据
        /// <summary>
        /// 脏数据 
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        internal DirtyCollection Dirtys { get; set; } = new DirtyCollection();
        #endregion

        #region 获取脏数据
        /// <summary>
        /// 获取脏数据
        /// </summary>
        /// <returns></returns>
        public virtual DirtyCollection GetDirty()
        {
            return this.Dirtys;
        }
        #endregion

        #region 添加脏数据
        /// <summary>
        /// 添加脏数据
        /// </summary>
        /// <param name="fieldName">字段名</param>
        public virtual void AddDirty(string fieldName)
        {
            if (fieldName.IsNullOrEmpty()) return;
            this.Dirtys.Add(fieldName);
            this.RemoveAllValues(fieldName);
            this.OnValueChange?.Invoke(fieldName, null, null);
        }
        /// <summary>
        /// 添加脏数据
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="oldValue">老值</param>
        /// <param name="newValue">新值</param>
        public virtual void AddDirty(string fieldName, object oldValue, object newValue)
        {
            if (fieldName.IsNullOrEmpty()) return;
            this.AddDirty(fieldName);
            this.SetAllValues(fieldName, newValue);
            this.OnValueChange?.Invoke(fieldName, oldValue, newValue);
        }
        /// <summary>
        /// 设置脏数据
        /// </summary>
        /// <param name="dirty">脏数据</param>
        public virtual void SetDirty(DirtyCollection dirty) => this.Dirtys = dirty;
        #endregion

        #region 清理脏数据
        /// <summary>
        /// 清理脏数据 字段名为空则清空所有脏数据
        /// </summary>
        public virtual void ClearDirty(string fieldName = "")
        {
            if (fieldName.IsNullOrEmpty())
                this.Dirtys.Clear();
            else
                this.Dirtys.Remove(fieldName);
        }
        /// <summary>
        /// 清理脏数据
        /// </summary>
        /// <param name="fieldName">字段</param>
        public virtual void Remove(string fieldName) => this.Dirtys.Remove(fieldName);
        /// <summary>
        /// 清理脏数据
        /// </summary>
        /// <param name="list">数据</param>
        public virtual void Remove(List<string> list)
        {
            if (list == null || list.Count == 0) return;
            list.Each(a => this.Dirtys.Remove(a));
        }
        #endregion

        #region 是否存在于脏数据中
        /// <summary>
        /// 是否存在于脏数据中
        /// </summary>
        /// <param name="name">键名</param>
        /// <returns></returns>
        public virtual Boolean ContainsDirty(string name) => this.Dirtys.Contains(name);
        #endregion

        #region 转Json
        /// <summary>
        /// 转Json
        /// </summary>
        /// <param name="formatting">Json格式设置</param>
        /// <returns></returns>
        public virtual string ToJSON(JsonSerializerSetting formatting = null) => this.ToJson(formatting);
        /// <summary>
        /// 转Json
        /// </summary>
        /// <param name="indented">是否格式化</param>
        /// <returns></returns>
        public virtual string ToJSON(bool indented) => this.ToJson(indented);
        #endregion

        #region 转Xml
        /// <summary>
        /// 转Xml
        /// </summary>
        /// <param name="encode">编码</param>
        /// <param name="removeNamespaces">是否移除命名空间</param>
        /// <param name="removeXmlDeclaration">是否移除XML声明</param>
        /// <returns></returns>
        public virtual string ToXML(string encode = "UTF-8", Boolean removeNamespaces = false, Boolean removeXmlDeclaration = false) => this.EntityToXml(Encoding.GetEncoding(encode), removeXmlDeclaration);
        #endregion

        #region 数据库映射属性
        /// <summary>
        /// 数据库映射属性
        /// </summary>
        public static Type DataMappingType { get; set; }
        #endregion
    }
    #endregion
}