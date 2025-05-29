using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models.ViewModels
{
    public class SinifViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Sınıf Adı")]
        public string SinifAdi { get; set; }

        [Display(Name = "Öğrenci Sayısı")]
        public int OgrenciSayisi { get; set; }
    }
} 