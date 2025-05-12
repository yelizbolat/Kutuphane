using Kutuphane.Models;
using Microsoft.EntityFrameworkCore;

namespace Kutuphane.Data
{
    public class KutuphaneDbContext : DbContext
    {
        public KutuphaneDbContext(DbContextOptions<KutuphaneDbContext> options) : base(options)
        {
        }
        public DbSet<Ogrenci> Ogrenciler { get; set; }
        public DbSet<Sinif> Siniflar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Kitap> Kitaplar { get; set; }
        public DbSet<OduncKitap> OduncKitaplar { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ogrenci>()
                .HasIndex(o => o.OkulNumarasi)
                .IsUnique(); // Burada benzersizlik sağlanıyor

            base.OnModelCreating(modelBuilder);
        }
    }
}