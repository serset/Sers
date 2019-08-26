#region << 版 本 注 释 - v1 >>
/*
 * ========================================================================
 *  
 * 作者：lith
 * 时间：2019-05-10
 * 邮箱：sersms@163.com
 * 
 * ========================================================================
*/
#endregion

using System.Net;
using System.Net.Mail;

namespace Sers.FrameWork.Util.Mail
{
    public  class MailHelp
    {
        #region Send 指定发件人账户



        /// <summary>
        /// 发送邮件,失败则抛异常
        /// </summary>
        /// <param name="mailInfo"></param>
        /// <param name="tos">收件人地址数组，例：123456789@qq.com</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="isBodyHtml">indicating whether the mail message body is in Html.</param>
        /// <param name="attachmentsPath">附件本地地址数组，例如："d://test.txt"</param>
        public static void Send(MailSenderAccount mailInfo, string[] tos, string subject, string body,bool isBodyHtml=false, string[] attachmentsPath=null)
        {
            MailMessage message = new MailMessage();

            #region 附件
            if (null != attachmentsPath && attachmentsPath.Length > 0)
            {
                foreach (string attachmentPath in attachmentsPath)
                {
                    //attachmentPath="d://test.txt"
                    message.Attachments.Add(new Attachment(attachmentPath));
                }
            }
            #endregion

            message.From = new MailAddress(mailInfo.address);

            //收件人邮箱地址是多个
            foreach (string to in tos)
                message.To.Add(to);

            message.Subject = subject;
            message.Body = body;

            //是否为html格式 
            message.IsBodyHtml = isBodyHtml;

            //发送邮件的优先等级 
            message.Priority = MailPriority.Normal;

            SmtpClient client = new SmtpClient(mailInfo.host);

            client.Credentials = new NetworkCredential(mailInfo.UserName, mailInfo.password);

            //发送邮件
            client.Send(message);
        }

 
        #endregion


        #region Send

        private static readonly MailSenderAccount _MailSenderAccount = Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<MailSenderAccount>("MailSenderAccount");

        /// <summary>
        /// 使用系统账户发送邮件,失败则抛异常
        /// </summary>
        /// <param name="tos">收件人地址数组，例：123456789@qq.com</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="isBodyHtml">indicating whether the mail message body is in Html.</param>
        /// <param name="attachmentsPath">附件本地地址数组，例如："d://test.txt"</param>
        public static void Send(string[] tos, string subject, string body, bool isBodyHtml = false, string[] attachmentsPath = null)
        {
            Send(_MailSenderAccount, tos, subject, body, isBodyHtml, attachmentsPath);
        }


        /// <summary>
        /// 使用系统账户发送邮件,发送邮件,失败则抛异常
        /// </summary>   
        /// <param name="to">收件人地址数组，例：123456789@qq.com</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="isBodyHtml">indicating whether the mail message body is in Html.</param>
        /// <param name="attachmentsPath">附件本地地址数组，例如："d://test.txt"</param>
        public static void Send(string to, string subject, string body, bool isBodyHtml = false, string[] attachmentsPath = null)
        {
            Send(_MailSenderAccount, new []{ to }, subject, body, isBodyHtml,attachmentsPath);
        }
        #endregion


    }
}
