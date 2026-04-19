using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Interfaces;

namespace RestaurantApp.Presenters
{
    public class EmployeeOrdersPresenter : BasePresenter<IEmployeeOrdersView>
    {
        private readonly RestaurantDbContext _context;

        public EmployeeOrdersPresenter(IEmployeeOrdersView view, RestaurantDbContext context)
            : base(view)
        {
            _context = context;
        }

        public async Task LoadOrdersAsync()
        {
            try
            {
                var query = _context.Narudzbe
                    .Include(n => n.Stavke)
                    .ThenInclude(s => s.Jelo)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(View.FilterStatus))
                    query = query.Where(n => n.Status == View.FilterStatus);
                else
                    query = query.Where(n => n.Status != "Dostavljeno" && n.Status != "Odbijeno");

                View.Narudzbe = await query
                    .OrderByDescending(n => n.DatumVrijeme)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                View.ErrorMessage = $"Greška: {ex.Message}";
            }
        }
    }
}