using Authentication__ASP.Net_Core__.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication__ASP.Net_Core__.Data
{
    public class ApplicationDBContext :DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
       
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Ensure email is unique
        }
    }
}
