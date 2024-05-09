//using Hospital_Template.DAL;
//using Microsoft.EntityFrameworkCore;
//using Hospital_Template.Models;
//using System.Linq;

//namespace Hospital_Template.Services
//{
//    public class MyService
//    {
//        private readonly AppDbContext _context;
//        private readonly SecondaryDatabaseService _secondaryDatabaseService;

//        public MyService(AppDbContext context, SecondaryDatabaseService secondaryDatabaseService)
//        {
//            _context = context;
//            _secondaryDatabaseService = secondaryDatabaseService;
//        }

//        public void DoSomething()
//        {
           
//            // İkinci bağlantıdan veri çekmek için SecondaryDatabaseService'i kullanın
//            var productsFromSecondaryDatabase = _secondaryDatabaseService.GetProducts().ToList();
           

//            // Diğer işlemleri gerçekleştirin...
//        }
//    }
//}
