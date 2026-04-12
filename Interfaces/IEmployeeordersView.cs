using RestaurantApp.Models;

namespace RestaurantApp.Interfaces
{
    public interface IEmployeeOrdersView : IView
    {
        List<Narudzba> Narudzbe { get; set; }
        string FilterStatus { get; set; }
    }
}