using WebQuanLySinhVien.Models.ViewModels;

namespace WebQuanLySinhVien.email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string[] email, string subject, string message, string? filepatch);
        Task SendDoXinNghiAsync(XinVangHocViewModel model);
    }
}
