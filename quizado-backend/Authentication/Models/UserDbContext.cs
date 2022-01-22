using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Models
{
    public class UserDbContext : DbContext
    {
        public UserDbContext() { }
        public UserDbContext(DbContextOptions options) : base(options)
        { this.Database.EnsureCreated(); }
        public DbSet<User> Users { get; set; }
    }
}
