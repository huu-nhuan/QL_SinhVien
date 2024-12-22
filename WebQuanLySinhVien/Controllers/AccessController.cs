using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebQuanLySinhVien.Models;

namespace WebQuanLySinhVien.Controllers
{
    public class AccessController : Controller
    {
        QuanLySinhVienContext db = new QuanLySinhVienContext();
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
        public IActionResult Login(Taikhoan tk)
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                var u = db.Taikhoans.Where(x => x.TenDangNhap.Equals(tk.TenDangNhap) && x.MatKhau.Equals(tk.MatKhau)).FirstOrDefault();
                if (u != null)
                {
                    HttpContext.Session.SetString("TenDangNhap", u.TenDangNhap.ToString());
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
        public IActionResult Registration(string sdt, Taikhoan tk)
        {
            var sv = db.SinhViens.Where(x=>x.Sdt.Equals(sdt)).FirstOrDefault();
            var gv = db.GiangViens.Where(y => y.Sdt.Equals(sdt)).FirstOrDefault();
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
                sv.IdTk= tk.IdTk;

                db.SaveChanges();
            }
            return RedirectToAction("Login", "Access");
        }

        //Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("TenDangNhap");
            return RedirectToAction("Login", "Access");
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
    }
}
