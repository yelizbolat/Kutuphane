using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models
{
    public class Kullanici
    {
        public int Id { get; set; }

        [Required]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required]
        public string Sifre { get; set; } = string.Empty;

        public string Rol { get; set; } = "Kullanici"; // veya "Admin"
    }
} 