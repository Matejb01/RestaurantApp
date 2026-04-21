using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Interfaces;
using RestaurantApp.Models;

namespace RestaurantApp.Presenters
{
    public class AdminMenuPresenter : BasePresenter<IAdminMenuView>
    {
        private readonly RestaurantDbContext _context;

        public AdminMenuPresenter(IAdminMenuView view, RestaurantDbContext context)
            : base(view)
        {
            _context = context;
        }

        public async Task LoadJelaAsync()
        {
            try
            {
                View.Jela = await _context.Jela
                    .OrderBy(j => j.Vrsta)
                    .ThenBy(j => j.Naziv)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                View.ErrorMessage = $"Greška: {ex.Message}";
            }
        }

        public async Task SaveJeloAsync(Jelo jelo)
        {
            try
            {
                if (jelo.Id == 0)
                    _context.Jela.Add(jelo);
                else
                    _context.Jela.Update(jelo);

                await _context.SaveChangesAsync();
                View.SuccessMessage = jelo.Id == 0
                    ? $"Jelo '{jelo.Naziv}' uspješno dodano!"
                    : $"Jelo '{jelo.Naziv}' uspješno ažurirano!";
            }
            catch (Exception ex)
            {
                View.ErrorMessage = $"Greška pri spremanju: {ex.Message}";
            }
        }

        public async Task DeleteJeloAsync(int id)
        {
            try
            {
                var jelo = await _context.Jela.FindAsync(id);
                if (jelo != null)
                {
                    _context.Jela.Remove(jelo);
                    await _context.SaveChangesAsync();
                    View.SuccessMessage = $"Jelo '{jelo.Naziv}' uspješno obrisano!";
                }
            }
            catch (Exception ex)
            {
                View.ErrorMessage = $"Greška pri brisanju: {ex.Message}";
            }
        }
    }
}