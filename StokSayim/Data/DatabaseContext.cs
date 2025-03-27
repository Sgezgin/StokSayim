using StokSayim.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace StokSayim.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            // Veritabanı oluşturma stratejisi belirleyin (isteğe bağlı)
            // Database.SetInitializer<DatabaseContext>(new CreateDatabaseIfNotExists<DatabaseContext>());
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Conventionları kaldır (isteğe bağlı)
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Barkod ve BrandID için indeks tanımlama
            modelBuilder.Entity<CatalogItem>()
                .Property(e => e.Barcode)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<CatalogItem>()
                .Property(e => e.BrandID)
                .IsRequired();
        }
    }
}
