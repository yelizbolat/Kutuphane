using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models
{
    public class Ogrenci
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Öğrenci adı zorunludur.")]
        public string OgrenciAdi { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Öğrenci soyadı zorunludur.")]
        public string OgrenciSoyadi { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Okul numarası zorunludur.")]
        public string OkulNumarasi { get; set; }
        
        public int? SinifId { get; set; } 
        public Sinif? Sinif { get; set; }
        
        // Kitap ödünç alma tarihi
        public DateTime? KitapOduncTarihi { get; set; }  
        
        // Kitap geri getirildi mi?
        public bool KitapGeriGetirildi { get; set; }  
        
        // Kitap adı
        [Required(ErrorMessage = "Kitap adı zorunludur.")]
        public string KitapAdi { get; set; } = string.Empty;

        // Kitap teslim tarihi (isteğe bağlı)
        public DateTime? TeslimTarihi { get; set; }
        
        // Eklenme tarihi (sisteme eklenme tarihi)
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
    }
}