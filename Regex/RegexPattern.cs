/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-08-11 17:22:57                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 正则格式
    /// Verstion : 1.0.0
    /// Create Time : 2017/8/11 17:22:57
    /// Update Time : 2017/8/11 17:22:57
    /// </summary>
    public static class RegexPattern
    {
        #region 属性

        #region 格式正则
        /// <summary>
        /// 物理路径
        /// </summary>
        public const string BasePath = @"^[a-z]+:(\\|\/)";
        /// <summary>
        /// 字母格式
        /// </summary>
        public const string Letter = @"^[a-zA-Z]+$";
        /// <summary>
        /// 数字格式
        /// </summary>
        public const string Numberic = @"^(\+|-)?\d+$";
        /// <summary>
        /// 浮点格式
        /// </summary>
        public const string Float = @"^(\+|-)?\d+([.]\d+)?$";
        /// <summary>
        /// Json
        /// </summary>
        public const string Json = @"^[\s\r\n\t]*(({[\s\S]+})|(\[[\s\S]+\]))[\s\r\n\t]*$";
        /// <summary>
        /// Xml
        /// </summary>
        public const string Xml = @"((^<\?xml[^>]+?\?>)|(<([a-z0-9]+?)(\s+[^>]*?)>[\s\S]*?</\3>)|(<[a-z0-9]+?[^>]*?/>))";
        /// <summary>
        /// 16进制
        /// </summary>
        public const string Hex = @"^[0-9a-f]+$";
        #endregion

        #endregion

        #region 方法

        #endregion
    }
}