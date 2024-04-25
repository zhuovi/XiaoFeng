using System.ComponentModel;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-12-14 09:32:07                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Data.SQL
{
    #region 表类型
    /// <summary>
    /// 表类型
    /// </summary>
    public enum TableType
    {
        /// <summary>
        /// 总表
        /// </summary>
        [Description("总表")]
        [Prefix("TT")]
        TResult = 0,
        /// <summary>
        /// 第1张表
        /// </summary>
        [Description("第1张表")]
        [Prefix("A")] 
        T1 = 65,
        /// <summary>
        /// 第2张表
        /// </summary>
        [Description("第2张表")]
        [Prefix("B")] 
        T2 = 66,
        /// <summary>
        /// 第3张表
        /// </summary>
        [Description("第3张表")]
        [Prefix("C")] 
        T3 = 67,
        /// <summary>
        /// 第4张表
        /// </summary>
        [Description("第4张表")]
        [Prefix("D")] 
        T4 = 68,
        /// <summary>
        /// 第5张表
        /// </summary>
        [Description("第5张表")]
        [Prefix("E")] 
        T5 = 69,
        /// <summary>
        /// 第6张表
        /// </summary>
        [Description("第6张表")]
        [Prefix("F")] 
        T6 = 70,
        /// <summary>
        /// 第7张表
        /// </summary>
        [Description("第7张表")]
        [Prefix("G")] 
        T7 = 71,
        /// <summary>
        /// 第8张表
        /// </summary>
        [Description("第8张表")]
        [Prefix("H")] 
        T8 = 72,
        /// <summary>
        /// 第9张表
        /// </summary>
        [Description("第9张表")]
        [Prefix("I")] 
        T9 = 73,
        /// <summary>
        /// 第10张表
        /// </summary>
        [Description("第10张表")]
        [Prefix("J")] 
        T10 = 74,
        /// <summary>
        /// 第11张表
        /// </summary>
        [Description("第11张表")]
        [Prefix("K")] 
        T11 = 75,
        /// <summary>
        /// 第12张表
        /// </summary>
        [Description("第12张表")]
        [Prefix("L")] 
        T12 = 76,
        /// <summary>
        /// 第13张表
        /// </summary>
        [Description("第13张表")]
        [Prefix("M")] 
        T13 = 77,
        /// <summary>
        /// 第14张表
        /// </summary>
        [Description("第14张表")]
        [Prefix("N")] 
        T14 = 78,
        /// <summary>
        /// 第15张表
        /// </summary>
        [Description("第15张表")]
        [Prefix("O")] 
        T15 = 79,
        /// <summary>
        /// 第16张表
        /// </summary>
        [Description("第16张表")]
        [Prefix("P")] 
        T16 = 80,
        /// <summary>
        /// 第17张表
        /// </summary>
        [Description("第17张表")]
        [Prefix("Q")] 
        T17 = 81,
        /// <summary>
        /// 第18张表
        /// </summary>
        [Description("第18张表")]
        [Prefix("R")] 
        T18 = 82,
        /// <summary>
        /// 第19张表
        /// </summary>
        [Description("第19张表")]
        [Prefix("S")] 
        T19 = 83,
        /// <summary>
        /// 第20张表
        /// </summary>
        [Description("第20张表")]
        [Prefix("T")] 
        T20 = 84,
        /// <summary>
        /// 第21张表
        /// </summary>
        [Description("第21张表")]
        [Prefix("U")] 
        T21 = 85,
        /// <summary>
        /// 第22张表
        /// </summary>
        [Description("第22张表")]
        [Prefix("V")] 
        T22 = 86,
        /// <summary>
        /// 第23张表
        /// </summary>
        [Description("第23张表")]
        [Prefix("W")] 
        T23 = 87,
        /// <summary>
        /// 第24张表
        /// </summary>
        [Description("第24张表")]
        [Prefix("X")] 
        T24 = 88,
        /// <summary>
        /// 第25张表
        /// </summary>
        [Description("第25张表")]
        [Prefix("Y")] 
        T25 = 89,
        /// <summary>
        /// 第26张表
        /// </summary>
        [Description("第26张表")]
        [Prefix("Z")] 
        T26 = 90
    }
    #endregion
}