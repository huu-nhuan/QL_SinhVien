using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using WebQuanLySinhVien.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace WebQuanLySinhVien.email
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string[] email, string subject, string messageContent, string? filePath = null)
        {
            var mail = "kurowa820@gmail.com";
            var pw = "peat asib viku pjtm";
                
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw),
            };

            string htmlBody = $@"
                <html>
                <body style='font-family:Arial,sans-serif; background-color:#f9f9f9; padding:20px;'>
                    <div style='max-width:600px; margin:auto; background:white; padding:20px; border-radius:8px; box-shadow:0 2px 4px rgba(0,0,0,0.1);'>
                        <h2 style='color:#2E86C1; border-bottom:1px solid #ccc; padding-bottom:10px;'>{subject}</h2>
                        <p style='font-size:16px; color:#333;'>{messageContent}</p>
                        <hr style='margin-top:30px;' />
                        <p style='font-size:14px; color:gray;'>Đây là email tự động từ hệ thống Quản lý Sinh viên. Vui lòng không trả lời email này.</p>
                    </div>
                </body>
                </html>
            ";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(mail),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            foreach (var e in email)
            {
                mailMessage.To.Add(e);
            }

            // Nếu có file đính kèm, kiểm tra dung lượng và đính kèm
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                const long maxFileSize = 20 * 1024 * 1024; // 20 MB
                var fileInfo = new FileInfo(filePath);

                if (fileInfo.Length > maxFileSize)
                {
                    throw new Exception($"File đính kèm quá lớn ({fileInfo.Length / (1024 * 1024)} MB). Vui lòng chọn file nhỏ hơn 20 MB.");
                }

                mailMessage.Attachments.Add(new Attachment(filePath));
            }

            return client.SendMailAsync(mailMessage);
        }

        public Task SendDoXinNghiAsync(XinVangHocViewModel model)
        {
            var mail = "kurowa820@gmail.com";
            var pw = "peat asib viku pjtm";
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw),
            };
            string htmlBody = $@"
            <html><body>
                <h2>Đơn xin phép vắng học</h2>
                <p><strong>MSSV:</strong> {model.MSSV}</p>
                <p><strong>Họ và tên:</strong> {model.HoTen}</p>
                <p><strong>Lớp:</strong> {model.Lop}</p>
                <p><strong>Ngày nghỉ:</strong> {model.NgayNghi:dd/MM/yyyy}</p>
                <p><strong>Từ tiết:</strong> {model.TietBatDau} đến tiết {model.TietKetThuc}</p>
                <p><strong>Lý do:</strong> {model.LyDo}</p>
                <p>Trân trọng,<br /><strong>{model.HoTen}</strong></p>
            </body></html>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(mail),
                Subject = $"[Đơn xin vắng học] - {model.HoTen} ({model.MSSV})",
                Body = htmlBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(model.EmailGiangVien);

            return client.SendMailAsync(mailMessage);
        }
    }
}
