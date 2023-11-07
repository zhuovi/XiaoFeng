using System;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-05-12 11:46:21                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Model
{
    /// <summary>
    /// 数据库连接映射
    /// </summary>
    public interface IDataMapping
    {
        /// <summary>
        /// 是否为空
        /// </summary>
        Boolean IsEmpty { get; }
        /// <summary>
        /// Token Key
        /// </summary>
        string TokenKey { get; set; }
        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="item">项</param>
        void Add(IDataMappingItem item);
        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="fromName">原节点名</param>
        /// <param name="fromIndex">原索引</param>
        /// <param name="toName">目的节点名</param>
        /// <param name="toIndex">目的索引</param>
        void Add(string fromName, uint fromIndex, string toName, uint toIndex);
        /// <summary>
        /// 移除项
        /// </summary>
        /// <param name="index">索引</param>
        void Remove(int index);
        /// <summary>
        /// 移除项
        /// </summary>
        /// <param name="fromName">原节点名</param>
        /// <param name="fromIndex">原索引</param>
        void Remove(string fromName, uint fromIndex);
        /// <summary>
        /// 移除项
        /// </summary>
        /// <param name="item">节点</param>
        void Remove(IDataMappingItem item);
        /// <summary>
        /// 获取项
        /// </summary>
        /// <param name="fromName">原节点名</param>
        /// <param name="fromIndex">原索引</param>
        /// <returns></returns>
        IDataMappingItem Get(string fromName, uint fromIndex);
        /// <summary>
        /// 获取项
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        IDataMappingItem Get(int index);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="item">项</param>
        /// <returns></returns>
        bool Contains(IDataMappingItem item);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="fromName">原节点名</param>
        /// <param name="fromIndex">原索引</param>
        /// <returns></returns>
        bool Contains(string fromName, uint fromIndex);
        /// <summary>
        /// 清空
        /// </summary>
        void Clear();
    }
}