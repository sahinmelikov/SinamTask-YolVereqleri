using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SinamYolVereqi.Models;
using SinamYolVereqi.Models.Auth;


namespace SinamYolVereqi.DAL
{
	public class AppDbContext : IdentityDbContext<AppUser>
    {


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Marka> Marka { get; set; }
        public DbSet<CarModel>CarModels { get; set; }
        public DbSet<Voen>  Voens { get; set; }
		public DbSet<YolSenedi>YolSenedis { get; set; }
		
	}
}
