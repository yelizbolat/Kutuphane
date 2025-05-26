using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Kutuphane.Models;
using Kutuphane.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Kutuphane.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly KutuphaneDbContext _context;

    public HomeController(KutuphaneDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var teslimEdilmeyenKitaplar = await _context.OduncKitaplar
            .Where(o => o.OduncAlmaTarihi.AddDays(7) < DateTime.Now && o.TeslimDurumu == false)
            .Include(o => o.Kitap)
            .Include(o => o.Ogrenci)
            .Select(o => new KitapOduncIslemleri
            {
                Id = o.Id,
                OgrenciAdi = o.Ogrenci.OgrenciAdi + " " + o.Ogrenci.OgrenciSoyadi,
                KitapAdi = o.Kitap.KitapAdi,
                AlinmaTarihi = o.OduncAlmaTarihi,
                TeslimTarihi = o.OduncAlmaTarihi.AddDays(7),
                GercekTeslimTarihi = o.TeslimTarihi,
                OduncKitap = o
            })
            .ToListAsync();
        return View(teslimEdilmeyenKitaplar);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}