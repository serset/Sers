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

using Newtonsoft.Json;

namespace Sers.FrameWork.Util.Mail
{
    public class MailSenderAccount
    {

        [JsonIgnore]
        private string _From;
        /// <summary>
        /// 发件人邮件地址,例：123456789@qq.com 
        /// </summary>
        [JsonProperty]
        public string address
        {
            set { _From = value; try { _UserName = value.Remove(value.LastIndexOf('@')); } catch { _UserName = value; } }
            get { return _From; }
        }

        /// <summary>
        ///  用户名，注意如果发件人地址是abc@def.com ，则用户名是abc 而不是abc@def.com 
        /// </summary>
        private string _UserName;
        [JsonIgnore]
        public string UserName { get { return _UserName; } }

        /// <summary>
        /// 密码(发件人的邮箱登陆密码或授权码)
        /// </summary>
        [JsonProperty]
        public string password{ get; set; }


        /// <summary>
        /// 发送邮件的服务器地址或IP,例："smtp.qq.com"
        /// </summary>
        [JsonProperty]
        public string host { get; set; }

     
        public MailSenderAccount(string address, string password, string host)
        {
            this.address = address; this.password = password; this.host = host;
        }
        public MailSenderAccount() { }

    }
}
