using System.IO;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-14 13:33:54                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Binary
{
    /*
     * 响应包格式
     Byte/     0       |       1       |       2       |       3       |
        /              |               |               |               |
       |0 1 2 3 4 5 6 7|0 1 2 3 4 5 6 7|0 1 2 3 4 5 6 7|0 1 2 3 4 5 6 7|
       +---------------+---------------+---------------+---------------+
      0| Magic         | Opcode        | Key Length                    |
       +---------------+---------------+---------------+---------------+
      4| Extras length | Data type     | Status                        |
       +---------------+---------------+---------------+---------------+
      8| Total body length                                             |
       +---------------+---------------+---------------+---------------+
     12| Opaque                                                        |
       +---------------+---------------+---------------+---------------+
     16| CAS                                                           |
       |                                                               |
       +---------------+---------------+---------------+---------------+
       Total 24 bytes
    ---------------------------------------------------------------------
    Magic               魔法数字，用来区分包头是请求包头还是响应包头
    Opcode              操作码命令码，也就是对应的命令
    Key Length          键长度，附加命令后面的文本键的长度（以字节为单位）
    Extras length       附加命令的长度（以字节为单位）
    Data type           保留以备将来使用（很快会使用）
    Status              状态响应的状态（错误时为非零）
    Total body length   正文总长度,额外+键+值的长度（以字节为单位）。
    Opaque              将在回复中复制回您。
    CAS                 数据版本检查。
     */
    /// <summary>
    /// 响应包
    /// </summary>
    public class ResponsePacket : BasePacket
    {
        #region 构造器
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="buffers">数据</param>
        public ResponsePacket(byte[] buffers)
        {
            if (buffers.Length < 24) return;
            var reader = new MemoryStream(buffers);
            this.Magic = reader.ReadByte().ToEnum<MagicType>();

            this.Opcode = reader.ReadByte().ToEnum<CommandType>();

            var Keys = new byte[2];
            reader.Read(Keys, 0, 2);

            var KeyLength = ToValue(Keys);
            var ExtrasLength = reader.ReadByte();

            this.DataType = reader.ReadByte();

            var Status = new byte[2];
            reader.Read(Status, 0, 2);

            this.Status = ToValue(Status).ToEnum<ResponseStatus>();
            var Bodys = new byte[4];
            reader.Read(Bodys, 0, 4);

            var TotalBodyLength = ToValue(Bodys);
            this.Opaque = new byte[4];
            reader.Read(this.Opaque, 0, 4);

            this.CAS = new byte[8];
            reader.Read(this.CAS, 0, 8);
            if (ExtrasLength > 0)
            {
                this.Extras = new byte[ExtrasLength];
                reader.Read(this.Extras, 0, this.Extras.Length);
            }
            if (KeyLength > 0)
            {
                this.Key = new byte[KeyLength];
                reader.Read(this.Key, 0, this.Key.Length);
            }
            if (TotalBodyLength - ExtrasLength - KeyLength > 0)
            {
                this.Value = new byte[TotalBodyLength - ExtrasLength - KeyLength];
                reader.Read(this.Value, 0, this.Value.Length);
            }
            this.PayLoad = new byte[reader.Length - reader.Position];
            reader.Read(this.PayLoad, 0, this.PayLoad.Length);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 请求响应response的状态
        /// </summary>
        public ResponseStatus Status { get; set; }
        /// <summary>
        /// 响应数据
        /// </summary>
        public byte[] PayLoad { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}