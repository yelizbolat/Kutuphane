using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models
{
    public class SilinenOgrenci
    {
        public int Id { get; set; }
        public string OgrenciAdi { get; set; } = string.Empty;
        public string OgrenciSoyadi { get; set; } = string.Empty;
        public string OkulNumarasi { get; set; } = string.Empty;
        public string SinifAdi { get; set; } = string.Empty;
        public DateTime SilinmeTarihi { get; set; } = DateTime.Now;
        public string SilenKullanici { get; set; } = string.Empty;
    }
} 