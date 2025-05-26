using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models
{
    public class Kategori
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        public string KategoriAdi { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        public bool Aktif { get; set; } = true;

        public ICollection<Kitap>? Kitaplar { get; set; }
    }
} 