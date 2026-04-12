using Microsoft.EntityFrameworkCore;
using RestaurantApp.Models;

namespace RestaurantApp.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
            : base(options) { }

        public DbSet<Jelo> Jela { get; set; }
        public DbSet<Narudzba> Narudzbe { get; set; }
        public DbSet<StavkaNarudzbe> StavkeNarudzbe { get; set; }
        public DbSet<Zaposlenik> Zaposlenici { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed some menu items
            modelBuilder.Entity<Jelo>().HasData(
                new Jelo { Id = 1, Naziv = "Margherita Pizza", Opis = "Klasična pizza s rajčicom i mozzarellom", Vrsta = "Hrana", Cijena = 9.50m, Dostupno = true },
                new Jelo { Id = 2, Naziv = "Hamburger", Opis = "Goveđi burger s povrćem", Vrsta = "Hrana", Cijena = 8.00m, Dostupno = true },
                new Jelo { Id = 3, Naziv = "Coca Cola", Opis = "0.33l", Vrsta = "Piće", Cijena = 2.50m, Dostupno = true },
                new Jelo { Id = 4, Naziv = "Pasta Carbonara", Opis = "Kremasta pasta s pancetom", Vrsta = "Hrana", Cijena = 10.00m, Dostupno = true },
                new Jelo { Id = 5, Naziv = "Sok od naranče", Opis = "Svježe cijeđeni", Vrsta = "Piće", Cijena = 3.00m, Dostupno = true }
            );

            // Seed an admin and an employee account
            modelBuilder.Entity<Zaposlenik>().HasData(
                new Zaposlenik { Id = 1, Ime = "Admin", Prezime = "Admin", KorisnickoIme = "admin", Lozinka = "admin123", Uloga = "Administrator" },
                new Zaposlenik { Id = 2, Ime = "Pero", Prezime = "Perić", KorisnickoIme = "pero", Lozinka = "pero123", Uloga = "Zaposlenik" }
            );
        }
    }
}