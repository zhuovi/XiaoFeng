using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Model;
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
    /// 最基础基类接口
    /// </summary>
    public interface IEntityBase
    {
        #region 定义属性值改变委托
        /// <summary>
        /// 委托事件
        /// </summary>
        event ValueChange OnValueChange;
        #endregion

        /// <summary>
        /// 移除值
        /// </summary>
        /// <param name="key">key值</param>
        void RemoveAllValues(string key);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        void SetAllValues(string key, object value);
        /// <summary>
        /// 通过字段名获取相应的值
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        object this[string name] { get; set; }
        /// <summary>
        /// 对象所有属性名集合
        /// </summary>
        List<string> AllKeys { get; }

        #region 获取脏数据
        /// <summary>
        /// 获取脏数据
        /// </summary>
        /// <returns></returns>
        DirtyCollection GetDirty();
        #endregion

        #region 添加脏数据
        /// <summary>
        /// 添加脏数据
        /// </summary>
        /// <param name="fieldName">字段名</param>
        void AddDirty(string fieldName);
        /// <summary>
        /// 添加脏数据
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="oldValue">老值</param>
        /// <param name="newValue">新值</param>
        void AddDirty(string fieldName, object oldValue, object newValue);
        /// <summary>
        /// 设置脏数据
        /// </summary>
        /// <param name="dirty">脏数据</param>
        void SetDirty(DirtyCollection dirty);
        #endregion

        #region 清理脏数据
        /// <summary>
        /// 清理脏数据 字段名为空则清空所有脏数据
        /// </summary>
        void ClearDirty(string fieldName = "");
        #endregion

        #region 是否存在于脏数据中
        /// <summary>
        /// 是否存在于脏数据中
        /// </summary>
        /// <param name="name">键名</param>
        /// <returns></returns>
        Boolean ContainsDirty(string name);
        #endregion
    }
}
