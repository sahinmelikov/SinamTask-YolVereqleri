using Microsoft.AspNetCore.Mvc.Rendering;
using SinamYolVereqi.Models;

namespace SinamYolVereqi.ViewModel
{
    public class CarVM
    {
        public int Id { get; set; }
        public string Kuza { get; set; }
        public string QeydiyatNishani { get; set; }
        public string SeriyeVeNomresi { get; set; }
        public string Rengi { get; set; }
        public string buraxilishili { get; set; }
        public IFormFile Photo { get; set; }
public string SahibkariName { get; set; }
        public string Email { get; set; }
    public int ModelId { get; set; }
        public int MarkaId { get; set; }
        public List<SelectListItem> MarkaOptions { get; set; } // Marka seçenekleri
        public List<SelectListItem> ModelOptions { get; set; } // Model seçenekleri
        public int SelectedModelId { get; set; } // Seçili model ID'si
    }
}
