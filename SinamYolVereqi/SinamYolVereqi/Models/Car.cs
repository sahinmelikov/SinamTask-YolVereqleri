using SinamYolVereqi.Models.Auth;

namespace SinamYolVereqi.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Kuza {  get; set; }
        public string QeydiyatNishani { get; set; }
      
        public string SeriyeVeNomresi { get;set; }
        public string Rengi { get; set; }   
        public string ImagePath { get;set; }
        public string buraxilishili { get;set; }   
        public Marka Marka { get; set; }
        public int MarkaId { get;set; }
        public AppUser AppUser { get; set; }
      public string? SahibkarName { get; set; }
        public string? AppUserName { get; set; }
        public bool IsActive { get; set; }

    }
}
