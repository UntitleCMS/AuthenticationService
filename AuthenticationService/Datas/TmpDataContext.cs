using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Datas
{
    public class TmpDataContext : DbContext
    {
        public DbSet<ProfileModel> Profile { get; set; }

        public TmpDataContext(DbContextOptions<TmpDataContext> options) : base(options)
        {
        }
    }
}
