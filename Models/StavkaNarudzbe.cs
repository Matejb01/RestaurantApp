namespace RestaurantApp.Models
{
    public class StavkaNarudzbe
    {
        public int Id { get; set; }
        public int Kolicina { get; set; }
        public decimal CijenaStavke { get; set; }

        // Foreign keys
        public int NarudzbaId { get; set; }
        public Narudzba Narudzba { get; set; } = null!;

        public int JeloId { get; set; }
        public Jelo Jelo { get; set; } = null!;
    }
}