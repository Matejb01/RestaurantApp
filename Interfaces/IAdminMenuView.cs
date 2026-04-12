using RestaurantApp.Models;

namespace RestaurantApp.Interfaces
{
    public interface IAdminMenuView : IView
    {
        List<Jelo> Jela { get; set; }
        Jelo? OdabranoJelo { get; set; }
    }
}