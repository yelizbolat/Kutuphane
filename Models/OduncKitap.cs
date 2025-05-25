using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kutuphane.Models
{
   public class OduncKitap
    {
        [Key]
        public int Id { get; set; }
        public DateTime OduncAlmaTarihi { get; set; }
        public DateTime? TeslimTarihi { get; set; }
        public bool TeslimDurumu { get; set; }
        public int KitapId { get; set; } 
        public Kitap? Kitap { get; set; }
        public int? OgrenciId { get; set; }  // Değiştirildi
        public Ogrenci? Ogrenci { get; set; }  // Nullable
    }
}