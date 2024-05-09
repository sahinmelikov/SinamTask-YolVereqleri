namespace SinamYolVereqi.Models
{
    public class Marka
    {
        public int Id { get;set; }
        public string MarkaName { get;set; }
        public List<Car> Car { get;set; }
       
        public List<CarModel> CarModels { get; set; }
    }
}
