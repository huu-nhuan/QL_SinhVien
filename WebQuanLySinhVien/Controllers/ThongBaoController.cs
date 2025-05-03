using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebQuanLySinhVien.Models;
using WebQuanLySinhVien.Models.ViewModels;
using WebQuanLySinhVien.email;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                if (mail_Infor.TepDinhKem != null && mail_Infor.TepDinhKem.Length > 0)
                {
                    // Lưu file tạm
                    var tempPath = Path.Combine(Path.GetTempPath(), mail_Infor.TepDinhKem.FileName);
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        await mail_Infor.TepDinhKem.CopyToAsync(stream);
                    }
                    filePath = tempPath;
                }

                string[] danhSachEmail = _context.SinhViens.Where(sv => sv.MaLop == mail_Infor.selectedIDLop).Select(sv => sv.Email).ToArray(); //lấy danh sách email của các sv trong lớp
                await _emailSender.SendEmailAsync(danhSachEmail, tieude, noidung, filePath);
                ViewData["Message"] = "Gửi mail thành công";
            }
            else
            {
                ViewData["ThongBao"] = "Hãy chọn lớp để gửi trước";
            }
            Mail_Infor mail = new Mail_Infor
            {
                DanhSachLop = _context.Lops.OrderBy(l => l.MaLop).Select(l => l.MaLop).ToList(),
                selectedIDLop = mail_Infor.selectedIDLop
            };
            return View(mail);
        }

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
            if (model.EmailGiangVien != null)
            {
                await _emailSender.SendDoXinNghiAsync(model);
                ViewData["Message"] = "Gửi mail thành công";
            }
            else
            {
                ViewData["ThongBao"] = "Hãy chọn giảng viên để gửi trước";
            }
            return View();
        }
    }
}
