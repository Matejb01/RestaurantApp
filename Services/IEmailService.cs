using RestaurantApp.Models;

namespace RestaurantApp.Services
{
    public interface IEmailService
    {
        Task<bool> SendOrderConfirmationAsync(Narudzba narudzba);
    }
}
