using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SinamYolVereqi.DAL;
using SinamYolVereqi.Models;
using SinamYolVereqi.ViewModel;
using System.Diagnostics;

namespace SinamYolVereqi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _appDbContext;
        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
               

                Cars = _appDbContext.Cars.ToList(),
                CarsModel=_appDbContext.CarModels.ToList(),
                Marka=_appDbContext.Marka.ToList(),

                //Products = _secondaryDatabaseService?.GetProducts().ToList(), // DoctorPosition tablosunu çek



            };
            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult MasinYolVereqi(int id)
        {
            // Kullanıcı oturumu açık mı kontrol edin
            if (!User.Identity.IsAuthenticated)
            {
                // Kullanıcı oturumu açık değilse, giriş sayfasına yönlendirin
                return RedirectToAction("Login", "Account");
            }

            // Araba detaylarını veritabanından al
            var car = _appDbContext.Cars
                                    .Include(c => c.Marka)
                                    .FirstOrDefault(c => c.Id == id);

            if (car == null)
            {
                return NotFound(); // Araba bulunamadıysa 404 hatası döndür
            }

            return View(car);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult YolVereqesiYarat(int id)
        {
            // Arabanın veritabanından alınması
            var car = _appDbContext.Cars
                .Include(c => c.AppUser) // AppUser'ı yükleyin
                .Include(c=>c.Marka)
                .Include(c=>c.Marka.CarModels)
                .FirstOrDefault(c => c.Id == id);
            return View(car);
        }
        [HttpPost]
        public IActionResult YolVereqesiYarat(int id, YolVereqesiViewModel viewModel)
        {
            // Arabanın veritabanından alınması
            var car = _appDbContext.Cars
                .Include(c => c.Marka)
                    .ThenInclude(m => m.CarModels)
                .FirstOrDefault(c => c.Id == id);

            if (car == null)
            {
                return NotFound(); // Araba bulunamadıysa 404 hatası döndür
            }

            // Forma girilen verileri kullanarak işlem yapabilirsiniz
            // Örneğin, yol vereqesi oluşturma işlemi yapılabilir

            // ViewModel'i kullanarak gelen verilere erişebilirsiniz
            var marka = car.Marka.MarkaName;
            var model = car.Marka.CarModels.FirstOrDefault()?.ModelName;
            var nomre = car.QeydiyatNishani;
            var seriyaNomresi = car.SeriyeVeNomresi;
            var TehvilAlaninAdi = viewModel.TehvilAlaninAdi;
            var TehvilAlaninEmaili = viewModel.TehvilAlaninEmaili;
            var SahibkarinAdi = car.SahibkarName;
            var verilmeMuddeti = viewModel.VerilmeMuddeti;

            // Yol vereqesi oluşturma işlemi burada gerçekleştirilebilir

            // Yol senedi modelini oluşturun
            YolSenedi yolSenedi = new YolSenedi
            {
                Marka = marka,
                Model = model,
                Nomre = nomre,
                SeriyaNomresi = seriyaNomresi,
                TehvilAlaninAdi = TehvilAlaninAdi,
                TehvilAlaninEmaili = TehvilAlaninEmaili,
                SahibkarinAdi = SahibkarinAdi,
                VerilmeMuddeti = verilmeMuddeti
            };

            // Yol senedini veritabanına ekleyin
            _appDbContext.YolSenedis.Add(yolSenedi);
            _appDbContext.SaveChanges();

            // İşlem başarıyla tamamlandığında, yeni oluşturulan yol vereqesinin Id'sini alarak `SlaydiYukle` metoduna yönlendirin
            return RedirectToAction("SlaydiYukle", "Home", new { carId = yolSenedi.Id });
        }


        public class YolVereqesiViewModel
        {
            
            public string Marka { get; set; }
            public string Nomre { get; set; }
            public string SeriyaNomresi { get; set; }
            public string TehvilAlaninAdi { get; set; }
            public string TehvilAlaninEmaili { get; set; }
            public string TehvilAlmaAdi { get; set; }
         
            public string SahibkarinAdi { get; set; }
            public string VerilmeMuddeti { get; set; }
        }


        public IActionResult SlaydiYukle(int carId,int a)
        {
            // Veritabanından ilgili arabayı bul
            var car = _appDbContext.YolSenedis.FirstOrDefault(c => c.Id == carId);

            if (car == null)
            {
                return NotFound(); // Araba bulunamadıysa 404 hatası döndür
            }

            // YolVereqesiViewModel'i doldur
            var viewModel = new YolSenedi
            {Id=carId,
                SeriyaNomresi = car.SeriyaNomresi,
                Nomre = car.Nomre,
                SahibkarinAdi=car.SahibkarinAdi,
                TehvilAlaninAdi=car.TehvilAlaninAdi,
                Marka=car.Marka,
                VerilmeMuddeti=car.VerilmeMuddeti
                // Diğer alanları buraya ekleyin
            };

            return View(viewModel);
        }
        public IActionResult About()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SlaydiYukle(int carId)
        {
            // Veritabanındaki yol vereqesi kaydını bulun
            var yolVereqesi = _appDbContext.YolSenedis.FirstOrDefault(y => y.Id == carId);

            if (yolVereqesi == null)
            {
                return NotFound(); // Yol vereqesi bulunamadıysa 404 hatası döndür
            }

            // IsActive değerini true yapın
            yolVereqesi.IsActive = true;

            // Değişiklikleri veritabanına kaydedin
            _appDbContext.SaveChanges();

            // İşlem başarıyla tamamlandığında, aynı sayfaya yönlendirin
            return RedirectToAction("SlaydiYukle", "Home", new { carId  });
        }




        public IActionResult MasinVereqleri(string nomre)
        {
            // Veritabanından ilgili pilaka numarasına sahip arabanın yol vereqelerini alın
            var yolVereqeleri = _appDbContext.YolSenedis.Where(y => y.Nomre == nomre&&y.IsActive==true).ToList();

            // View'e yol vereqelerini gönderin
            return View(yolVereqeleri);
        }


    }
}
