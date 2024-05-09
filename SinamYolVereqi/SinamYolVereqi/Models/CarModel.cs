namespace SinamYolVereqi.Models
{
    public class CarModel
    {
        public int Id { get;set; }
        public string ModelName { get; set; }
        public Marka Marka { get; set; }
        public int MarkaId { get; set; }

    }
}
