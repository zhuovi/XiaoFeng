using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
namespace XiaoFeng.Net
{
    /*
   ===================================================================
      Author : jacky
      Email : jacky@zhuovi.com
      QQ : 7092734
      Site : www.zhuovi.com
   ===================================================================
   */
    /// <summary>
    ///  发送邮件操作类
    /// Verstion : 1.0.1
    /// Author : jacky
    /// Email : jacky@zhuovi.com
    /// QQ : 7092734
    /// Site : www.zhuovi.com
    /// Create Time : 2018/04/10 17:25:39
    /// Update Time : 2018/04/10 17:25:39
    /// </summary>
    public class Email
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public Email() { }
        #endregion

        #region 属性
        /// <summary>
        /// 发送到邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.com:service
        /// </summary>
        private string _ToMail = "";
        /// <summary>
        /// 发送到邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.com:service
        /// </summary>
        public string ToMail { get { return _ToMail; } set { _ToMail = value; } }
        /// <summary>
        /// 密送邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.com:service
        /// </summary>
        private string _BCCMail = "";
        /// <summary>
        /// 密送到邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.com:service
        /// </summary>
        public string BCCMail { get { return _BCCMail; } set { _BCCMail = value; } }
        /// <summary>
        /// 抄送邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.com:service
        /// </summary>
        private string _CCMail = "";
        /// <summary>
        /// 抄送到邮箱地址 如果是多个 用","分开 如：jacky@zhuovi.com:jacky,service@zhuovi.comt:service
        /// </summary>
        public string CCMail { get { return _CCMail; } set { _CCMail = value; } }
        /// <summary>
        /// 发送邮箱地址
        /// </summary>
        private string _FromMail = "";
        /// <summary>
        /// 发送邮箱地址
        /// </summary>
        public string FromMail { get { return _FromMail; } set { _FromMail = value; } }
        /// <summary>
        ///  发送人姓名
        /// </summary>
        private string _FromName = "";
        /// <summary>
        /// 发送人姓名
        /// </summary>
        public string FromName { get { return _FromName; } set { _FromName = value; } }
        /// <summary>
        /// 标题
        /// </summary>
        private string _Subject = "";
        /// <summary>
        /// 标题
        /// </summary>
        public string Subject { get { return _Subject; } set { _Subject = value; } }
        /// <summary>
        /// 标题编码
        /// </summary>
        private string _SubjectEncoding = "";
        /// <summary>
        /// 标题编码
        /// </summary>
        public string SubjectEncoding { get { return _SubjectEncoding; } set { _SubjectEncoding = value; } }
        /// <summary>
        /// 内容
        /// </summary>
        private string _Body = "";
        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get { return _Body; } set { _Body = value; } }
        /// <summary>
        /// 内容编码
        /// </summary>
        private string _BodyEncoding = "";
        /// <summary>
        /// 内容编码
        /// </summary>
        public string BodyEncoding { get { return _BodyEncoding; } set { _BodyEncoding = value; } }
        /// <summary>
        /// 用户帐号
        /// </summary>
        private string _UserName = "";
        /// <summary>
        /// 用户帐号
        /// </summary>
        public string UserName { get { return _UserName; } set { _UserName = value; } }
        /// <summary>
        /// 用户密码
        /// </summary>
        private string _UserPwd = "";
        /// <summary>
        /// 用户密码
        /// </summary>
        public string UserPwd { get { return _UserPwd; } set { _UserPwd = value; } }
        /// <summary>
        /// 邮件服务器
        /// </summary>
        private string _SmtpHost = "";
        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string SmtpHost { get { return _SmtpHost; } set { _SmtpHost = value; } }
        /// <summary>
        /// 邮件服务器端口
        /// </summary>
        private int _SmtpPort = 25;
        /// <summary>
        /// 邮件服务器端口
        /// </summary>
        public int SmtpPort { get { return _SmtpPort; } set { _SmtpPort = value; } }
        /// <summary>
        /// 附件
        /// </summary>
        private string[] _Attachments = null;
        /// <summary>
        /// 附件
        /// </summary>
        public string[] Attachments { get { return _Attachments; } set { _Attachments = value; } }
        /// <summary>
        /// 邮件优先级
        /// </summary>
        private System.Net.Mail.MailPriority _Priority = System.Net.Mail.MailPriority.Normal;
        /// <summary>
        /// 邮件优先级
        /// </summary>
        public System.Net.Mail.MailPriority Priority { get { return _Priority; } set { _Priority = value; } }
        /// <summary>
        /// 发送错误信息
        /// </summary>
        private string _ErrorMessage = "";
        /// <summary>
        /// 发送错误信息
        /// </summary>
        public string ErrorMessage { get { return _ErrorMessage; } set { _ErrorMessage = value; } }
        #endregion

        #region 发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        public Boolean Send()
        {
            using (var message = new MailMessage
            {
                From = new MailAddress(this.FromMail, (this.FromName == "" ? this.UserName : this.FromName))
            })
            {
                message.To.Clear();
                this.ToMail.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Each(destemail =>
                {
                    if (destemail.Contains(":"))
                    {
                        string[] mailInfo = destemail.Split(':');
                        message.To.Add(new MailAddress(mailInfo[0].Trim(), mailInfo[1].Trim()));
                    }
                    else
                    {
                        message.To.Add(destemail);
                    }
                });
                if (this.CCMail.IsNotNullOrEmpty())
                {
                    message.CC.Clear();
                    this.CCMail.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Each(destemail =>
                    {
                        if (destemail.Contains(":"))
                        {
                            string[] mailInfo = destemail.Split(':');
                            message.To.Add(new MailAddress(mailInfo[0].Trim(), mailInfo[1].Trim()));
                        }
                        else
                        {
                            message.CC.Add(destemail);
                        }
                    });
                }
                if (this.BCCMail.IsNotNullOrEmpty())
                {
                    message.Bcc.Clear();
                    this.BCCMail.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Each(destemail =>
                    {
                        if (destemail.Contains(":"))
                        {
                            string[] mailInfo = destemail.Split(':');
                            message.To.Add(new MailAddress(mailInfo[0].Trim(), mailInfo[1].Trim()));
                        }
                        else
                        {
                            message.Bcc.Add(destemail);
                        }
                    });
                }
                message.Subject = this.Subject;//设置邮件主题 
                if (this.BodyEncoding.IsNotNullOrEmpty()) message.SubjectEncoding = Encoding.GetEncoding(this.SubjectEncoding);
                message.IsBodyHtml = true;//设置邮件正文为html格式 
                message.Body = this.Body;//设置邮件内容 

                if (this.BodyEncoding.IsNotNullOrEmpty()) message.BodyEncoding = Encoding.GetEncoding(this.BodyEncoding);
                //添加附件
                Attachments.Each(path =>
                {
                    if (path.IsNotNullOrEmpty())
                        message.Attachments.Add(new Attachment(path));
                });
                //邮件的优先级
                message.Priority = this.Priority;
                //邮件发送人地址
                message.Sender = message.From;
                using (var client = new SmtpClient(this.SmtpHost, this.SmtpPort)
                {
                    //设置发送邮件身份验证方式 
                    //注意如果发件人地址是jacky@zhuovi.com，则用户名是jacky而不是jacky@zhuovi.com 
                    Credentials = new NetworkCredential(this.UserName, this.UserPwd)
                })
                {
                    try
                    {
                        client.Send(message);
                        return true;
                    }
                    catch (Exception e)
                    {
                        this.ErrorMessage = e.Message;
                        return false;
                    }
                }
            }
        }
        #endregion
    }
}