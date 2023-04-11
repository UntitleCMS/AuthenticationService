using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Datas
{
    public class AspNetIdentityDbContext : IdentityDbContext
    {
        public AspNetIdentityDbContext(DbContextOptions<AspNetIdentityDbContext> options)
            : base(options)
        {
            
        }
    }

}
