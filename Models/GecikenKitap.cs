using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models
{
    public class GecikenKitap
    {
        public int Id { get; set; }
        public string OgrenciAdi { get; set; }
        public string OgrenciSoyadi { get; set; }
        public string KitapAdi { get; set; }
        public DateTime AlinmaTarihi { get; set; }
        public DateTime TeslimTarihi { get; set; } 
        public DateTime? GercekTeslimTarihi { get; set; } 
        public OduncKitap OduncKitap {get;set;}
    public int? GecikmeSuresi
    {
        get
        {
            DateTime teslimEdilenTarih = GercekTeslimTarihi ?? DateTime.Now;
            if (teslimEdilenTarih > TeslimTarihi)
            {
                return (teslimEdilenTarih - TeslimTarihi).Days;
            }
                return null; // Gecikme yok
        }
    }
     // Teslim durumu (string)
    public string TeslimDurumu
    {
        get
        {
            return GercekTeslimTarihi.HasValue ? "Teslim Alındı" : "Teslim Edilmedi";
        }
    }

    // Kitap hâlâ teslim edilmemişse true döner
    public bool KitapTeslimEdilmediMi => !GercekTeslimTarihi.HasValue && DateTime.Now > TeslimTarihi;
    }
} 