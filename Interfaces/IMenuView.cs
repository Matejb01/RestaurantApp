using RestaurantApp.Models;

namespace RestaurantApp.Interfaces
{
    public interface IMenuView : IView
    {
        List<Jelo> Jela { get; set; }
        string FilterVrsta { get; set; }
    }
}