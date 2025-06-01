using Microsoft.AspNetCore.Mvc;
using Kutuphane.Data; // Adjust namespace based on your project structure
using Kutuphane.Models; // Adjust namespace based on your models
using Kutuphane.Models.ViewModels; // Add this line for ViewModels
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
            var aktifOgrenciler = await _context.Ogrenciler
                .Include(o => o.Sinif)
                .Where(o => o.Aktif)
                .ToListAsync();

            var pasifOgrenciler = await _context.Ogrenciler
                .Include(o => o.Sinif)
                .Where(o => !o.Aktif)
                .ToListAsync();

            ViewBag.PasifOgrenciler = pasifOgrenciler;
            return View(aktifOgrenciler);
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
                // Okul numarasının benzersiz olup olmadığını kontrol et
                var numaraKontrol = await _context.Ogrenciler
                    .AnyAsync(o => o.OkulNumarasi == ogrenci.OkulNumarasi);

                if (numaraKontrol)
                {
                    ModelState.AddModelError("OkulNumarasi", "Bu okul numarası başka bir öğrenci tarafından kullanılıyor.");
                    ViewBag.Siniflar = new SelectList(await _context.Siniflar.ToListAsync(), "Id", "SinifAdi");
                    return View(ogrenci);
                }

                _context.Ogrenciler.Add(ogrenci);
                await _context.SaveChangesAsync();
                TempData["Basari"] = "Öğrenci başarıyla eklendi.";
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

            // Okul numarasının başka bir öğrenci tarafından kullanılıp kullanılmadığını kontrol et
            var numaraKontrol = await _context.Ogrenciler
                .AnyAsync(o => o.OkulNumarasi == ogrenci.OkulNumarasi && o.Id != ogrenci.Id);

            if (numaraKontrol)
            {
                ModelState.AddModelError("OkulNumarasi", "Bu okul numarası başka bir öğrenci tarafından kullanılıyor.");
                ViewBag.Siniflar = new SelectList(await _context.Siniflar.ToListAsync(), "Id", "SinifAdi");
                return View(ogrenci);
            }

            try
            {
                _context.Update(ogrenci);
                await _context.SaveChangesAsync();
                TempData["Basari"] = "Öğrenci başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Ogrenciler.AnyAsync(o => o.Id == ogrenci.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
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

                // Öğrenciyi silmek yerine pasif hale getir
                ogrenci.Aktif = false;

                // Silinen öğrenci kaydını tut
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
                _context.Update(ogrenci);
                await _context.SaveChangesAsync();

                TempData["Basari"] = "Öğrenci başarıyla pasif hale getirildi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "Öğrenci silinirken bir hata oluştu: " + ex.Message;
                return RedirectToAction(nameof(Sil), new { id });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GeriAl(int id)
        {
            var ogrenci = await _context.Ogrenciler.FindAsync(id);
            if (ogrenci != null)
            {
                ogrenci.Aktif = true;
                _context.Ogrenciler.Update(ogrenci);
                await _context.SaveChangesAsync();
                TempData["Basari"] = "Öğrenci başarıyla geri alındı.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detaylar(int id)
        {
            var ogrenci = await _context.Ogrenciler
                .Include(o => o.Sinif)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ogrenci == null)
            {
                return NotFound();
            }

            // Öğrencinin ödünç aldığı kitapları getir
            var oduncKitaplar = await _context.OduncKitaplar
                .Include(o => o.Kitap)
                .Where(o => o.OgrenciId == id)
                .OrderByDescending(o => o.OduncAlmaTarihi)
                .ToListAsync();

            var viewModel = new OgrenciDetayViewModel
            {
                Id = ogrenci.Id,
                OgrenciAdi = ogrenci.OgrenciAdi,
                OgrenciSoyadi = ogrenci.OgrenciSoyadi,
                OkulNumarasi = ogrenci.OkulNumarasi,
                SinifAdi = ogrenci.Sinif?.SinifAdi ?? "Belirtilmemiş",
                OduncKitaplar = oduncKitaplar.Select(o => new OgrenciOduncKitapViewModel
                {
                    KitapAdi = o.Kitap.KitapAdi,
                    Yazar = o.Kitap.Yazar,
                    AlinmaTarihi = o.OduncAlmaTarihi,
                    TeslimTarihi = o.TeslimTarihi,
                    TeslimDurumu = o.TeslimDurumu
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IzactifGuncelle()
        {
            var silinenOgrenciNumaralari = await _context.SilinenOgrenciler.Select(x => x.OkulNumarasi).ToListAsync();
            var silinenOgrenciler = await _context.Ogrenciler.Where(x => silinenOgrenciNumaralari.Contains(x.OkulNumarasi)).ToListAsync();
            foreach (var ogr in silinenOgrenciler)
            {
                ogr.Aktif = false;
            }
            await _context.SaveChangesAsync();
            TempData["Basari"] = "Silinen öğrenciler pasif hale getirildi.";
            return RedirectToAction("Index");
        }
    }   
}