namespace RestaurantApp.Models
{
    public class Narudzba
    {
        public int Id { get; set; }
        public DateTime DatumVrijeme { get; set; } = DateTime.Now;
        public string Ime { get; set; } = string.Empty;
        public string Adresa { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string NacinPreuzimanja { get; set; } = string.Empty; // Dostava / Preuzimanje
        public string Status { get; set; } = "Zaprimljeno";
        // Statusi: Zaprimljeno, U pripremi, U dostavi, Dostavljeno, Odbijeno
        public decimal UkupnaCijena { get; set; }
        public string? RazlogOdbijanja { get; set; }
        public int? PredvijenoVrijemeDostave { get; set; } // in minutes
        public string? Email { get; set; }

        // Foreign key
        public int? ZaposlenikId { get; set; }
        public Zaposlenik? Zaposlenik { get; set; }

        // Navigation property
        public ICollection<StavkaNarudzbe> Stavke { get; set; } = new List<StavkaNarudzbe>();
    }
}