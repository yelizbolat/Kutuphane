using Microsoft.AspNetCore.Mvc;
using Kutuphane.Data;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane.Controllers
{
    public class RaporController : Controller
    {
        private readonly KutuphaneDbContext _context;

        public RaporController(KutuphaneDbContext context)
        {
            _context = context;
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
    }
}
