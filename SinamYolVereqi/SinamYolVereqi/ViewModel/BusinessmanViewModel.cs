using SinamYolVereqi.Models;

namespace SinamYolVereqi.ViewModel
{
    public class BusinessmanViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<Car> UserCars { get; set; }
    }

}
