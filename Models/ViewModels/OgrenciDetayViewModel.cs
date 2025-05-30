using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models.ViewModels
{
    public class OgrenciDetayViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Öğrenci Adı")]
        public string OgrenciAdi { get; set; }

        [Display(Name = "Öğrenci Soyadı")]
        public string OgrenciSoyadi { get; set; }

        [Display(Name = "Okul Numarası")]
        public string OkulNumarasi { get; set; }

        [Display(Name = "Sınıf")]
        public string SinifAdi { get; set; }

        public List<OgrenciOduncKitapViewModel> OduncKitaplar { get; set; }
    }

    public class OgrenciOduncKitapViewModel
    {
        public string KitapAdi { get; set; }
        public string Yazar { get; set; }
        public DateTime AlinmaTarihi { get; set; }
        public DateTime? TeslimTarihi { get; set; }
        public bool TeslimDurumu { get; set; }
    }
} 