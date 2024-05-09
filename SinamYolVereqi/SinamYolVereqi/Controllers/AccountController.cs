using SinamYolVereqi.Models.Auth;
using SinamYolVereqi.Services;
using SinamYolVereqi.ViewModel.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SinamYolVereqi.Models.Auth;
using SinamYolVereqi.ViewModel.Auth;
using System.Net.Mail;
using SinamYolVereqi.DAL;
using SinamYolVereqi.Models;
using Microsoft.AspNetCore.Hosting;
using System.Numerics;
using SinamYolVereqi.ViewModel;
using Microsoft.EntityFrameworkCore;
using SinamYolVereqi.Utilities.Extensions;
using System.Net;

namespace SinamYolVereqi.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService; // E-posta servisi ekleyin
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IConfiguration _configuration;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IEmailService emailService, IServiceProvider serviceProvider, ITempDataProvider tempDataProvider, IRazorViewEngine razorViewEngine, AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _serviceProvider = serviceProvider;
            _tempDataProvider = tempDataProvider;
            _razorViewEngine = razorViewEngine;
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            // VOEN girilmediyse veya boşsa kullanıcıyı normal şekilde kaydet
            if (string.IsNullOrEmpty(registerVM.VOEN))
            {
                var newUser = new AppUser
                {
                    Fullname = registerVM.Fullname,
                    UserName = registerVM.Username,
                    Email = registerVM.Email,
                            VOEN = registerVM.VOEN
                };

                var result = await _userManager.CreateAsync(newUser, registerVM.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(registerVM);
                }

                await _userManager.AddToRoleAsync(newUser, UserRoles.User.ToString());
                return RedirectToAction(nameof(Login));
            }

            // VOEN girilmişse ve eşleşiyorsa kullanıcıyı kaydet
            var voenSahibkar = registerVM.VOEN;
            var voens = _appDbContext.Voens.Select(v => v.VoenSahibkar).ToList();

            if (!voens.Contains(voenSahibkar))
            {
                ModelState.AddModelError("", "Girdiğiniz VOEN geçersiz.");
                return View(registerVM);
            }

            var userWithVoen = new AppUser
            {
                Fullname = registerVM.Fullname,
                UserName = registerVM.Username,
                Email = registerVM.Email,
                VOEN=registerVM.VOEN
            };

            var registerResult = await _userManager.CreateAsync(userWithVoen, registerVM.Password);
            if (!registerResult.Succeeded)
            {
                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(userWithVoen, UserRoles.User.ToString());
            return RedirectToAction(nameof(Login));
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM login, string? ReturnUrl)
        {
            if (!ModelState.IsValid) return View(login);
            AppUser user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email or Password is wrong!");
                return View(login);
            }
            Microsoft.AspNetCore.Identity.SignInResult signinResult =
                await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);

            if (!signinResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is wrong!");
                return View(login);
            }
            await _signInManager.SignInAsync(user, login.RememberMe);
            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        //public async Task<IActionResult> AddRoles()
        //{
        //    foreach (var role in Enum.GetValues(typeof(UserRoles)))
        //    {
        //        if (!await _roleManager.RoleExistsAsync(role.ToString()))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
        //        }
        //    }
        //    return Json("Ok");
        //}
        public IActionResult AccessDenied()
        {
            return View();
        }
        public enum UserRoles
        {
            Admin,
            User,
            Moderator
        }
        public async Task<IActionResult> Businessman()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Mevcut kullanıcıyı alın
                var currentUser = await _userManager.GetUserAsync(User);

                // VOEN değerini kontrol et
                if (!string.IsNullOrEmpty(currentUser.VOEN))
                {
                    // Kullanıcının arabalarını veritabanından alın
                    var userCars = _appDbContext.Cars.Where(c => c.AppUserName == currentUser.Id).ToList();

                    // ViewModel'e kullanıcı bilgilerini ve arabalarını aktarın
                    var viewModel = new BusinessmanViewModel
                    {
                        UserName = currentUser.UserName,
                        PhoneNumber = currentUser.PhoneNumber,
                        Email = currentUser.Email,
                        UserCars = userCars
                    };

                    return View(viewModel);
                }
            }

            // VOEN yoksa, veya kullanıcı giriş yapmamışsa, User sayfasına yönlendir
            return RedirectToAction("User1");
        }


        public async Task<IActionResult> User1()
        {
            // Kullanıcının oturum açtığı kullanıcıyı alın
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Kullanıcı bulunamazsa, hata mesajı döndürün
                ViewBag.ErrorMessage = "Kullanıcı bulunamadı.";
                return View();
            }

            // Kullanıcının e-posta adresini alın
            string currentUserEmail = user.Email;

            // E-posta adresiyle ilişkilendirilen tüm YolSenedi verilerini alın
            var yolSenediler = _appDbContext.YolSenedis.Where(y => y.TehvilAlaninEmaili == currentUserEmail&&y.IsActive==true).ToList();

            // Eğer YolSenedi verisi bulunamadıysa, hata mesajı döndürün
            if (yolSenediler.Count == 0)
            {
                ViewBag.ErrorMessage = "Girdiğiniz e-posta adresine ait YolSenedi verisi bulunamadı.";
                return View();
            }

            // YolSenedi verilerini Razor görünümüne aktarın
            return View(yolSenediler);
        }


        [HttpPost]
        public async Task<IActionResult> EmailBusinessMan(int id)
        {
            // Arabanın veritabanından alınması
            var car = _appDbContext.Cars
                .Include(c => c.AppUser) // AppUser'ı yükleyin
                .FirstOrDefault(c => c.Id == id);

            if (car == null)
            {
                return NotFound(); // Araba bulunamadıysa 404 hatası döndür
            }

            // Arabaya atanmış bir kullanıcı varsa
            if (car.AppUserName != null)
            {
                // Arabanın sahibinin e-posta adresinin alınması
                var owner = await _userManager.FindByIdAsync(car.AppUserName);

                if (owner != null)
                {
                    var ownerEmail = owner.Email;

                    // E-posta gönderme işlemi
                    await SendEmailAsync(ownerEmail, "Yol Vereqesi Alındı", "Salam, size Masinlarinizdan birine Yol vereqesi almaq isteyyi geldi,Hesabiniza daxil olun!");

                    // Araba'nın IsActive özelliğini true olarak ayarla
                    car.IsActive = true;
                    _appDbContext.SaveChanges(); // Değişiklikleri veritabanına kaydet

                    // İşlem başarıyla tamamlandığında, bir geri dönüş yapabilirsiniz
                    return RedirectToAction("Index", "Home"); // İşlem tamamlandıktan sonra ana sayfaya yönlendirme
                }
                else
                {
                    // Kullanıcı bulunamadıysa, uygun bir işlem yapılabilir
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                // Arabaya atanmış bir kullanıcı yoksa, işlem yapılamaz
                // Kullanıcıya bir hata mesajı gösterilebilir veya farklı bir işlem yapılabilir
                return RedirectToAction("Error", "Home");
            }
        }

        private async Task SendEmailAsync(string recipientEmail, string subject, string body)
{
    var emailConfig = _configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();

    using (var client = new SmtpClient())
    {
        client.Host = emailConfig.SmtpServer;
        client.Port = emailConfig.SmtpPort;
        client.EnableSsl = emailConfig.EnableSsl;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(emailConfig.SmtpUsername, emailConfig.SmtpPassword);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailConfig.FromEmail, emailConfig.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(recipientEmail);

        await client.SendMailAsync(mailMessage);
    }
}
        [HttpGet]
        public async Task<IActionResult> CreateCar()
        {
            // Mevcut kullanıcıyı alın
            var currentUser = await _userManager.GetUserAsync(User);

            // Kullanıcının adını ViewModel'e aktarın
            var viewModel = new CarVM
            {
                SahibkariName = currentUser.UserName
            };

            // Marka seçeneklerini alın ve View'e aktarın
            var markaItems = _appDbContext.Marka
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.MarkaName
                })
                .ToList();
            ViewBag.Marka = markaItems;

            // Model seçeneklerini alın ve View'e aktarın
            var modelItems = _appDbContext.CarModels
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.ModelName
                })
                .ToList();
            ViewBag.Models = modelItems;

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult GetModels(int markaId)
        {
            var models = _appDbContext.CarModels
                .Where(m => m.MarkaId == markaId)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.ModelName
                })
                .ToList();

            return Json(models);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCar(CarVM member)
        {
            if (!ModelState.IsValid)
            {
                // Fotoğrafın boyutunu ve tipini kontrol edin
                if (!member.Photo.CheckContentType("image/"))
                {
                    ModelState.AddModelError("Photo", $"{member.Photo.FileName} şekil tipinde olmalıdır");
                    ViewBag.Marka = await _appDbContext.Marka
                        .Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.MarkaName
                        })
                        .ToListAsync();
                    ViewBag.Models = await _appDbContext.CarModels
                          .Select(p => new SelectListItem
                          {
                              Value = p.Id.ToString(),
                              Text = p.ModelName,
                          })
                          .ToListAsync();
                    return View(member);
                }

                if (!member.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", $"{member.Photo.FileName} - 200kb'dan fazla olamaz");
                    ViewBag.Marka = await _appDbContext.Marka
                        .Select(p => new SelectListItem
                        {
                            Value = p.Id.ToString(),
                            Text = p.MarkaName
                        })
                        .ToListAsync();
                    ViewBag.Models = await _appDbContext.CarModels
                       .Select(p => new SelectListItem
                       {
                           Value = p.Id.ToString(),
                           Text = p.ModelName,
                       })
                       .ToListAsync();
                    return View(member);
                }

                // Fotoğrafı kaydedin
                string root = Path.Combine(_webHostEnvironment.WebRootPath, "RootAllPictures", "img");
                string fileName = await member.Photo.SaveAsync(root);

                // Mevcut kullanıcıyı alın
                var currentUser = await _userManager.GetUserAsync(User);

                // Arabayı oluşturun ve veritabanına ekleyin
                Car car = new Car
                {
                    Kuza = member.Kuza,
                    ImagePath = fileName,
                    QeydiyatNishani = member.QeydiyatNishani,
                    SeriyeVeNomresi = member.SeriyeVeNomresi,
                    Rengi = member.Rengi,
                    buraxilishili = member.buraxilishili,
                    MarkaId = member.MarkaId,
                    AppUserName = currentUser.Id,
                    SahibkarName = currentUser.UserName
                };

                await _appDbContext.Cars.AddAsync(car);
                await _appDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(CreateCar)); // Başka bir yere yönlendirme yapabilirsiniz
            }

            // ModelState.IsValid koşulu sağlanmazsa, hataları işleyin ve view'e geri dönün
            ViewBag.Marka = await _appDbContext.Marka
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.MarkaName
                })
                .ToListAsync();

            ViewBag.Models = await _appDbContext.CarModels
                         .Select(p => new SelectListItem
                         {
                             Value = p.Id.ToString(),
                             Text = p.ModelName,
                         })
                         .ToListAsync();
            return View(member);
        }



        [HttpGet]
        public IActionResult SahibkarHaqqinda(int id)
        {
            // Veritabanından ilgili Araba'nın Sahibkarını al
            var car = _appDbContext.Cars.FirstOrDefault(c => c.Id == id);
            if (car == null)
            {
                return NotFound(); // Araba bulunamazsa 404 hatası döndür
            }

            // Arabanın Sahibkarının ID'sini kullanarak ilgili AppUser'ı bul
            var appUser = _appDbContext.Users.FirstOrDefault(u => u.Id == car.AppUserName);
            if (appUser == null)
            {
                return NotFound(); // AppUser bulunamazsa 404 hatası döndür
            }

            // AppUser'a ait Sahibkar bilgilerini riçeren bir model oluştur
            var sahibkarViewModel = new CarVM
            {
                SahibkariName= appUser.UserName,
               Email=appUser.Email
                // Diğer özellikleri buraya ekle
            };

            // Sahibkar bilgilerini içeren view'e modeli gönder
            return View(sahibkarViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> LoginNotification(string email)
        {
            // E-posta içeriğini oluşturun
            string emailContent = GetLoginNotificationEmailContent(email);

            // E-posta gönderme işlemi
            await _emailService.SendEmailAsync(email, "Giriş Bildirimi", emailContent);

            // Kullanıcıya bir mesaj gösterme işlemi
            ViewBag.PasswordReminderSent = true;
            ViewBag.EmailForLogin = email; // E-posta adresini ViewBag'e ekleyin

            return View("ForgetPassword"); // ya da başka bir view'a yönlendirme yapabilirsiniz
        }

        private string GetLoginNotificationEmailContent(string email)
        {
            // Bu metot, e-posta içeriğini HTML formatında oluşturur
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Login Notification</title>
            </head>
            <body>
                <h2>Hospital Online Randevu Sitesine Giriş Bildirimi</h2>
                <p>Sayın kullanıcı, Hospital Online Randevu Sitesi'ne giriş yapıldı. Bu siz misiniz?</p>
                
                <form action=""URL_GIRIS_YAPMAK_ICIN"" method=""post"">
                    <input type=""hidden"" name=""email"" value=""{email}"" />
                    <button type=""submit"">Evet</button>
                </form>
                
                <form action=""URL_HAYIR_TIKLANDIGINDA_YONLENDIRILECEK"" method=""post"">
                    <input type=""hidden"" name=""email"" value=""{email}"" />
                    <button type=""submit"">Hayır</button>
                </form>
            </body>
            </html>";
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactVM model, [FromServices] IEmailService emailService)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // ModelState.IsValid false dönerse, hata mesajları otomatik olarak gösterilecek.
                    return View(model);
                }

                // Rastgele bir 6 haneli doğrulama kodu oluşturun
                var verificationCode = model.Message;

                // E-posta içeriğini oluşturma
                var emailContent = GetVerificationCodeEmailContentContact(model, "7p43xz0@code.edu.az", verificationCode);

                // E-posta gönderme işlemi
                var subject = "Musteri Reyleri";
                await emailService.SendEmailAsync("7p43xz0@code.edu.az", subject, emailContent);

                // E-posta gönderildikten sonra başarılı bir şekilde yönlendirme yapabilirsiniz.
                return RedirectToAction(nameof(Contact));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"E-posta gönderirken bir hata oluştu: {ex.Message}");

                // Hata durumunda ViewBag kullanarak hata mesajını gönder
                ViewBag.EmailSendError = "E-posta göndermek  mumkun olmadi,zehmet Olmasa Tekrar Ceht Edin.";

                // Hata durumunda View'a model ile geri dön
                return View(model);
            }
        }


        public IActionResult Services()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // Rastgele bir 6 haneli doğrulama kodu oluşturun
                    var verificationCode = new Random().Next(100000, 999999).ToString();

                    // E-posta içeriğini oluşturma
                    var emailContent = GetVerificationCodeEmailContent(model.Email, verificationCode);

                    try
                    {
                        // E-posta gönderme işlemi
                        var subject = "Şifre Hatırlatma - Doğrulama Kodu";
                        await _emailService.SendEmailAsync(model.Email, subject, emailContent);

                        // Doğrulama kodunu ve kullanıcı bilgisini TempData üzerinden başka sayfaya taşı
                        TempData["VerificationCode"] = verificationCode;
                        TempData["UserId"] = user.Id;

                        // Şifre sıfırlama başarılı, doğrulama kodu giriş sayfasına yönlendir
                        return RedirectToAction("VerifyCode");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.EmailSendError = "E-posta gönderme işleminde bir hata oluştu. Lütfen tekrar deneyin.";
                        Console.WriteLine($"E-posta gönderirken bir hata oluştu: {ex.Message}");
                        // Loglama veya başka bir işlem yapabilirsiniz.
                        return View();
                    }
                }
                else
                {
                    // E-posta adresiyle ilişkilendirilmiş bir kullanıcı bulunamazsa, hata mesajı göster.
                    ViewBag.PasswordReminderSent = false;
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.EmailSendError = "E-posta göndermek mümkün olmadı, lütfen tekrar deneyin.";
                Console.WriteLine($"E-posta gönderirken bir hata oluştu: {ex.Message}");
                // Loglama veya başka bir işlem yapabilirsiniz.
                throw;
            }
        }

        ///Dogrulama Kodu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyCode(string verificationCode)
        {
            try
            {
                // Doğrulama kodunu kontrol et
                if (verificationCode == TempData.Peek("VerificationCode")?.ToString())
                {
                    // Doğrulama başarılı, şifre sıfırlama sayfasına yönlendir
                    return RedirectToAction("ResetPassword");
                }
                else
                {
                    // Doğrulama başarısız, hata sayacını artır
                    int verificationAttempts = TempData.ContainsKey("VerificationAttempts") ? (int)TempData["VerificationAttempts"] : 0;
                    verificationAttempts++;

                    if (verificationAttempts >= 3)
                    {
                        // Eğer belirli bir sayıda başarısız deneme olduysa, tekrar e-posta gönder ve sayacı sıfırla
                        TempData["VerificationCode"] = null;
                        TempData["VerificationAttempts"] = null;
                        ViewBag.MaxVerificationAttemptsReached = true; // View'da bir bilgi mesajı göstermek için
                        return RedirectToAction("ForgetPassword");
                    }
                    else
                    {
                        // Hata mesajını göster ve sayacı TempData'de sakla
                        ViewBag.VerificationFailed = true;
                        TempData["VerificationAttempts"] = verificationAttempts;
                        return View("VerifyCode");
                    }
                }
            }
            catch (Exception ex)
            {
                // Genel bir hata durumunda buraya düşer
                ViewBag.EmailSendError = "E-posta göndermek mümkün olmadı, lütfen tekrar deneyin.";
                Console.WriteLine($"Bir hata oluştu: {ex.Message}");
                return View();
            }
        }


        ///ForgetPaasword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string newPassword, string confirmPassword)
        {
            try
            {
                // Kullanıcıyı TempData'den al
                var userId = TempData.Peek("UserId")?.ToString();
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null)
                {
                    // Yeni parolanın ve tekrarının eşleşip eşleşmediğini kontrol et
                    if (newPassword == confirmPassword)
                    {
                        // Kullanıcıya bir şifre sıfırlama token'ı oluştur
                        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                        // Yeni parolayı kullanarak şifreyi sıfırla
                        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

                        if (result.Succeeded)
                        {
                            // Parola sıfırlama başarılı, kullanıcıyı bilgilendir
                            ViewBag.ChangePasswordSuccess = true;

                            // İsterseniz kullanıcıya e-posta ile de bilgi verebilirsiniz
                            string newPasswordEmailContent = GetNewPasswordEmailContent(newPassword);
                            await _emailService.SendEmailAsync(user.Email, "Parola Değişikliği", newPasswordEmailContent);

                            return RedirectToAction(nameof(Login));
                        }
                    }
                }
                else
                {
                    // Kullanıcı bulunamadı, hata mesajı göster
                    ViewBag.ChangePasswordFailed = true;
                    return View("ResetPassword");
                }

                // Parola sıfırlama başarısız, hata mesajı göster
                ViewBag.ChangePasswordFailed = true;
                return View("ResetPassword");
            }
            catch (Exception ex)
            {
                ViewBag.EmailSendError = "E-posta göndermek  mumkun olmadi,zehmet Olmasa Tekrar Ceht Edin.";
                Console.WriteLine($"Parola sıfırlama işlemi sırasında bir hata oluştu: {ex.Message}");
                // Hata durumunda da bir şey döndür
                ViewBag.ChangePasswordFailed = true;
                return View("ResetPassword");
            }
        }


        public IActionResult VerifyCode()
        {
            return View();
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        public IActionResult EmailLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EmailLogin(string email)
        {
            // E-posta ile kullanıcıyı bul
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                // E-posta ile doğrulama işlemi yap
                var result = await _userManager.ConfirmEmailAsync(user, await _userManager.GenerateEmailConfirmationTokenAsync(user));

                if (result.Succeeded)
                {
                    // Giriş işlemi
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Kullanıcıyı ana sayfaya yönlendir
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // E-posta doğrulama başarısızsa, hata mesajı ile geri dön
                    ModelState.AddModelError(string.Empty, "E-posta doğrulama başarısız.");
                }
            }
            else
            {
                // E-posta adresi veritabanında bulunamazsa, hata mesajı ile geri dön
                ModelState.AddModelError(string.Empty, "Bu e-posta adresiyle kayıtlı bir kullanıcı bulunamadı.");
            }

            // Hata durumunda tekrar formu göster
            return View("EmailLogin");
        }





        /// Email i nece gondermek ucun html 
        private string GetNewPasswordEmailContent(string newPassword)
        {
            // Bu metot, e-posta içeriğini HTML formatında oluşturur
            return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>Yeni Şifrə</title>
        </head>
        <body>
            <h2>Yeni Şifrəniz</h2>
            <p>Şifrəniz başarıyla dəyiçdirildi.  yeni şifrəniz:</p>
            <p><strong>{newPassword}</strong></p>
        </body>
        </html>";
        }


        private string GetVerificationCodeEmailContent(string email, string verificationCode)
        {
            // Bu metot, doğrulama kodunu içeren e-posta içeriğini HTML formatında oluşturur
            return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>Doğrulama Kodu</title>
        </head>
        <body>
            <h2> Doğrulama Kodu</h2>
            <p>Şifrenizi dəyiştirmək  üçün aşağıdaki doğrulama kodunu istifadə edin:</p>
            <p><strong>{verificationCode}</strong></p>
        </body>
        </html>";
        }

        private string GetVerificationCodeEmailContentContact(ContactVM model, string email, string verificationCode)
        {
            // Bu metot, doğrulama kodunu içeren e-posta içeriğini HTML formatında oluşturur
            return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>Kimden: {model.Name}</title>
        </head>
        <body>
            <h2> Kimden: {model.Name}</h2>
            <p>Movzu: {model.Subject}</p>
            <p><strong>{verificationCode}</strong></p>
        </body>
        </html>";
        }

    }
}

