namespace RestaurantApp.Models
{
    public class Jelo
    {
        public int Id { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public string Vrsta { get; set; } = string.Empty;  // Hrana / Piće
        public decimal Cijena { get; set; }
        public bool Dostupno { get; set; } = true;
        public string? SlikaUrl { get; set; }

        // Navigation property
        public ICollection<StavkaNarudzbe> Stavke { get; set; } = new List<StavkaNarudzbe>();
    }
}