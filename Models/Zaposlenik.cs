namespace RestaurantApp.Models
{
    public class Zaposlenik
    {
        public int Id { get; set; }
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string KorisnickoIme { get; set; } = string.Empty;
        public string Lozinka { get; set; } = string.Empty; // hashed later
        public string Uloga { get; set; } = "Zaposlenik"; // Zaposlenik / Administrator

        // Navigation property
        public ICollection<Narudzba> Narudzbe { get; set; } = new List<Narudzba>();
    }
}