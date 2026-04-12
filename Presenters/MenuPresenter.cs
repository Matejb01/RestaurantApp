using RestaurantApp.Data;
using RestaurantApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApp.Presenters
{
    public class MenuPresenter : BasePresenter<IMenuView>
    {
        private readonly RestaurantDbContext _context;

        public MenuPresenter(IMenuView view, RestaurantDbContext context)
            : base(view)
        {
            _context = context;
        }

        public async Task LoadMenuAsync()
        {
            try
            {
                var query = _context.Jela.Where(j => j.Dostupno);

                if (!string.IsNullOrEmpty(View.FilterVrsta))
                    query = query.Where(j => j.Vrsta == View.FilterVrsta);

                View.Jela = await query.OrderBy(j => j.Vrsta).ThenBy(j => j.Naziv).ToListAsync();
            }
            catch (Exception ex)
            {
                View.ErrorMessage = $"Greška pri učitavanju menija: {ex.Message}";
            }
        }
    }
}