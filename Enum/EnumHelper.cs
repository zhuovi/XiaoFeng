using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XiaoFeng
{
    /// <summary>
    /// Enum 帮助类
    /// Version  : 1.0.1
    /// Author : jacky
    /// Create Date : 2016-06-05
    /// Update Date : 2016-07-04
    /// </summary>
    public class EnumHelper { }
    
    #region 输出类型
    /// <summary>
    /// 输出类型
    /// </summary>
    public enum WriteType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        [Description("字符串")]
        String = 0,
        /// <summary>
        /// JSON
        /// </summary>
        [Description("JSON")] 
        JSON = 1,
        /// <summary>
        /// XML
        /// </summary>
        [Description("XML")] 
        XML = 2,
        /// <summary>
        /// 空
        /// </summary>
        [Description("空")] 
        Null = 3
    }
    #endregion

    #region 值类型枚举
    /// <summary>
    /// 值类型枚举
    /// </summary>
    public enum ValueTypes
    {
        /// <summary>
        /// 空
        /// </summary>
        [Description("空")] 
        Null = 0,
        /// <summary>
        /// 值
        /// </summary>
        [Description("值")] 
        Value = 1,
        /// <summary>
        /// 类
        /// </summary>
        [Description("类")] 
        Class = 2,
        /// <summary>
        /// 结构体
        /// </summary>
        [Description("结构体")] 
        Struct = 3,
        /// <summary>
        /// 枚举
        /// </summary>
        [Description("枚举")] 
        Enum = 4,
        /// <summary>
        /// 字符串
        /// </summary>
        [Description("字符串")] 
        String = 5,
        /// <summary>
        /// 数组
        /// </summary>
        [Description("数组")] 
        Array = 6,
        /// <summary>
        /// List
        /// </summary>
        [Description("List")] 
        List = 7,
        /// <summary>
        /// 字典
        /// </summary>
        [Description("字典")] 
        Dictionary = 8,
        /// <summary>
        /// ArrayList
        /// </summary>
        [Description("ArrayList")] 
        ArrayList = 9,
        /// <summary>
        /// 是否是集合类型
        /// </summary>
        [Description("是否是集合类型")] 
        IEnumerable = 10,
        /// <summary>
        /// 字典类型
        /// </summary>
        [Description("字典类型")] 
        IDictionary = 11,
        /// <summary>
        /// 匿名类型
        /// </summary>
        [Description("匿名类型")] 
        Anonymous = 12,
        /// <summary>
        /// DataTable
        /// </summary>
        [Description("DataTable")] 
        DataTable = 13,
        /// <summary>
        /// 其它
        /// </summary>
        [Description("其它")] 
        Other = 20
    }
    #endregion

    #region 消息状态
    /// <summary>
    /// 消息状态
    /// </summary>
    public enum ResponseState
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        none = 0,
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        success = 200,
        /// <summary>
        /// 出错
        /// </summary>
        [Description("出错")]
        error = 500,
        /// <summary>
        /// 警告
        /// </summary>
        [Description("警告")]
        warning = 100
    }
    #endregion

    #region DateDiff类型
    /// <summary>
    /// DateDiff类型
    /// </summary>
    public enum DateDiffType
    {
        /// <summary>
        /// 毫秒
        /// </summary>
        [Description("毫秒")]
        Milliseconds = 0,
        /// <summary>
        /// 秒
        /// </summary>
        [Description("秒")]
        Seconds = 1,
        /// <summary>
        /// 分
        /// </summary>
        [Description("分")]
        Minutes = 2,
        /// <summary>
        /// 时
        /// </summary>
        [Description("时")]
        Hours = 3,
        /// <summary>
        /// 天
        /// </summary>
        [Description("天")]
        Days = 4,
        /// <summary>
        /// 周
        /// </summary>
        [Description("周")]
        Weeks = 5,
        /// <summary>
        /// 月
        /// </summary>
        [Description("月")]
        Months = 6,
        /// <summary>
        /// 年
        /// </summary>
        [Description("年")]
        Years = 7
    }
    #endregion

    #region 配置格式
    /// <summary>
    /// 配置格式
    /// </summary>
    public enum ConfigFormat
    {
        /// <summary>
        /// Json
        /// </summary>
        [Description("Json")]
        Json = 0,
        /// <summary>
        /// Xml
        /// </summary>
        [Description("Xml")]
        Xml = 1,
        /// <summary>
        /// Ini
        /// </summary>
        [Description("Ini")]
        Ini = 2
    }
    #endregion

    #region Model类型
    /// <summary>
    /// Model类型
    /// </summary>
    public enum ModelType
    {
        /// <summary>
        /// Model
        /// </summary>
        [Description("实体模型")]
        Model = 0,
        /// <summary>
        /// 表
        /// </summary>
        [Description("表")]
        Table = 1,
        /// <summary>
        /// 视图
        /// </summary>
        [Description("视图")]
        View = 2,
        /// <summary>
        /// 存储过程
        /// </summary>
        [Description("存储过程")]
        Procedure = 3,
        /// <summary>
        /// 函数
        /// </summary>
        [Description("函数")]
        Function = 4
    }
    #endregion

    #region 系统类型
    /// <summary>
    /// 系统类型
    /// </summary>
    public enum PlatformOS{
        /// <summary>
        /// linux os
        /// </summary>
        [Description("Linux")]
        Linux = 0,
        /// <summary>
        /// mac os
        /// </summary>
        [Description("OSX")]
        OSX = 1,
        /// <summary>
        /// windows os
        /// </summary>
        [Description("Windows")]
        Windows = 2
    }
    #endregion

    #region 文件类型
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileAttribute
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        UnKnown = 0,
        /// <summary>
        /// 文件
        /// </summary>
        [Description("文件")]
        File = 1,
        /// <summary>
        /// 目录
        /// </summary>
        [Description("目录")]
        Directory = 2
    }
    #endregion

    #region 缓存类型
    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum CacheType
    {
        /// <summary>
        /// 默认
        /// </summary>
        [Description("默认")]
        Default = -1,
        /// <summary>
        /// 不缓存
        /// </summary>
        [Description("不缓存")]
        No = 0,
        /// <summary>
        /// 内存
        /// </summary>
        [Description("内存")]
        Memory = 1,
        /// <summary>
        /// 磁盘
        /// </summary>
        [Description("磁盘")]
        Disk = 2,
        /// <summary>
        /// Redis
        /// </summary>
        [Description("Redis")]
        Redis = 3,
        /// <summary>
        /// Memcache
        /// </summary>
        [Description("Memcache")]
        Memcache = 4,
        /// <summary>
        /// MongoDB
        /// </summary>
        [Description("MongoDB")]
        MongoDB = 5
    }
    #endregion

    #region 枚举值
    /// <summary>
    /// 枚举值
    /// </summary>
    public enum EnumValueType
    {
        /// <summary>
        /// 值
        /// </summary>
        [Description("值")]
        Value = 0,
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")] 
        Name = 1,
        /// <summary>
        /// 说明
        /// </summary>
        [Description("说明")] 
        Description = 2
    }
    #endregion
}