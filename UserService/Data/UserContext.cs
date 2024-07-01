using Microsoft.EntityFrameworkCore;
using UserService.Models.Database;

namespace UserService.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RegularUser> RegularUsers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring one-to-many relationship between Role and User
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId);

            // Configuring one-to-many relationship between Permission and Admin
            modelBuilder.Entity<Permission>()
                .HasMany(p => p.Admins)
                .WithOne(a => a.Permission)
                .HasForeignKey(a => a.PermissionId);

            // Configuring TPH (Table Per Hierarchy) for User, RegularUser, and Admin
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<RegularUser>("RegularUser")
                .HasValue<Admin>("Admin");

            base.OnModelCreating(modelBuilder);
        }
    }
}
