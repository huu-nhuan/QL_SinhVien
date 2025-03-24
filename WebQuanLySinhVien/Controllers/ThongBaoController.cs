using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebQuanLySinhVien.Models;
using WebQuanLySinhVien.Models.ViewModels;
using WebQuanLySinhVien.email;

namespace WebQuanLySinhVien.Controllers
{
    public class ThongBaoController : Controller
    {
        private readonly QuanLySinhVienContext _context;
        private readonly IEmailSender _emailSender;

        public ThongBaoController(QuanLySinhVienContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [Authorize(Policy = "GiangVienOrAdmin")]
        [HttpGet]
        public IActionResult Index()
        {
            Mail_Infor mail_Infor = new Mail_Infor
            {
                DanhSachLop = _context.Lops.OrderBy(l => l.MaLop).Select(l => l.MaLop).ToList()
            };
            return View(mail_Infor);
        }
        [Authorize(Policy = "GiangVienOrAdmin")]
        [HttpPost]
        public IActionResult Index(Mail_Infor mail_Infor)
        {
            if(mail_Infor.selectedIDLop!=null)
            {
                string tieude = mail_Infor.TieuDe ?? "Không có tiêu đề";
                string noidung = mail_Infor.NoiDung ?? "Không có nội dung";
                    
                string[] danhSachEmail = _context.SinhViens.Where(sv => sv.MaLop== mail_Infor.selectedIDLop).Select(sv =>sv.Email).ToArray(); //lấy danh sách email của các sv trong lớp
                _emailSender.SendEmailAsync(danhSachEmail, tieude, noidung);
                ViewData["Message"] = "Gửi mail thành công";
            }
            if (ViewData["Message"]==null) ViewData["Message"] = "Gửi mail thất bại";
            Mail_Infor mail = new Mail_Infor
            {
                DanhSachLop = _context.Lops.OrderBy(l => l.MaLop).Select(l => l.MaLop).ToList(),
                selectedIDLop = mail_Infor.selectedIDLop
            };
            return View(mail);
        }
    }
}
