using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Kutuphane.Models;
using Kutuphane.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Kutuphane.Models.ViewModels;

namespace Kutuphane.Controllers;

[Authorize]
public class SinifController : Controller
{
    private readonly KutuphaneDbContext _context;

    public SinifController(KutuphaneDbContext context)
    {
        _context = context;
    }

    // GET: Sinif
    public async Task<IActionResult> Index()
    {
        var siniflar = await _context.Siniflar
            .Include(s => s.Ogrenciler)
            .Select(s => new SinifViewModel
            {
                Id = s.Id,
                SinifAdi = s.SinifAdi,
                OgrenciSayisi = s.Ogrenciler.Count
            })
            .ToListAsync();

        return View(siniflar);
    }

    // GET: Sinif/Create
    public IActionResult Ekle()
    {
        return View();
    }

    // POST: Sinif/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(Sinif sinif)
    {
        if (ModelState.IsValid)
        {
            await _context.Siniflar.AddAsync(sinif);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Sınıf başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }
        return View(sinif);
    }

    // GET: Sinif/Edit/5
    public async Task<IActionResult> Guncelle(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var sinif = await _context.Siniflar.FindAsync(id);
        if (sinif == null)
        {
            return NotFound();
        }
        return View(sinif);
    }

    // POST: Sinif/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guncelle(Sinif sinif)
    {
        if (ModelState.IsValid)
        {
            _context.Siniflar.Update(sinif);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Sınıf başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }
        return View(sinif);
    }

    // GET: Sinif/Delete/5
    public async Task<IActionResult> Sil(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var sinif = await _context.Siniflar.FindAsync(id);
        if (sinif != null)
        {
            _context.Siniflar.Remove(sinif);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Sınıf başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["Error"] = "Sınıf bulunamadı.";
            return NotFound();
        }   
    }   
}