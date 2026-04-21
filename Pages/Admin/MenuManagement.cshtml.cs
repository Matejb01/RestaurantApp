using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantApp.Data;
using RestaurantApp.Interfaces;
using RestaurantApp.Models;
using RestaurantApp.Presenters;

namespace RestaurantApp.Pages.Admin
{
    public class MenuManagementModel : PageModel, IAdminMenuView
    {
        private readonly RestaurantDbContext _context;

        public MenuManagementModel(RestaurantDbContext context)
        {
            _context = context;
        }

        // IView
        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        // IAdminMenuView
        public List<Jelo> Jela { get; set; } = new();
        public Jelo? OdabranoJelo { get; set; }

        [BindProperty] public Jelo FormJelo { get; set; } = new();
        public string AdminIme { get; set; } = string.Empty;

        private bool IsAdminLoggedIn() =>
            HttpContext.Session.GetInt32("AdminId") != null;

        public async Task<IActionResult> OnGetAsync(int? editId)
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("./Login");

            AdminIme = HttpContext.Session.GetString("AdminIme") ?? "";
            var presenter = new AdminMenuPresenter(this, _context);
            await presenter.LoadJelaAsync();

            if (editId.HasValue)
                FormJelo = Jela.FirstOrDefault(j => j.Id == editId.Value) ?? new Jelo();

            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("./Login");

            var presenter = new AdminMenuPresenter(this, _context);
            await presenter.SaveJeloAsync(FormJelo);
            await presenter.LoadJelaAsync();

            AdminIme = HttpContext.Session.GetString("AdminIme") ?? "";
            FormJelo = new Jelo();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int jeloId)
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("./Login");

            var presenter = new AdminMenuPresenter(this, _context);
            await presenter.DeleteJeloAsync(jeloId);
            await presenter.LoadJelaAsync();

            AdminIme = HttpContext.Session.GetString("AdminIme") ?? "";
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