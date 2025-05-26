using Microsoft.EntityFrameworkCore;
using Kutuphane.Models;

namespace Kutuphane.Data
{
    public class KutuphaneDbContext : DbContext
    {
        public KutuphaneDbContext(DbContextOptions<KutuphaneDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ogrenci> Ogrenciler { get; set; }
        public DbSet<Sinif> Siniflar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Kitap> Kitaplar { get; set; }
        public DbSet<OduncKitap> OduncKitaplar { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }

        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kitap>()
                .HasOne(k => k.Kategori)
                .WithMany(kat => kat.Kitaplar)
                .HasForeignKey(k => k.KategoriId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ogrenci>()
                .HasOne(o => o.Sinif)
                .WithMany(s => s.Ogrenciler)
                .HasForeignKey(o => o.SinifId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}