using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebQuanLySinhVien.Models;
using WebQuanLySinhVien.Models.ViewModels;
using WebQuanLySinhVien.email;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Mail;

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
        public async Task<IActionResult> Index(Mail_Infor mail_Infor)
        {
            if (mail_Infor.selectedIDLop != null)
            {
                string tieude = mail_Infor.TieuDe ?? "Không có tiêu đề";
                string noidung = mail_Infor.NoiDung ?? "Không có nội dung";
                string? filePath = null;

                // Lưu file tạm 
                if (mail_Infor.TepDinhKem != null && mail_Infor.TepDinhKem.Length > 0)
                {
                    var tempPath = Path.Combine(Path.GetTempPath(), mail_Infor.TepDinhKem.FileName);
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        await mail_Infor.TepDinhKem.CopyToAsync(stream);
                    }
                    filePath = tempPath;
                }

                // Lấy danh sách email
                string[] danhSachEmail = _context.SinhViens
                    .Where(sv => sv.MaLop == mail_Infor.selectedIDLop)
                    .Select(sv => sv.Email)
                    .ToArray();

                // Kiểm tra email hợp lệ
                var validEmails = danhSachEmail.Where(e => IsValidEmail(e)).ToList();
                var invalidEmails = danhSachEmail.Except(validEmails).ToList();

                if (validEmails.Count == 0)
                {
                    ViewData["ThongBao"] = "Không có email hợp lệ để gửi.";
                }
                else
                {
                    await _emailSender.SendEmailAsync(validEmails.ToArray(), tieude, noidung, filePath);
                    ViewData["Message"] = "Gửi mail thành công";

                    if (invalidEmails.Count > 0)
                    {
                        ViewData["ThongBao"] = "Một số email không hợp lệ và đã bị bỏ qua: " + string.Join(", ", invalidEmails);
                    }
                }
            }
            else
            {
                ViewData["ThongBao"] = "Hãy chọn lớp để gửi trước.";
            }

            Mail_Infor mail = new Mail_Infor
            {
                DanhSachLop = _context.Lops.OrderBy(l => l.MaLop).Select(l => l.MaLop).ToList(),
                selectedIDLop = mail_Infor.selectedIDLop
            };
            return View(mail);
        }
        [Authorize(Policy = "SinhVienOrAdmin")]
        [HttpGet]
        public IActionResult XinVangHoc()
        {
            var gv = _context.GiangViens
               .Select(g => new {
                   g.Email,
                   DisplayName = g.MaGv + " - " + g.HoTen
               }).ToList();

            ViewBag.MaGV = new SelectList(gv, "Email", "DisplayName");
            return View();
        }
        [Authorize(Policy = "SinhVienOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> XinVangHoc(XinVangHocViewModel model)
        {
            var gv = _context.GiangViens
               .Select(g => new {
                   g.Email,
                   DisplayName = g.MaGv + " - " + g.HoTen
               }).ToList();

            ViewBag.MaGV = new SelectList(gv, "Email", "DisplayName");
            if (!ModelState.IsValid)
            {
                var errorList = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(ms => new
                    {
                        Key = ms.Key,
                        Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                ViewData["ThongBao"] = Newtonsoft.Json.JsonConvert.SerializeObject(errorList);
                return View();//Json(new { success = false, message = Newtonsoft.Json.JsonConvert.SerializeObject(errorList) });
            }
            if (!string.IsNullOrWhiteSpace(model.EmailGiangVien))
            {
                if (!IsValidEmail(model.EmailGiangVien))
                {
                    ViewData["ThongBao"] = "Địa chỉ email giảng viên không hợp lệ, hãy liên hệ với addmin về vấn đề này!";
                    return View();
                }

                await _emailSender.SendDoXinNghiAsync(model);
                ViewData["Message"] = "Gửi mail thành công";
            }
            else
            {
                ViewData["ThongBao"] = "Hãy chọn giảng viên để gửi trước.";
            }
            return View();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
