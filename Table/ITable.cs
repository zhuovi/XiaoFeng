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
*  Create Time : 2017-10-31 14:18:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Table
{
    /// <summary>
    /// 创建表接口
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// 创建表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="tableName">表名</param>
        /// <param name="connName">连接串名</param>
        /// <param name="connIndex">连接串索引</param>
        /// <returns></returns>
        string Create<T>(string tableName = "", string connName = "", int connIndex = -1);
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="modelType">类型</param>
        /// <param name="tableName">表名</param>
        /// <param name="connName">连接串名</param>
        /// <param name="connIndex">连接串索引</param>
        /// <returns></returns>
        string Create(Type modelType, string tableName = "", string connName = "", int connIndex = -1);
    }
}