using System.ComponentModel.DataAnnotations;

namespace SinamYolVereqi.ViewModel.Auth
{
	public class LoginVM
	{

		[Required, DataType(DataType.EmailAddress)]
		public string Email { get; set; }
		[Required, MinLength(8), DataType(DataType.Password)]
		public string Password { get; set; }
		public bool RememberMe { get; set; }
	}
}
