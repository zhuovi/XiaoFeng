using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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