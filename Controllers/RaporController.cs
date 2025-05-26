using Microsoft.AspNetCore.Mvc;
using Kutuphane.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Kutuphane.Controllers
{
    [Authorize]
    public class RaporController : Controller
    {
        private readonly KutuphaneDbContext _context;

        public RaporController(KutuphaneDbContext context)
        {
            _context = context;
        }

        // GET: Rapor
        public IActionResult Index()
        {
            return View();
        }

        // GET: Rapor/OduncListesi
        public async Task<IActionResult> OduncListesi()
        {
            var oduncler = await _context.OduncKitaplar
                .Include(o => o.Kitap)
                .Include(o => o.Ogrenci)
                .ToListAsync();

            return View(oduncler);
        }

        // En çok okuyan öğrenciler
        public async Task<IActionResult> EnCokOkuyanOgrenciler()
        {
            var ogrenciList = await _context.OduncKitaplar
                .Where(o => o.TeslimDurumu)
                .GroupBy(o => o.OgrenciId)
                .Select(g => new {
                    OgrenciId = g.Key,
                    OkumaSayisi = g.Count()
                })
                .OrderByDescending(x => x.OkumaSayisi)
                .Take(10)
                .ToListAsync();

            var detaylar = await _context.Ogrenciler
                .Where(o => ogrenciList.Select(x => x.OgrenciId).Contains(o.Id))
                .ToListAsync();

            var sonuc = ogrenciList.Select(x => new {
                Ogrenci = detaylar.FirstOrDefault(o => o.Id == x.OgrenciId),
                OkumaSayisi = x.OkumaSayisi
            }).ToList();

            return View(sonuc);
        }

        // En çok okunan kitaplar
        public async Task<IActionResult> EnCokOkunanKitaplar()
        {
            var kitapList = await _context.OduncKitaplar
                .Where(o => o.TeslimDurumu)
                .GroupBy(o => o.KitapId)
                .Select(g => new {
                    KitapId = g.Key,
                    OkunmaSayisi = g.Count()
                })
                .OrderByDescending(x => x.OkunmaSayisi)
                .Take(10)
                .ToListAsync();

            var detaylar = await _context.Kitaplar
                .Where(k => kitapList.Select(x => x.KitapId).Contains(k.Id))
                .ToListAsync();

            var sonuc = kitapList.Select(x => new {
                Kitap = detaylar.FirstOrDefault(k => k.Id == x.KitapId),
                OkunmaSayisi = x.OkunmaSayisi
            }).ToList();

            return View(sonuc);
        }
    }
}
