using System;
namespace XiaoFeng
{
    /// <summary>  
    /// 动态生产有规律的ID  
    /// 雪花算法
    /// Author : Jacky
    /// Email : Jacky@zhuovi.com
    /// Create Date : 2018-05-31
    /// </summary>  
    public class SnowFlake
    {
        #region 属性
        /// <summary>
        /// 机器ID
        /// </summary>
        private static long MachineID;
        /// <summary>
        /// 数据ID
        /// </summary>
        private static long DatacenterID = 0L;
        /// <summary>
        /// 计数从零开始
        /// </summary>
        private static long Sequence = 0L;
        /// <summary>
        /// 唯一时间随机量
        /// </summary>
        private static long Twepoch = 687888001020L;
        /// <summary>
        /// 机器码字节数
        /// </summary>
        private static long MachineIdBits = 5L;
        /// <summary>
        /// 数据字节数
        /// </summary>
        private static long DatacenterIdBits = 5L;
        /// <summary>
        /// 最大机器ID
        /// </summary>
        public static long MaxMachineID = -1L ^ -1L << (int)MachineIdBits;
        /// <summary>
        /// 最大数据ID
        /// </summary>
        private static long MaxDatacenterID = -1L ^ (-1L << (int)DatacenterIdBits);
        /// <summary>
        /// 计数器字节数，12个字节用来保存计数码
        /// </summary>
        private static long SequenceBits = 12L;
        /// <summary>
        /// 机器码数据左移位数，就是后面计数器占用的位数
        /// </summary>
        private static long MachineIdShift = SequenceBits;
        /// <summary>
        /// 
        /// </summary>
        private static long DatacenterIdShift = SequenceBits + MachineIdBits;
        /// <summary>
        /// 时间戳左移动位数就是机器码+计数器总字节数+数据字节数
        /// </summary>
        private static long TimestampLeftShift = SequenceBits + MachineIdBits + DatacenterIdBits;
        /// <summary>
        /// 一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成
        /// </summary>
        public static long SequenceMask = -1L ^ -1L << (int)SequenceBits;
        /// <summary>
        /// 最后时间戳
        /// </summary>
        private static long LastTimestamp = -1L;
        /// <summary>
        /// 加锁对象
        /// </summary>
        private static object SyncRoot = new object();
        /// <summary>
        /// 静态方法
        /// </summary>
        static SnowFlake Snowflake;
        /// <summary>
        /// 实例化一个静态方法
        /// </summary>
        /// <returns></returns>
        public static SnowFlake Instance()
        {
            if (Snowflake == null)
                Snowflake = new SnowFlake();
            return Snowflake;
        }
        #endregion

        #region 构造器
        /// <summary>
        /// 构造器
        /// </summary>
        public SnowFlake()
        {
            SnowFlakes(0L, -1);
        }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="machineId">机器码</param>
        public SnowFlake(long machineId)
        {
            SnowFlakes(machineId, -1);
        }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="machineId">机器码</param>
        /// <param name="datacenterId">数据字节数</param>
        public SnowFlake(long machineId, long datacenterId)
        {
            SnowFlakes(machineId, datacenterId);
        }
        /// <summary>
        /// 生成数据ID
        /// </summary>
        /// <param name="machineId">机器码</param>
        /// <param name="datacenterId">数据字节数</param>
        private void SnowFlakes(long machineId, long datacenterId)
        {
            if (machineId >= 0)
            {
                if (machineId > MaxMachineID)
                {
                    throw new Exception("机器码ID非法");
                }
                SnowFlake.MachineID = machineId;
            }
            if (datacenterId >= 0)
            {
                if (datacenterId > MaxDatacenterID)
                {
                    throw new Exception("数据中心ID非法");
                }
                SnowFlake.DatacenterID = datacenterId;
            }
        }
        #endregion

        #region 方法
        /// <summary>  
        /// 生成当前时间戳  
        /// </summary>  
        /// <returns>毫秒</returns>  
        private static long GetTimestamp()
        {
            return DateTime.Now.ToTimeStamps();
        }
        /// <summary>  
        /// 获取下一微秒时间戳  
        /// </summary>  
        /// <param name="lastTimestamp">最后时间戳</param>  
        /// <returns></returns>  
        private static long GetNextTimestamp(long lastTimestamp)
        {
            long timestamp = GetTimestamp();
            if (timestamp <= lastTimestamp)
            {
                timestamp = GetTimestamp();
            }
            return timestamp;
        }
        /// <summary>  
        /// 获取长整形的ID  
        /// </summary>  
        /// <returns></returns>  
        public long GetID()
        {
            lock (SyncRoot)
            {
                long timestamp = GetTimestamp();
                if (SnowFlake.LastTimestamp == timestamp)
                { //同一微妙中生成ID  
                    Sequence = (Sequence + 1) & SequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限  
                    if (Sequence == 0)
                    {
                        //一微妙内产生的ID计数已达上限，等待下一微妙  
                        timestamp = GetNextTimestamp(SnowFlake.LastTimestamp);
                    }
                }
                else
                {
                    //不同微秒生成ID  
                    Sequence = 0L;
                }
                if (timestamp < LastTimestamp)
                {
                    throw new Exception("时间戳比上一次生成ID时时间戳还小，故异常");
                }
                SnowFlake.LastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳  
                long Id = ((timestamp - Twepoch) << (int)TimestampLeftShift)
                    | (DatacenterID << (int)DatacenterIdShift)
                    | (MachineID << (int)MachineIdShift)
                    | Sequence;
                return Id;
            }
        }
        #endregion
    }
}