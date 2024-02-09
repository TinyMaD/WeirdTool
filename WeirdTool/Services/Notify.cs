using System.Net;
using System.Net.Mail;
using System.Text;

namespace WeirdTool.Services
{
    public class Notify
    {
        private static readonly string _smtpHost = Environment.GetEnvironmentVariable("smtp_host") ?? AppSettings.GetConfig("SmtpHost");
        private static readonly string _uid = Environment.GetEnvironmentVariable("uid") ?? AppSettings.GetConfig("Uid");
        private static readonly string _pwd = Environment.GetEnvironmentVariable("pwd") ?? AppSettings.GetConfig("Pwd");
        private static readonly string _email = Environment.GetEnvironmentVariable("email") ?? AppSettings.GetConfig("Email");

        public void SendEmail(string msg)
        {
            MailMessage mailMsg = new(_uid, _email)
            {
                Subject = "王者荣耀活动",//邮件主题  
                IsBodyHtml = true,
                Body = msg//邮件正文  
            };
            MailAddress from = new(_uid, "奇葩工具", Encoding.UTF8);
            mailMsg.From = from;

            //实例化SmtpClient  
            SmtpClient smtpClient = new(_smtpHost, 587)
            {
                //设置验证发件人身份的凭据  
                Credentials = new NetworkCredential(_uid, _pwd),
            };
            //发送  
            smtpClient.Send(mailMsg);
        }
    }
}
