using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SuperSmartShopping.DAL.Common
{
    public static class EmailManager
    {
        public static string SendForgetPasswordEmail(string emailTo, string emailFrom, string subject, string template)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(emailFrom);
                mail.IsBodyHtml = true;
                mail.To.Add(emailTo);
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendToBccEmails"].ToString()) == true)
                {
                    string[] bccEmiils = ConfigurationManager.AppSettings["bccEmails"].Split(',');
                    foreach (var item in bccEmiils)
                    {
                        mail.Bcc.Add(item);
                    }
                }
                mail.Subject = subject;
                mail.Priority = MailPriority.High;
                mail.IsBodyHtml = true;
                mail.Body = template;
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Send(mail);
                return "1";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}
