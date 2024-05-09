namespace SinamYolVereqi.Models
{
    public class YolSenedi
    {
        public int Id { get; set; }
        public string? Marka { get; set; }
        public string? Model { get; set; }
        public string? Nomre { get; set; }
        public string? SeriyaNomresi { get; set; }
        public string? TehvilAlaninAdi { get; set; }
        public string? TehvilAlaninEmaili { get; set; }
        public string? SahibkarinAdi { get; set; }
        public string? VerilmeMuddeti { get; set; }
        public bool? IsActive { get; set; }
    }
}
