using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XiaoFeng.Config;
using XiaoFeng.Data;
using XiaoFeng.Data.SQL;
using XiaoFeng.Expressions;
using XiaoFeng.Json;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Model
{
    /// <summary>
    /// 实体类基础类
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public abstract class Entity<T> : EntityBase, IEntity<T> where T : Entity<T>, new()
    {
        #region 构造器
        /// <summary>
        /// 构造器
        /// </summary>
        protected Entity()
        {
            this.ModelType = typeof(T);
            TableAttribute table = this.ModelType.GetTableAttribute();
            this.TableAttr = table;
            if (table != null)
            {
                this.TableType = table.ModelType;
                this.DataBaseName = table.ConnName;
                this.DataBaseNum = (uint)table.ConnIndex;
                /*判断有无数据库映射*/
                if (DataMappingType != null)
                {
                    var dataMap = Activator.CreateInstance(DataMappingType) as IDataMapping;
                    if (!dataMap.IsEmpty)
                    {
                        var item = dataMap.Get(table.ConnName, (uint)table.ConnIndex);
                        if (item != null)
                        {
                            this.DataBaseName = item.ToName;
                            this.DataBaseNum = item.ToIndex;
                        }
                    }
                }

                this.TableName = table.Name.IfEmpty(this.ModelType.Name);
                ConnectionConfig config;
                var db = DataBase.Current;
                ConnectionConfig[] configs = db.Get(this.DataBaseName);
                if (configs == null) return;
                uint index = this.DataBaseNum;
                if (index >= configs.Length) index = 0;
                config = configs[index];
                this.Config = config;
                //改为用的时候再实例化 不同则不实例化 减少开销
                //this.Data = new DataHelperX<T>(config);
                //this.DataQ = new DataHelperQ(config);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 表类型
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public ModelType TableType { get; set; } = XiaoFeng.ModelType.Table;
        /// <summary>
        /// 配置文件
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        private ConnectionConfig Config { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        private List<ColumnAttribute> _Fields = null;
        /// <summary>
        /// 列
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public List<ColumnAttribute> Fields
        {
            get
            {
                if (this._Fields == null)
                {
                    this._Fields = new List<ColumnAttribute>();
                    this.ModelType.GetMembers().Each(m =>
                    {
                        if (m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)
                        {
                            var columnAttr = m.GetColumnAttribute();
                            if (columnAttr != null && !this._Fields.Contains(columnAttr))
                                this._Fields.Add(columnAttr);
                        }
                    });
                }
                return _Fields;
            }
            set { this._Fields = value; }
        }
        /// <summary>
        /// 基础类型
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        private Type ModelType { get; set; }
        /// <summary>
        /// 基础类型
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public Type EntityType
        {
            get { return this.ModelType; }
            set
            {
                this.ModelType = value;
                /*设置表名*/
                this._Data = new DataHelperX<T>(this.Config, this.RunSqlCallBack);
                this._Data.DataSQL.TableName = value.GetTableAttribute().Name;
            }
        }
        /// <summary>
        /// 当前类型对象
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public static T Create { get { return Activator.CreateInstance<T>(); } }
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="func">Lambda表达式</param>
        /// <returns></returns>
        [FieldIgnore]
        public static IQueryableX<T> Queryable(Expression<Func<T, bool>> func) => Create.Where(func);
        /// <summary>
        /// 数据库操作
        /// </summary>
        private DataHelperX<T> _Data = null;
        /// <summary>
        /// 数据库操作
        /// </summary>
        private DataHelperX<T> Data
        {
            get
            {
                if (this._Data == null)
                {
                    if (this.Config == null) return null;
                    this._Data = new DataHelperX<T>(this.Config, this.RunSqlCallBack);
                    if (this.TableName.IsNotNullOrEmpty())
                        this._Data.DataSQL.TableName = this.TableName;
                }
                return this._Data;
            }
            set { this._Data = value; }
        }
        /// <summary>
        /// 数据库操作
        /// </summary>
        private DataHelperQ _DataQ = null;
        /// <summary>
        /// 运行SQL事件
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public RunSQLEventHandler RunSqlCallBack;
        /// <summary>
        /// 数据库操作
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public DataHelperQ DataQ
        {
            get
            {
                if (this._DataQ == null)
                {
                    if (this.Config == null) return null;
                    this._DataQ = new DataHelperQ(this.Config, RunSqlCallBack);
                }
                return this._DataQ;
            }
            set { this._DataQ = value; }
        }
        /// <summary>
        /// 转换为QueryableX 注:用其它Json或Xml组件序列化此实例时,要重写此属性,让其忽略当前属性,如果不忽略则会出现列循环导致内存溢出
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public virtual IQueryableX<T> QueryableX { get { return this.Data?.AS(); } }
        /// <summary>
        /// 数据库名
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public string DataBaseName { get; set; }
        /// <summary>
        /// 分库索引 
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public uint DataBaseNum { get; set; } = 0;
        /// <summary>
        /// 分表名称
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [FieldIgnore]
        public string TableName { get; set; }
        /// <summary>
        /// 表属性
        /// </summary>
        private TableAttribute TableAttr { get; set; }
        #endregion

        #region 增删改查
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="func">Lambda表达式</param>
        /// <returns></returns>
        public virtual IQueryableX<T> Where(Expression<Func<T, bool>> func) => this.QueryableX.Where(func);
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        public virtual IQueryableX<T> Where(string whereString) => this.QueryableX.Where(whereString);
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="isBackAutoID">是否返回自增长ID</param>
        /// <returns></returns>
        public virtual Boolean Insert(Boolean isBackAutoID = false)
        {
            if (isBackAutoID)
            {
                return this.Insert(out _);
            }
            else
            {
                var b = this.QueryableX.Insert(this);
                this.Dirtys.Clear();
                return b;
            }
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="ID">自增长ID</param>
        /// <returns></returns>
        public virtual Boolean Insert(out long ID)
        {
            var b = this.Data.Insert(this, out ID);
            this.Dirtys.Clear();
            var PrimaryKey = this.GetPrimaryKey();
            if (PrimaryKey.IsNotNullOrEmpty())
            {
                var p = this.ModelType.GetProperty(PrimaryKey, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (p != null)
                    p.SetValue(this, ID.GetValue(p.PropertyType));
            }
            return b;
        }
        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="models">model集合</param>
        /// <returns></returns>
        public virtual Boolean Inserts(IEnumerable<T> models) => this.QueryableX.Inserts(models);
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public virtual Boolean Update()
        {
            if (this.EntityType != this.Data.DataSQL.ModelType)
                this.Data.DataSQL.ModelType = this.EntityType;
            var b = this.Data.Update(this);
            this.Dirtys.Clear();
            return b;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public virtual Boolean Delete()
        {
            string WhereString = "";
            this.ModelType.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance).Each(p =>
            {
                var cAttr = p.GetColumnAttribute();
                if (cAttr != null && cAttr.PrimaryKey)
                    WhereString += $"and {this.FieldFormat(p.Name)} = '{p.GetValue(this).GetValue()}'";
            });
            WhereString = WhereString.RemovePattern(@"^and\s+");
            return Data.Delete(WhereString);
        }

        #region 数据库格式
        /// <summary>
        /// 数据库格式
        /// </summary>
        /// <param name="_">字符串</param>
        /// <returns></returns>
        internal string FieldFormat(string _)
        {
            if (this.Data == null) return _;
            return DataSQLFun.FieldFormat(this.Data.Config, _);
            /*
            switch (this.Data.Config.ProviderType)
            {
                case DbProviderType.SqlServer:
                case DbProviderType.OleDb:
                case DbProviderType.SQLite:
                    _ = "[" + _ + "]"; break;
                case DbProviderType.Oracle:
                case DbProviderType.MySql:
                    _ = "`" + _ + "`"; break;
                default:
                    _ = "[" + _ + "]"; break;
            }
            return _;*/
        }
        #endregion

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public virtual Boolean Delete(Expression<Func<T, bool>> func) => this.QueryableX.Delete(func);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        public virtual Boolean Delete(string whereString) => this.QueryableX.Delete(whereString);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        public virtual Boolean Delete(Expression<Func<T, bool>> func, string whereString) => this.QueryableX.Where(func).Where(whereString).Delete();
        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public virtual T Find(Expression<Func<T, bool>> func)
        {
            var data = this.Where(func).First();
            if (data != null)
            {
                data.ClearDirty();
                data.SubDataBase(this.DataBaseName, this.DataBaseNum);
                if (data.TableName.StartsWith(this.TableName)) this.TableName = data.TableName;
                if (this.TableName != data.TableName) data.TableName = this.TableName;

            }
            return data;
        }
        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public virtual object Find(Expression<Func<T, bool>> func, Type type)
        {
            var data = this.Where(func).ToEntity(type);
            if (data != null)
            {
                var _data = data as IEntity;
                _data.ClearDirty();
            }
            return data;
        }
        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        public virtual T Find(string whereString)
        {
            var data = this.Where(whereString).First();
            if (data == null) return null;
            data.ClearDirty();
            data.SubDataBase(this.DataBaseName, this.DataBaseNum);
            if (this.TableName != data.TableName) data.TableName = this.TableName;
            return data;
        }/// <summary>
         /// 查找
         /// </summary>
         /// <param name="whereString">条件</param>
         /// <param name="type">类型</param>
         /// <returns></returns>
        public virtual object Find(string whereString, Type type)
        {
            var data = this.Where(whereString).ToEntity(type);
            if (data == null) return null;
            return data;
        }
        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        public virtual T Find(Expression<Func<T, bool>> func, string whereString)
        {
            var data = this.Where(func).Where(whereString).First();
            if (data != null)
            {
                data.ClearDirty();
                data.SubDataBase(this.DataBaseName, this.DataBaseNum);
                if (this.TableName != data.TableName) data.TableName = this.TableName;
            }
            return data;
        }
        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public virtual object Find(Expression<Func<T, bool>> func, string whereString, Type type)
        {
            var data = this.Where(func).Where(whereString).ToEntity(type);

            return data;
        }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public virtual List<T> ToList(Expression<Func<T, bool>> func = null)
        {
            return this.QueryableX.Where(func).ToList();
        }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public virtual List<object> ToList(Type type, Expression<Func<T, bool>> func = null)
        {
            return this.QueryableX.Where(func).ToList(type);
        }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        public virtual List<T> ToList(string whereString)
        {
            return this.QueryableX.Where(whereString).ToList();
        }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="whereString">条件</param>
        /// <param name="type">type</param>
        /// <returns></returns>
        public virtual List<object> ToList(string whereString, Type type)
        {
            return this.QueryableX.Where(whereString).ToList(type);
        }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        public virtual List<T> ToList(Expression<Func<T, bool>> func, string whereString)
        {
            return this.QueryableX.Where(func).Where(whereString).ToList();
        }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public virtual List<object> ToList(Expression<Func<T, bool>> func, string whereString, Type type)
        {
            return this.QueryableX.Where(func).Where(whereString).ToList(type);
        }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <returns></returns>
        public virtual List<object> ToObjectList(Expression<Func<T, bool>> func = null)
        {
            return this.QueryableX.Where(func).ToList().ToList<object>();
        }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        public virtual List<object> ToObjectList(string whereString)
        {
            var QueryX = this.QueryableX;
            return QueryX.Where(whereString).ToList().ToList<object>();
        }
        /// <summary>
        /// 返回列表
        /// </summary>
        /// <param name="func">条件</param>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        public virtual List<object> ToObjectList(Expression<Func<T, bool>> func, string whereString)
        {
            return this.QueryableX.Where(func).Where(whereString).ToList().ToList<object>();
        }
        #endregion

        #region 设置数据
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="func">表达式树</param>
        public virtual void Set(Expression<Func<T, bool>> func) { }
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="func">表达式树</param>
        /// <param name="obj">对象</param>
        public virtual void Set(Expression<Func<T, bool>> func, T obj)
        {
            string val = new QueryableHelper().ExpressionRouter(func.Body).ReplacePattern(@"\s*is\s*null", " = null");

            var ms = val.GetMatches(@"(?<FieldName>\w+)\s*=\s*(?<FieldValue>[^)]*)");
            var t = obj.GetType();
            ms.Each(a =>
            {
                var FieldName = a["FieldName"];
                var FieldValue = a["FieldValue"].Trim('\'').Trim();
                var p = t.GetProperty(FieldName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (p != null || p.CanWrite)
                {
                    (obj as Entity<T>).AddDirty(p.Name);
                    p.SetValue(obj, FieldValue == "null" ? null : FieldValue.GetValue(p.PropertyType));
                }
            });
        }
        #endregion

        #region 查找主键
        /// <summary>
        /// 查找主键
        /// </summary>
        /// <returns></returns>
        public virtual string GetPrimaryKey()
        {
            var table = (this.ModelType ?? typeof(T)).GetTableAttribute();
            if (table != null && table.PrimaryKey.IsNotNullOrEmpty()) return table.PrimaryKey;
            return this.Fields.Where(a => a.PrimaryKey).FirstOrDefault()?.Name;
        }
        #endregion

        #region 获取唯一键字段
        /// <summary>
        /// 获取唯一键字段
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetUniqueKey()
        {
            return this.Fields.Where(a => a.IsUnique && !a.AutoIncrement).Select(a => a.Name).ToList();
        }
        #endregion

        #region 是否存在
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public Boolean Exists(Expression<Func<T, bool>> where) => this.Count(where) > 0;
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        public Boolean Exists(string whereString) => this.Count(whereString) > 0;
        #endregion

        #region 条数
        /// <summary>
        /// 条数
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> where = null) => this.QueryableX.Where(where).Count();
        /// <summary>
        /// 条数
        /// </summary>
        /// <param name="whereString">条件</param>
        /// <returns></returns>
        public int Count(string whereString) => this.QueryableX.Where(whereString).Count();
        #endregion

        #region 设置分库
        /// <summary>
        /// 设置分库
        /// </summary>
        /// <param name="key">分库数据库连接串</param>
        /// <param name="num">库索引</param>
        /// <param name="suffix">分表后缀</param>
        /// <returns>对象</returns>
        [Obsolete("当前方法已过时,请使用SubDataBase", false)]
        public T SetSubDataBase(string key, uint num, string suffix = "")
        {
            var db = DataBase.Current;
            ConnectionConfig[] configs;
            if (key.IsNotNullOrEmpty() && key.EqualsIgnoreCase(this.DataBaseName))
            {
                this.DataBaseName = key;
            }
            if (this.DataBaseName.IsNullOrEmpty()) this.DataBaseName = this.TableAttr.ConnName;

            configs = db.Get(this.DataBaseName);
            if (configs == null) return null;
            this.Config = configs[num];

            this.DataBaseNum = num;
            if (suffix.IsNotNullOrEmpty())
                return this.SetSubTable(suffix);
            else
            {
                this.SetDataX();
                return (T)this;
            }
        }
        /// <summary>
        /// 设置分库
        /// </summary>
        /// <param name="num">库索引</param>
        /// <returns></returns>
        [Obsolete("当前方法已过时,请使用SubDataBase", false)]
        public T SetSubDataBase(uint num) => this.SetSubDataBase("", num);
        #endregion

        #region 设置分表名称
        /// <summary>
        /// 设置分表名称
        /// </summary>
        /// <param name="suffix">后缀</param>
        /// <returns>对象</returns>
        [Obsolete("当前方法已过时,请使用SubTable", false)]
        public T SetSubTable(string suffix)
        {
            this.TableName.IfEmptyValue(() => this.TableAttr?.Name.IfEmpty(this.ModelType?.Name));
            this.TableName = this.TableName.RemovePattern(@"_FB_[\s\S]*$");
            if (suffix.IsNotNullOrEmpty())
                this.TableName += "_FB_" + suffix;
            this.SetDataX();
            return this as T;
        }
        #endregion

        #region 创建分表
        /// <summary>
        /// 创建分表
        /// </summary>
        /// <param name="suffix">后缀</param>
        /// <returns>返回是否创建成功</returns>
        public async Task<Boolean> CreateSubTable(string suffix)
        {
            if (suffix.IsNullOrEmpty() ||
                this.TableAttr == null ||
                this.TableAttr.ModelType != XiaoFeng.ModelType.Table ||
                this.TableAttr.Name.IsNullOrEmpty()) return false;
            return await this.Data.DataHelper.ExecuteNonQueryAsync($"select * into {this.TableAttr.Name}_FB_{suffix} from {this.TableAttr.Name} where 1=0;").ConfigureAwait(false) != -1;
        }
        #endregion

        #region 使用分库
        /// <summary>
        /// 使用分库分表
        /// </summary>
        /// <returns></returns>
        public T Sub()
        {
            /*var SessionID = UserSession.Id;
            if (SessionID.IsNotNullOrEmpty())
            {
                var value = Cache.CacheHelper.Get(SessionID);
                if (value != null)
                {
                    var val = value as SubDataBaseTable;
                    return this.SubDataBase(val.Key, val.Num.Value, val.Suffix);
                }
            }*/
            return (T)this;
        }
        /// <summary>
        /// 使用分库
        /// </summary>
        /// <returns></returns>
        public T SubDataBase()
        {
            /*
            var SessionID = UserSession.Id;
            if (SessionID.IsNotNullOrEmpty())
            {
                var value = Cache.CacheHelper.Get(SessionID);
                if (value != null)
                {
                    var val = value as SubDataBaseTable;
                    return this.SubDataBase(val.Key, val.Num.Value);
                }
            }*/
            return (T)this;
        }
        /// <summary>
        /// 使用分库
        /// </summary>
        /// <param name="key">数据库连接串</param>
        /// <param name="isGlobal">是否全局使用</param>
        /// <returns></returns>
        public T SubDataBase(string key, bool isGlobal = false) => this.SubDataBase(key, 0, "", isGlobal);
        /// <summary>
        /// 使用分库
        /// </summary>
        /// <param name="num">库索引</param>
        /// <param name="isGlobal">是否全局使用</param>
        /// <returns></returns>
        public T SubDataBase(uint num, bool isGlobal = false) => this.SubDataBase("", num, "", isGlobal);
        /// <summary>
        /// 使用分库
        /// </summary>
        /// <param name="key">分库数据库连接串</param>
        /// <param name="num">库索引</param>
        /// <param name="suffix">分表后缀</param>
        /// <param name="isGlobal">是否全局使用</param>
        /// <returns></returns>
        public T SubDataBase(string key, uint num, string suffix = "", bool isGlobal = false)
        {
            var db = DataBase.Current;
            ConnectionConfig[] configs;
            if (key.IsNotNullOrEmpty() && key.EqualsIgnoreCase(this.DataBaseName))
            {
                this.DataBaseName = key;
                if (isGlobal)
                {
                    /* var SessionID = UserSession.Id;
                     if (SessionID.IsNotNullOrEmpty())
                     {
                         var value = Cache.CacheHelper.Get(SessionID);
                         if (value != null)
                         {
                             var val = value as SubDataBaseTable;
                             val.Key = key;
                             val.Num = num;
                             Cache.CacheHelper.Set(SessionID, val);
                         }
                     }*/
                }
            }
            if (this.DataBaseName.IsNullOrEmpty()) this.DataBaseName = this.TableAttr.ConnName;

            configs = db.Get(this.DataBaseName);
            if (configs == null) return null;
            this.Config = configs[num];

            this.DataBaseNum = num;

            if (suffix.IsNotNullOrEmpty())
                return this.SubTable(suffix, isGlobal);
            else
            {
                this.SetDataX();
                return (T)this;
            }
        }
        /// <summary>
        /// 使用分库
        /// </summary>
        /// <param name="config">数据库配置</param>
        /// <param name="suffix">分表后缀</param>
        /// <param name="isGlobal">是否全局使用</param>
        /// <returns></returns>
        public T SubDataBase(ConnectionConfig config, string suffix = "", bool isGlobal = false)
        {
            this.Config = config;

            this.DataBaseNum = 0;

            if (suffix.IsNotNullOrEmpty())
                return this.SubTable(suffix, isGlobal);
            else
            {
                this.SetDataX();
                return (T)this;
            }
        }
        #endregion

        #region 使用分表
        /// <summary>
        /// 使用分表
        /// </summary>
        /// <returns></returns>
        public T SubTable()
        {
            /*var SessionID = UserSession.Id;
            if (SessionID.IsNotNullOrEmpty())
            {
                var value = Cache.CacheHelper.Get(SessionID);
                if (value != null)
                {
                    var val = value as SubDataBaseTable;
                    return this.SubTable(val.Suffix);
                }
            }*/
            return this as T;
        }
        /// <summary>
        /// 使用分表
        /// </summary>
        /// <param name="suffix">分表后缀</param>
        /// <param name="isGlobal">是否全局使用</param>
        /// <returns></returns>
        public T SubTable(string suffix, bool isGlobal = false)
        {
            this.TableName.IfEmptyValue(() => this.TableAttr?.Name.IfEmpty(this.ModelType?.Name));
            this.TableName = this.TableName.RemovePattern(@"_FB_[\s\S]*$");
            if (suffix.IsNotNullOrEmpty())
            {
                this.TableName += "_FB_" + suffix;
                if (isGlobal)
                {
                    /*
                    var SessionID = UserSession.Id;
                    if (SessionID.IsNotNullOrEmpty())
                    {
                        var value = Cache.CacheHelper.Get(SessionID);
                        if (value != null)
                        {
                            var val = value as SubDataBaseTable;
                            val.Suffix = suffix;
                            Cache.CacheHelper.Set(SessionID, val);
                        }
                    }*/
                }
            }
            this.SetDataX();
            return this as T;
        }
        #endregion

        #region 设置DataHelperX
        /// <summary>
        /// 设置DataHelperX
        /// </summary>
        private void SetDataX()
        {
            this._Data = new DataHelperX<T>(this.Config, this.RunSqlCallBack);
            this._Data.DataSQL.TableName = this.TableName;
            this._DataQ = new DataHelperQ(this.Config, this.RunSqlCallBack);
        }
        #endregion
    }
    /// <summary>
    /// 视图基础类
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public abstract class EntityView<T> : Entity<T> where T : Entity<T>, new()
    {
        /// <summary>
        /// 无参构造器
        /// </summary>
        public EntityView() : base() { }
        /// <summary>
        /// 视图内容
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public string ViewContent { get; set; }
    }
    /// <summary>
    /// 模型
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public abstract class Entity<T, TKey> : Entity<T> where T : Entity<T>, new()
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public virtual TKey ID { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        public Entity()
        {

        }
    }
    /// <summary>
    /// 实体基础类
    /// </summary>
    public abstract class Entity : EntityBase { }
}