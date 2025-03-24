using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace WebQuanLySinhVien.email
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string[] email, string subject, string message)
        {
            var mail = "kurowa820@gmail.com";
            var pw = "peat asib viku pjtm";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw),
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(mail),
                Subject = subject,
                Body = message,
            };

            foreach (var e in email)
            {
                mailMessage.To.Add(e);
            }

            return client.SendMailAsync(mailMessage);
        }
    }
}
