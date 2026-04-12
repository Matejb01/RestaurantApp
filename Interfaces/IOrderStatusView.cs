using RestaurantApp.Models;

namespace RestaurantApp.Interfaces
{
    public interface IOrderStatusView : IView
    {
        Narudzba? Narudzba { get; set; }
        int SearchId { get; set; }
    }
}