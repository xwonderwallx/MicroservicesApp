using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RegularUser> RegularUsers { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring one-to-many relationship between Role and User
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId);

            // Configuring one-to-one relationship between User and RegularUser
            modelBuilder.Entity<User>()
                .HasOne(u => u.RegularUser)
                .WithOne(ru => ru.User)
                .HasForeignKey<RegularUser>(ru => ru.UserId);
            
            // Configuring one-to-one relationship between User and Admin
            modelBuilder.Entity<User>()
                .HasOne(u => u.Admin)
                .WithOne(a => a.User)
                .HasForeignKey<Admin>(a => a.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
