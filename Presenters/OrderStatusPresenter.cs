using RestaurantApp.Data;
using RestaurantApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApp.Presenters
{
    public class OrderStatusPresenter : BasePresenter<IOrderStatusView>
    {
        private readonly RestaurantDbContext _context;

        public OrderStatusPresenter(IOrderStatusView view, RestaurantDbContext context)
            : base(view)
        {
            _context = context;
        }

        public async Task LoadOrderAsync()
        {
            try
            {
                if (View.SearchId <= 0)
                {
                    View.ErrorMessage = "Unesite ispravan broj narudžbe.";
                    return;
                }

                View.Narudzba = await _context.Narudzbe
                    .Include(n => n.Stavke)
                    .ThenInclude(s => s.Jelo)
                    .FirstOrDefaultAsync(n => n.Id == View.SearchId);

                if (View.Narudzba == null)
                    View.ErrorMessage = $"Narudžba #{View.SearchId} nije pronađena.";
            }
            catch (Exception ex)
            {
                View.ErrorMessage = $"Greška: {ex.Message}";
            }
        }
    }
}