using Microsoft.AspNetCore.Mvc;
using Kutuphane.Data;
using Kutuphane.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Kutuphane.Controllers
{
    [Authorize]
    public class OduncController : Controller
    {
        private readonly KutuphaneDbContext _context;
        private readonly ILogger<OduncController> _logger;

        public OduncController(KutuphaneDbContext context, ILogger<OduncController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Odunc
        public async Task<IActionResult> Index()
        {
            var oduncKitaplar = await _context.OduncKitaplar
                .Include(o => o.Kitap)
                .Include(o => o.Ogrenci)
                    .ThenInclude(o => o.Sinif)
                .ToListAsync();

            // Her sınıf için öğrenci sayısını hesapla
            var sinifOgrenciSayilari = await _context.Ogrenciler
                .GroupBy(o => o.SinifId)
                .Select(g => new { SinifId = g.Key, OgrenciSayisi = g.Count() })
                .ToDictionaryAsync(x => x.SinifId, x => x.OgrenciSayisi);

            ViewBag.SinifOgrenciSayilari = sinifOgrenciSayilari;

            return View(oduncKitaplar);
        }

        // GET: Odunc/Ver
        public IActionResult Ver()
        {
            return View();
        }

        // Öğrenci arama API'si
        [HttpGet]
        public async Task<IActionResult> OgrenciAra(string arama)
        {
            if (string.IsNullOrWhiteSpace(arama))
                return Json(new List<object>());

            var ogrenciler = await _context.Ogrenciler
                .Where(o => (o.OgrenciAdi + " " + o.OgrenciSoyadi).Contains(arama) || o.OkulNumarasi.Contains(arama))
                .Select(o => new
                {
                    id = o.Id,
                    adSoyad = o.OgrenciAdi + " " + o.OgrenciSoyadi
                })
                .Take(5)
                .ToListAsync();

            return Json(ogrenciler);
        }

        // Kitap arama API'si
        [HttpGet]
        public async Task<IActionResult> KitapAra(string arama)
        {
            if (string.IsNullOrWhiteSpace(arama))
                return Json(new List<object>());

            var kitaplar = await _context.Kitaplar
                .Where(k => k.KitapAdi.Contains(arama) || k.Yazar.Contains(arama))
                .Select(k => new
                {
                    id = k.Id,
                    ad = k.KitapAdi + " - " + k.Yazar
                })
                .Take(5)
                .ToListAsync();

            return Json(kitaplar);
        }

        // POST: Odunc/Ver
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ver([FromBody] OduncKitap oduncKitap)
        {
            try
            {
                _logger.LogInformation("Ver metodu çağrıldı. KitapId: {KitapId}, OgrenciId: {OgrenciId}", 
                    oduncKitap.KitapId, oduncKitap.OgrenciId);

                if (oduncKitap.KitapId == 0 || oduncKitap.OgrenciId == 0)
                {
                    return Json(new { success = false, message = "Lütfen kitap ve öğrenci seçiniz." });
                }

                // Öğrencinin teslim etmediği kitap var mı kontrol et
                var teslimEdilmeyenKitapSayisi = await _context.OduncKitaplar
                    .Where(o => o.OgrenciId == oduncKitap.OgrenciId && !o.TeslimDurumu)
                    .CountAsync();

                if (teslimEdilmeyenKitapSayisi > 0)
                {
                    return Json(new { 
                        success = false, 
                        message = "Bu öğrencinin teslim etmediği kitap bulunmaktadır. Yeni kitap almak için önce diğer kitabı teslim etmelidir." 
                    });
                }

                // Kitabın ödünç verilip verilmediğini kontrol et
                var kitapOduncDurumu = await _context.OduncKitaplar
                    .AnyAsync(o => o.KitapId == oduncKitap.KitapId && !o.TeslimDurumu);

                if (kitapOduncDurumu)
                {
                    return Json(new { success = false, message = "Bu kitap şu anda başka bir öğrencide bulunmaktadır." });
                }

                oduncKitap.OduncAlmaTarihi = DateTime.Now;
                oduncKitap.TeslimTarihi = DateTime.Now.AddDays(15); // 15 günlük ödünç süresi
                oduncKitap.TeslimDurumu = false;

                await _context.OduncKitaplar.AddAsync(oduncKitap);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Kitap başarıyla ödünç verildi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kitap ödünç verme işlemi sırasında hata oluştu");
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }

        // GET: Odunc/TeslimAl/5
        public async Task<IActionResult> TeslimAl(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oduncKitap = await _context.OduncKitaplar
                .Include(o => o.Kitap)
                .Include(o => o.Ogrenci)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (oduncKitap == null)
            {
                return NotFound();
            }

            return View(oduncKitap);
        }

        // POST: Odunc/TeslimAl/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeslimAlOnay(int id)
        {
            try
            {
                var oduncKitap = await _context.OduncKitaplar
                    .Include(o => o.Kitap)
                    .Include(o => o.Ogrenci)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (oduncKitap == null)
                {
                    TempData["Error"] = "Kitap bulunamadı.";
                    return RedirectToAction(nameof(Index));
                }

                if (oduncKitap.TeslimDurumu)
                {
                    TempData["Error"] = "Bu kitap zaten teslim edilmiş.";
                    return RedirectToAction(nameof(Index));
                }

                oduncKitap.TeslimDurumu = true;
                oduncKitap.TeslimTarihi = DateTime.Now;

                _context.Update(oduncKitap);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{oduncKitap.Kitap.KitapAdi} kitabı başarıyla teslim alındı.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kitap teslim alma işlemi sırasında hata oluştu");
                TempData["Error"] = "Kitap teslim alma işlemi sırasında bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 