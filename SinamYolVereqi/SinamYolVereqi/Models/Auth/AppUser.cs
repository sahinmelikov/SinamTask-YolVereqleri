using Microsoft.AspNetCore.Identity;

namespace SinamYolVereqi.Models.Auth
{
    public class AppUser : IdentityUser
    {
        public string Fullname { get; set; }
        public bool IsActivated { get; set; }
        public List<Car> Cars { get; set; }
   public string? VOEN { get; set; }
    }
}
