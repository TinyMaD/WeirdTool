using System.Net;
using System.Net.Mail;
using System.Text;

namespace WeirdTool.Services
{
    public class Notify
    {
        private static readonly string _smtpHost = AppSettings.GetConfig("SmtpHost");
        private static readonly string _uid = AppSettings.GetConfig("Uid");
        private static readonly string _pwd = AppSettings.GetConfig("Pwd");

        public void SendEmail(MailMessage msg)
        {
            string smtpHost = _smtpHost;
            string uid = _uid;//发件人邮箱地址@符号前面的字符tom@dddd.com,则为"tom" 
            string pwd = _pwd;//发件人邮箱的密码 

            MailAddress from = new(uid, "奇葩工具", Encoding.UTF8);
            msg.From = from;

            //实例化SmtpClient  
            SmtpClient smtpClient = new(smtpHost, 25)
            {
                //设置验证发件人身份的凭据  
                Credentials = new NetworkCredential(uid, pwd),
            };
            //发送  
            smtpClient.Send(msg);
        }
    }
}
