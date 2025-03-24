
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebQuanLySinhVien.email;
using WebQuanLySinhVien.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace WebQuanLySinhVien.Controllers
{
    public class AccessController : Controller
    {
        QuanLySinhVienContext db = new QuanLySinhVienContext();

        private readonly IEmailSender _emailSender;
        public AccessController(IEmailSender emailSender)
        {
            this._emailSender = emailSender;
        }
        //Đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(Taikhoan tk)
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                var u = db.Taikhoans.Where(x => x.TenDangNhap.Equals(tk.TenDangNhap) 
                && x.MatKhau.Equals(tk.MatKhau)).FirstOrDefault();
                if (u != null)
                {
                    HttpContext.Session.SetString("TenDangNhap", u.TenDangNhap.ToString());
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, u.TenDangNhap),
                        new Claim("Id", u.IdTk.ToString()),
                        new Claim("Role", u.VaiTro.ToString())  // Lưu vai trò vào Claims
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        //Đăng Ký
        [HttpGet]
        public IActionResult Registration()
        {  
            return View(); 
        }

        [HttpPost]
        public IActionResult Registration(string email, Taikhoan tk)
        {
            var sv = db.SinhViens.Where(x => x.Email.Equals(email)).FirstOrDefault();
            var gv = db.GiangViens.Where(y => y.Email.Equals(email)).FirstOrDefault();
            if (sv == null && gv == null) return View();
            else if (gv != null)
            {
                tk.IdTk = TaoID();
                tk.VaiTro = 2;
                db.Taikhoans.Add(tk);
                gv.IdTk = tk.IdTk;

                db.SaveChanges();
            }
            else if (sv != null)
            {
                tk.IdTk = TaoID();
                tk.VaiTro = 3;
                db.Taikhoans.Add(tk);
                sv.IdTk = tk.IdTk;

                db.SaveChanges();
            }
            //var receiver = "soroje2090@fanicle.com";
            //var subject = "test";
            //var message = "test";

            //await _emailSender.SendEmailAsync(receiver, subject, message);
            return RedirectToAction("Login", "Access");
        }

        //Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("TenDangNhap");
            return RedirectToAction("Login", "Access");
        }

        [HttpGet]
        public IActionResult EmailConfirm()
        {
            ViewData["Email"] = TempData["Email"];
            return View();
        }

        [HttpPost]
        public IActionResult EmailConfirm( string newpass, string verificationCode)
        {
            string email = HttpContext.Session.GetString("email");
            var sv = db.SinhViens.Where(x => x.Email.Equals(email)).FirstOrDefault();
            var gv = db.GiangViens.Where(y => y.Email.Equals(email)).FirstOrDefault();
            if (sv == null && gv == null)
                return BadRequest("Email không tồn tại trong hệ thống");
            if (verificationCode != HttpContext.Session.GetString("MaXacNhan"))
                return BadRequest("Mã xác nhận không chính xác");
            else if (gv != null && gv.IdTk != null)
            {
                var tk = db.Taikhoans.Where(x => x.IdTk.Equals(gv.IdTk)).FirstOrDefault();
                tk.MatKhau = newpass;
                HttpContext.Session.Remove("MaXacNhan");
                HttpContext.Session.Remove("email");
                db.SaveChanges();
            }
            else if (sv != null && sv.IdTk != null)
            {
                var tk = db.Taikhoans.Where(x => x.IdTk.Equals(sv.IdTk)).FirstOrDefault();
                tk.MatKhau = newpass;
                HttpContext.Session.Remove("MaXacNhan");
                HttpContext.Session.Remove("email");
                db.SaveChanges();
            }
            else return BadRequest("Bạn chưa tạo tài khoản. Hãy tạo một tài khoản mới");

            return RedirectToAction("Login", "Access");
        }

        [HttpPost]
        public IActionResult GuiMaXacNhan( string email)
        {
            var sv = db.SinhViens.Where(x => x.Email.Equals(email)).FirstOrDefault();
            var gv = db.GiangViens.Where(y => y.Email.Equals(email)).FirstOrDefault();
            if (sv == null && gv == null)
                return BadRequest("Email không tồn tại trong hệ thống");
            TaoMaXacNhan();
            string[] receiver = [email];
            var subject = "Mã xác nhận";
            var message = "Mã xác nhận của bạn là: " + HttpContext.Session.GetString("MaXacNhan");

            TempData["Email"] = email;
            HttpContext.Session.SetString("email", email);

            _emailSender.SendEmailAsync(receiver, subject, message);
            return RedirectToAction("EmailConfirm", "Access");
        }

        public string TaoID()
        {
            string randomid;
            do
            {
                randomid = TaoIDRandom();
            }
            while (db.Taikhoans.Any(x=>x.IdTk==randomid));
            return randomid;
        }

        public string TaoIDRandom()
        {
            int length = 6;
            const string digits = "0123456789";
            StringBuilder result = new StringBuilder(length);
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(digits.Length);
                result.Append(digits[index]);
            }

            return result.ToString();
        }

        public void TaoMaXacNhan()
        {
            int length = 4;
            const string digits = "0123456789";
            StringBuilder result = new StringBuilder(length);
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(digits.Length);
                result.Append(digits[index]);
            }
            HttpContext.Session.SetString("MaXacNhan", result.ToString());
        }
    }
}
