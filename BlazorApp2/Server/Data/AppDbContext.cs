using Microsoft.EntityFrameworkCore;

using BlazorApp2.Shared.Models;

namespace BlazorApp2.Server.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

        public DbSet<AssetUsage> AssetUsages { get; set; }
        public DbSet<AssetMaster> AssetMasters { get; set; }
		

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<AssetUsage>()
				.HasOne(au => au.AssetMaster)
				.WithMany(am => am.AssetUsages)
				.HasForeignKey(au => au.AssetCode)
				.HasPrincipalKey(am => am.AssetCode);

            modelBuilder.Entity<AssetMaster>()
				.Property(p => p.Fee)
				.HasPrecision(18, 2);
        }
    }
}

