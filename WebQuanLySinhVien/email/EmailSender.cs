using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace WebQuanLySinhVien.email
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "kurowa820@gmail.com";
            var pw = "peat asib viku pjtm";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw),
            };
            return client.SendMailAsync(
                new MailMessage(from: mail,
                                to: email,
                                subject,
                                message));  
        }
    }
}
