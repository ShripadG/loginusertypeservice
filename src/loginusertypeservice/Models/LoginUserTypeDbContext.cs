using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace loginusertypeservice.Models
{
    public partial class LoginUserTypeDbContext : DbContext
    {
        public LoginUserTypeDbContext()
        {
        }

        public LoginUserTypeDbContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<LoginUserType> UserType { get; set; }
    }
}
