using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_cSharp.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_cSharp.Data
{
    public class EFDataContext : DbContext
    {
        private readonly IConfiguration _config;

        public EFDataContext(IConfiguration config)
        {
            _config = config;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSalary> UserSalary { get; set; }
        public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    _config.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure()
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("CSharpDotNetFirstProject");
            modelBuilder
                .Entity<User>()
                .ToTable("Users", "CSharpDotNetFirstProject")
                .HasKey(u => u.UserId);
            modelBuilder.Entity<UserSalary>().HasKey(u => u.UserId);
            modelBuilder.Entity<UserJobInfo>().HasKey(u => u.UserId);
        }
    }
}
