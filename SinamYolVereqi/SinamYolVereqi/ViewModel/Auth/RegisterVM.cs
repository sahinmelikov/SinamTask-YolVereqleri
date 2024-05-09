using System.ComponentModel.DataAnnotations;

namespace SinamYolVereqi.ViewModel.Auth
{
	public class RegisterVM
	{
		[Required, MaxLength(100)]
		public string Fullname { get; set; }
		[Required, MaxLength(100)]
		public string Username { get; set; }
		[Required, MaxLength(255), DataType(DataType.EmailAddress)]
		public string Email { get; set; }
		[Required, MinLength(8), DataType(DataType.Password)]
		public string Password { get; set; }
		[Compare(nameof(Password)), DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }
        public string? VOEN { get; set; }
    }
}
