using Microsoft.AspNetCore.Identity;

namespace SinamYolVereqi.Models.Auth
{
    public class AppRole : IdentityRole
    {
        public bool IsActivated { get; set; }
    }
}
