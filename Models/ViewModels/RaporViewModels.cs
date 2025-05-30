using System;
using System.Collections.Generic;

namespace Kutuphane.Models.ViewModels
{
    public class OgrenciOkumaRaporuViewModel
    {
        public int OgrenciId { get; set; }
        public string OgrenciAdi { get; set; }
        public string OgrenciSoyadi { get; set; }
        public int OkumaSayisi { get; set; }
    }

    public class KitapOkunmaRaporuViewModel
    {
        public int KitapId { get; set; }
        public string KitapAdi { get; set; }
        public string Yazar { get; set; }
        public int OkunmaSayisi { get; set; }
    }

    public class OgrenciViewModel
    {
        public int Id { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string OkulNumarasi { get; set; }
    }

    public class KitapViewModel
    {
        public int Id { get; set; }
        public string KitapAdi { get; set; }
        public string Yazar { get; set; }
        public string Kategori { get; set; }
    }

    public class KitapTuruRaporuViewModel
    {
        public string Kategori { get; set; }
        public int OduncSayisi { get; set; }
    }
} 