using Microsoft.AspNetCore.Mvc;
using Kutuphane.Models;
using Kutuphane.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane.Controllers
{
    public class AccountController : Controller
    {
        private readonly KutuphaneDbContext _context;
        public AccountController(KutuphaneDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string kullaniciAdi, string sifre, string sifreTekrar)
        {
            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre))
            {
                ModelState.AddModelError("", "Kullanıcı adı ve şifre gereklidir.");
                return View();
            }

            if (sifre != sifreTekrar)
            {
                ModelState.AddModelError("", "Şifreler eşleşmiyor.");
                return View();
            }

            // Kullanıcı adının benzersiz olduğunu kontrol et
            if (await _context.Kullanicilar.AnyAsync(x => x.KullaniciAdi == kullaniciAdi))
            {
                ModelState.AddModelError("", "Bu kullanıcı adı zaten kullanılıyor.");
                return View();
            }

            // Yeni kullanıcı oluştur
            var yeniKullanici = new Kullanici
            {
                KullaniciAdi = kullaniciAdi,
                Sifre = sifre, // Gerçek uygulamada şifre hash'lenmelidir
                Rol = "Kullanici"
            };

            _context.Kullanicilar.Add(yeniKullanici);
            await _context.SaveChangesAsync();

            // Kullanıcıyı otomatik olarak giriş yap
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, yeniKullanici.KullaniciAdi),
                new Claim(ClaimTypes.Role, yeniKullanici.Rol)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string kullaniciAdi, string sifre, string? returnUrl = null)
        {
            var user = await _context.Kullanicilar.FirstOrDefaultAsync(x => x.KullaniciAdi == kullaniciAdi && x.Sifre == sifre);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.KullaniciAdi),
                new Claim(ClaimTypes.Role, user.Rol)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["LogoutMessage"] = "Başarıyla çıkış yaptınız.";
            return RedirectToAction("Login");
        }
    }
} 