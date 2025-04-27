using StokSayim.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
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
            // Performans optimizasyonları
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;

            // Hata ayıklama için SQL sorgularını loglama
            this.Database.Log = s => Debug.WriteLine(s);

            Database.SetInitializer<DatabaseContext>(null);

        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<Personel> Personel { get; set; }
        public DbSet<SayimDetay> SayimDetay { get; set; }
        public DbSet<Sayim> Sayim { get; set; }
        public DbSet<IlTanim> IlTanim { get; set; }
        public DbSet<AlanTipTanim> AlanTipTanim { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Conventionları kaldır (isteğe bağlı)
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<CatalogItem>()
                  .ToTable("CatalogItems"); // Tam tablo adını belirt

            modelBuilder.Entity<Brand>()
                .ToTable("Brands");

            modelBuilder.Entity<Store>()
                .ToTable("Stores");

            // Barkod ve BrandID için gereklilikleri belirt (veritabanını değiştirmez)
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
