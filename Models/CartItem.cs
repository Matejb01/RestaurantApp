namespace RestaurantApp.Models
{
    public class CartItem
    {
        public int JeloId { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public decimal Cijena { get; set; }
        public int Kolicina { get; set; }
        public decimal Ukupno => Cijena * Kolicina;
    }
}