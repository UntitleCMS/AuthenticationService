using AuthenticationService.Entitis;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Datas
{
    public class TmpDataContext : IdentityDbContext<IdentityUser,IdentityRole, string>
    {
        public DbSet<ProfileModel> Profile { get; set; }

        public TmpDataContext(DbContextOptions<TmpDataContext> options) : base(options)
        {
        }
    }
}
