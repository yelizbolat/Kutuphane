using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Kutuphane.Models;
using Kutuphane.Data;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane.Controllers;
public class HomeController : Controller
{
    private readonly KutuphaneDbContext _context;

    public HomeController(KutuphaneDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
         // Teslim süresi 15 günü geçen ödünç alınmış kitapları sorgula
        var suresiGecenOgrenciler = _context.Ogrenciler
            .Where(o => o.KitapOduncTarihi.HasValue && 
                        o.KitapGeriGetirildi == false && 
                        o.KitapOduncTarihi.Value.AddDays(15) < DateTime.Now)
            .ToList();

        return View(suresiGecenOgrenciler);
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