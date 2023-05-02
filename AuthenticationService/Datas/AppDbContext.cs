using AuthenticationService.Entitis;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Datas
{
    public class AppDbContext : IdentityDbContext<IdentityUser,IdentityRole, string>
    {
        public DbSet<ProfileModel> Profile { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
