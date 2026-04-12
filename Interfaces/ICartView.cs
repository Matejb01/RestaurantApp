using RestaurantApp.Models;

namespace RestaurantApp.Interfaces
{
    public interface ICartView : IView
    {
        List<CartItem> CartItems { get; set; }
        decimal UkupnaCijena { get; set; }
        string Ime { get; set; }
        string Adresa { get; set; }
        string Telefon { get; set; }
        string Email { get; set; }
        string NacinPreuzimanja { get; set; }
    }
}