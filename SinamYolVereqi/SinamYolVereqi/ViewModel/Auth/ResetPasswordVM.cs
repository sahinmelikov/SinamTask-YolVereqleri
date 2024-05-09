using System.ComponentModel.DataAnnotations;

namespace SinamYolVereqi.ViewModel.Auth
{
    public class ResetPasswordVM
    {
        public string UserId { get; set; }
        public string Code { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}
