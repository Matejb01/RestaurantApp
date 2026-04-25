using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Interfaces;
using RestaurantApp.Models;

namespace RestaurantApp.Presenters
{
    public class EmployeesPresenter : BasePresenter<IEmployeesView>
    {
        private readonly RestaurantDbContext _context;

        public EmployeesPresenter(IEmployeesView view, RestaurantDbContext context)
            : base(view)
        {
            _context = context;
        }

        public async Task LoadZaposleniciAsync()
        {
            try
            {
                View.Zaposlenici = await _context.Zaposlenici
                    .Where(z => z.Aktivan)
                    .OrderBy(z => z.Uloga)
                    .ThenBy(z => z.Prezime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                View.ErrorMessage = $"Greška: {ex.Message}";
            }
        }

        public async Task SaveZaposlenikAsync(Zaposlenik zaposlenik)
        {
            try
            {
                var exists = await _context.Zaposlenici
                    .AnyAsync(z => z.KorisnickoIme == zaposlenik.KorisnickoIme
                                && z.Id != zaposlenik.Id);

                if (exists)
                {
                    View.ErrorMessage = $"Korisničko ime '{zaposlenik.KorisnickoIme}' već postoji!";
                    return;
                }

                if (zaposlenik.Id == 0)
                {
                    _context.Zaposlenici.Add(zaposlenik);
                }
                else
                {
                    // If password left empty, keep the existing one
                    if (string.IsNullOrWhiteSpace(zaposlenik.Lozinka))
                    {
                        var existing = await _context.Zaposlenici.FindAsync(zaposlenik.Id);
                        if (existing != null)
                            zaposlenik.Lozinka = existing.Lozinka;
                        _context.Entry(existing!).CurrentValues.SetValues(zaposlenik);
                    }
                    else
                    {
                        _context.Zaposlenici.Update(zaposlenik);
                    }
                }

                await _context.SaveChangesAsync();
                View.SuccessMessage = zaposlenik.Id == 0
                    ? $"Zaposlenik '{zaposlenik.Ime} {zaposlenik.Prezime}' uspješno dodan!"
                    : $"Zaposlenik '{zaposlenik.Ime} {zaposlenik.Prezime}' uspješno ažuriran!";
            }
            catch (Exception ex)
            {
                View.ErrorMessage = $"Greška pri spremanju: {ex.Message}";
            }
        }

        public async Task DeleteZaposlenikAsync(int id)
        {
            try
            {
                var zaposlenik = await _context.Zaposlenici.FindAsync(id);
                if (zaposlenik == null) return;

                // Soft delete - just deactivate
                zaposlenik.Aktivan = false;
                await _context.SaveChangesAsync();
                View.SuccessMessage = $"Zaposlenik '{zaposlenik.Ime} {zaposlenik.Prezime}' deaktiviran.";
            }
            catch (Exception ex)
            {
                View.ErrorMessage = $"Greška pri brisanju: {ex.Message}";
            }
        }
    }
}