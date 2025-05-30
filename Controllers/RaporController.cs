using Microsoft.AspNetCore.Mvc;
using Kutuphane.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Kutuphane.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        // Sınıf listesini yardımcı olarak getir
        private async Task<List<SelectListItem>> GetSiniflarSelectListAsync()
        {
            var siniflar = await _context.Siniflar.ToListAsync();
            var list = siniflar.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.SinifAdi
            }).ToList();
            list.Insert(0, new SelectListItem { Value = "", Text = "Tüm Sınıflar" });
            return list;
        }

        // Hiç kitap okumayan öğrenciler (sınıf bazlı)
        [HttpGet]
        public async Task<IActionResult> HicKitapOkumayanOgrenciler(int? sinifId)
        {
            ViewBag.Siniflar = await GetSiniflarSelectListAsync();
            ViewBag.SelectedSinif = sinifId;
            var okuyanOgrenciIdler = await _context.OduncKitaplar
                .Select(o => o.OgrenciId)
                .Distinct()
                .ToListAsync();

            var ogrencilerQuery = _context.Ogrenciler
                .Where(o => !okuyanOgrenciIdler.Contains(o.Id));
            if (sinifId.HasValue && sinifId.Value > 0)
                ogrencilerQuery = ogrencilerQuery.Where(o => o.SinifId == sinifId.Value);

            var ogrenciler = await ogrencilerQuery
                .Select(o => new OgrenciViewModel
                {
                    Id = o.Id,
                    Adi = o.OgrenciAdi,
                    Soyadi = o.OgrenciSoyadi,
                    OkulNumarasi = o.OkulNumarasi
                }).ToListAsync();

            return View(ogrenciler);
        }

        // Hiç ödünç verilmeyen kitaplar
        public async Task<IActionResult> HicOduncVerilmeyenKitaplar()
        {
            var oduncVerilenKitapIdler = await _context.OduncKitaplar
                .Select(o => o.KitapId)
                .Distinct()
                .ToListAsync();

            var kitaplar = await _context.Kitaplar
                .Where(k => !oduncVerilenKitapIdler.Contains(k.Id))
                .Select(k => new KitapViewModel
                {
                    Id = k.Id,
                    KitapAdi = k.KitapAdi,
                    Yazar = k.Yazar,
                    Kategori = k.Kategori.KategoriAdi
                }).ToListAsync();

            return View(kitaplar);
        }

        // En çok ödünç verilen kitap türü raporu (sınıf bazlı)
        public async Task<IActionResult> EnCokOduncVerilenKitapTurleri(int? sinifId)
        {
            ViewBag.Siniflar = await GetSiniflarSelectListAsync();
            ViewBag.SelectedSinif = sinifId;
            var odunclar = _context.OduncKitaplar.AsQueryable();
            if (sinifId.HasValue && sinifId.Value > 0)
                odunclar = odunclar.Where(o => o.Ogrenci.SinifId == sinifId.Value);

            var turler = await odunclar
                .Include(o => o.Kitap)
                .ThenInclude(k => k.Kategori)
                .Include(o => o.Ogrenci)
                .GroupBy(o => o.Kitap.Kategori.KategoriAdi)
                .Select(g => new KitapTuruRaporuViewModel
                {
                    Kategori = g.Key,
                    OduncSayisi = g.Count()
                })
                .OrderByDescending(x => x.OduncSayisi)
                .ToListAsync();

            return View(turler);
        }

        // Tarih ve sınıf filtreli en çok okuyan öğrenciler
        public async Task<IActionResult> EnCokOkuyanOgrencilerFiltreli(DateTime? baslangic, DateTime? bitis, int? sinifId)
        {
            ViewBag.Siniflar = await GetSiniflarSelectListAsync();
            ViewBag.SelectedSinif = sinifId;
            var query = _context.OduncKitaplar.Include(o => o.Ogrenci).AsQueryable();
            if (baslangic.HasValue)
                query = query.Where(o => o.OduncAlmaTarihi >= baslangic.Value);
            if (bitis.HasValue)
                query = query.Where(o => o.OduncAlmaTarihi <= bitis.Value);
            if (sinifId.HasValue && sinifId.Value > 0)
                query = query.Where(o => o.Ogrenci.SinifId == sinifId.Value);

            var ogrenciList = await query
                .Where(o => o.TeslimDurumu && o.OgrenciId != null)
                .GroupBy(o => o.OgrenciId)
                .Select(g => new OgrenciOkumaRaporuViewModel
                {
                    OgrenciId = g.Key.Value,
                    OkumaSayisi = g.Count(),
                    OgrenciAdi = g.FirstOrDefault().Ogrenci.OgrenciAdi,
                    OgrenciSoyadi = g.FirstOrDefault().Ogrenci.OgrenciSoyadi
                })
                .OrderByDescending(x => x.OkumaSayisi)
                .Take(10)
                .ToListAsync();

            ViewBag.Baslangic = baslangic?.ToString("yyyy-MM-dd");
            ViewBag.Bitis = bitis?.ToString("yyyy-MM-dd");
            return View(ogrenciList);
        }

        // Tarih ve sınıf filtreli en çok okunan kitaplar
        public async Task<IActionResult> EnCokOkunanKitaplarFiltreli(DateTime? baslangic, DateTime? bitis, int? sinifId)
        {
            ViewBag.Siniflar = await GetSiniflarSelectListAsync();
            ViewBag.SelectedSinif = sinifId;
            var query = _context.OduncKitaplar.Include(o => o.Ogrenci).Include(o => o.Kitap).AsQueryable();
            if (baslangic.HasValue)
                query = query.Where(o => o.OduncAlmaTarihi >= baslangic.Value);
            if (bitis.HasValue)
                query = query.Where(o => o.OduncAlmaTarihi <= bitis.Value);
            if (sinifId.HasValue && sinifId.Value > 0)
                query = query.Where(o => o.Ogrenci.SinifId == sinifId.Value);

            var kitapList = await query
                .Where(o => o.TeslimDurumu && o.KitapId != null)
                .GroupBy(o => o.KitapId)
                .Select(g => new KitapOkunmaRaporuViewModel
                {
                    KitapId = g.Key,
                    OkunmaSayisi = g.Count(),
                    KitapAdi = g.FirstOrDefault().Kitap.KitapAdi,
                    Yazar = g.FirstOrDefault().Kitap.Yazar
                })
                .OrderByDescending(x => x.OkunmaSayisi)
                .Take(10)
                .ToListAsync();

            ViewBag.Baslangic = baslangic?.ToString("yyyy-MM-dd");
            ViewBag.Bitis = bitis?.ToString("yyyy-MM-dd");
            return View(kitapList);
        }
    }
}
