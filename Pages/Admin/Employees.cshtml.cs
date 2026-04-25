using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantApp.Data;
using RestaurantApp.Interfaces;
using RestaurantApp.Models;
using RestaurantApp.Presenters;

namespace RestaurantApp.Pages.Admin
{
    public class EmployeesModel : PageModel, IEmployeesView
    {
        private readonly RestaurantDbContext _context;

        public EmployeesModel(RestaurantDbContext context)
        {
            _context = context;
        }

        // IView
        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        // IEmployeesView
        public List<Zaposlenik> Zaposlenici { get; set; } = new();
        public Zaposlenik? OdabraniZaposlenik { get; set; }

        [BindProperty] public Zaposlenik FormZaposlenik { get; set; } = new();
        public string AdminIme { get; set; } = string.Empty;
        public int CurrentAdminId { get; set; }

        private bool IsAdminLoggedIn() =>
            HttpContext.Session.GetInt32("AdminId") != null;

        public async Task<IActionResult> OnGetAsync(int? editId)
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("./Login");

            AdminIme = HttpContext.Session.GetString("AdminIme") ?? "";
            CurrentAdminId = HttpContext.Session.GetInt32("AdminId") ?? 0;

            var presenter = new EmployeesPresenter(this, _context);
            await presenter.LoadZaposleniciAsync();

            if (editId.HasValue)
            {
                FormZaposlenik = Zaposlenici
                    .FirstOrDefault(z => z.Id == editId.Value) ?? new Zaposlenik();
                // Clear password for security - admin must re-enter
                FormZaposlenik.Lozinka = string.Empty;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("./Login");

            var presenter = new EmployeesPresenter(this, _context);
            await presenter.SaveZaposlenikAsync(FormZaposlenik);
            await presenter.LoadZaposleniciAsync();

            AdminIme = HttpContext.Session.GetString("AdminIme") ?? "";
            CurrentAdminId = HttpContext.Session.GetInt32("AdminId") ?? 0;

            if (string.IsNullOrEmpty(ErrorMessage))
                FormZaposlenik = new Zaposlenik();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int zaposlenikId)
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("./Login");

            // Prevent admin from deleting themselves
            var currentId = HttpContext.Session.GetInt32("AdminId");
            if (currentId == zaposlenikId)
            {
                ErrorMessage = "Ne možete obrisati vlastiti račun!";
                var p = new EmployeesPresenter(this, _context);
                await p.LoadZaposleniciAsync();
                AdminIme = HttpContext.Session.GetString("AdminIme") ?? "";
                CurrentAdminId = currentId ?? 0;
                return Page();
            }

            var presenter = new EmployeesPresenter(this, _context);
            await presenter.DeleteZaposlenikAsync(zaposlenikId);
            await presenter.LoadZaposleniciAsync();

            AdminIme = HttpContext.Session.GetString("AdminIme") ?? "";
            CurrentAdminId = HttpContext.Session.GetInt32("AdminId") ?? 0;
            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Remove("AdminId");
            HttpContext.Session.Remove("AdminIme");
            return RedirectToPage("./Login");
        }
    }
}