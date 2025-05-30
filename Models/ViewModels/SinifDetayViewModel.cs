using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models.ViewModels
{
    public class SinifDetayViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Sınıf Adı")]
        public string SinifAdi { get; set; }

        [Display(Name = "Öğrenci Sayısı")]
        public int OgrenciSayisi { get; set; }

        public List<SinifOgrenciViewModel> Ogrenciler { get; set; }
        public List<OduncKitapViewModel> OduncKitaplar { get; set; }
    }

    public class SinifOgrenciViewModel
    {
        public int Id { get; set; }
        public string OgrenciAdi { get; set; }
        public string OgrenciSoyadi { get; set; }
        public string OkulNumarasi { get; set; }
    }

    public class OduncKitapViewModel
    {
        public string OgrenciAdi { get; set; }
        public string KitapAdi { get; set; }
        public DateTime AlinmaTarihi { get; set; }
        public DateTime? TeslimTarihi { get; set; }
        public bool TeslimDurumu { get; set; }
    }
} 