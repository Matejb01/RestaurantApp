using RestaurantApp.Models;

namespace RestaurantApp.Interfaces
{
    public interface IEmployeesView : IView
    {
        List<Zaposlenik> Zaposlenici { get; set; }
        Zaposlenik? OdabraniZaposlenik { get; set; }
    }
}