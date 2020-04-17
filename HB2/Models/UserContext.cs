using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HB2.Models
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Plan> Plans { get; set; }
      
        public DbSet<P> Ps { get; set; }
        public DbSet<P1> P1s { get; set; }
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
           /* Database.EnsureDeleted(); */  // удаляем бд со старой схемой
            Database.EnsureCreated();   // создаем бд с новой схемой
        }
    }
}
