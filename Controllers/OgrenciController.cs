using Microsoft.AspNetCore.Mvc;
using Kutuphane.Data; // Adjust namespace based on your project structure
using Kutuphane.Models; // Adjust namespace based on your models
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Kutuphane.Controllers
{
    [Authorize]
    public class OgrenciController : Controller
    {
        private readonly KutuphaneDbContext _context;

        public OgrenciController(KutuphaneDbContext context)
        {
            _context = context;
        }

        // GET: Ogrenci
        public async Task<IActionResult> Index()
        {
            var ogrenciler = await _context.Ogrenciler
            .Include(o => o.Sinif) // Sinif navigasyonunu doldur
            .ToListAsync();
            return View(ogrenciler);
        }

        public async Task<IActionResult> SilinenOgrenciler()
        {
            var silinenOgrenciler = await _context.SilinenOgrenciler
                .OrderByDescending(s => s.SilinmeTarihi)
                .ToListAsync();
            return View(silinenOgrenciler);
        }

        public async Task<IActionResult> Ekle()
        {
            ViewBag.Siniflar = new SelectList(await _context.Siniflar.ToListAsync(), "Id", "SinifAdi");
            return View();
        }
        // POST: Ogrenci/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(Ogrenci ogrenci)
        {
             if (ModelState.IsValid)
            {
                _context.Ogrenciler.Add(ogrenci);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
           ViewBag.Siniflar = new SelectList(await _context.Siniflar.ToListAsync(), "Id", "SinifAdi");
            return View(ogrenci);
        }


        [HttpGet]
        public async Task<IActionResult> Guncelle(int id)
        {
            var ogrenci = await _context.Ogrenciler.FindAsync(id);
            if (ogrenci == null) return NotFound();

            ViewBag.Siniflar = new SelectList(await _context.Siniflar.ToListAsync(), "Id", "SinifAdi");
            return View(ogrenci);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(Ogrenci ogrenci)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Siniflar = new SelectList(await _context.Siniflar.ToListAsync(), "Id", "SinifAdi");
                return View(ogrenci);
            }

            _context.Update(ogrenci);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Sil(int id)
        {
            var ogrenci = await _context.Ogrenciler
                .Include(o => o.Sinif)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ogrenci == null)
            {
                return NotFound();
            }

            // Öğrencinin ödünç aldığı kitapları kontrol et
            var oduncKitaplar = await _context.OduncKitaplar
                .Include(o => o.Kitap)
                .Where(o => o.OgrenciId == id && !o.TeslimDurumu)
                .ToListAsync();

            ViewBag.OduncKitaplar = oduncKitaplar;

            return View(ogrenci);
        }

        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilOnayla(int id)
        {
            try
            {
                var ogrenci = await _context.Ogrenciler
                    .Include(o => o.Sinif)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (ogrenci == null)
                {
                    return NotFound();
                }

                // Öğrencinin ödünç aldığı kitapları kontrol et
                var oduncKitaplar = await _context.OduncKitaplar
                    .Where(o => o.OgrenciId == id && !o.TeslimDurumu)
                    .ToListAsync();

                if (oduncKitaplar.Any())
                {
                    TempData["Hata"] = "Bu öğrencinin teslim etmediği kitaplar var. Önce kitapları teslim almalısınız.";
                    return RedirectToAction(nameof(Sil), new { id });
                }

                // Silinen öğrenci bilgilerini kaydet
                var silinenOgrenci = new SilinenOgrenci
                {
                    OgrenciAdi = ogrenci.OgrenciAdi,
                    OgrenciSoyadi = ogrenci.OgrenciSoyadi,
                    OkulNumarasi = ogrenci.OkulNumarasi,
                    SinifAdi = ogrenci.Sinif?.SinifAdi ?? "Belirtilmemiş",
                    SilinmeTarihi = DateTime.Now,
                    SilenKullanici = User.Identity?.Name ?? "Sistem"
                };

                _context.SilinenOgrenciler.Add(silinenOgrenci);

                // Öğrencinin sınıf bağlantısını kaldır
                ogrenci.SinifId = null;
                _context.Update(ogrenci);
                await _context.SaveChangesAsync();

                // Şimdi öğrenciyi sil
                _context.Ogrenciler.Remove(ogrenci);
                await _context.SaveChangesAsync();

                TempData["Basari"] = "Öğrenci başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Öğrenci silinirken bir hata oluştu: " + ex.Message;
                return RedirectToAction(nameof(Sil), new { id });
            }
        }
    }   
}