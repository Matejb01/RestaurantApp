using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Interfaces;
using RestaurantApp.Models;
using RestaurantApp.Presenters;
using RestaurantApp.Services;

namespace RestaurantApp.Pages.Employee
{
    public class OrdersModel : PageModel, IEmployeeOrdersView
    {
        private readonly RestaurantDbContext _context;
        private readonly IEmailService _emailService;

        public OrdersModel(RestaurantDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // IView
        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        // IEmployeeOrdersView
        public List<Narudzba> Narudzbe { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string FilterStatus { get; set; } = string.Empty;

        public string ZaposlenikIme { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Session.GetInt32("ZaposlenikId") == null)
                return RedirectToPage("./Login");

            ZaposlenikIme = HttpContext.Session.GetString("ZaposlenikIme") ?? "";

            var presenter = new EmployeeOrdersPresenter(this, _context);
            await presenter.LoadOrdersAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostConfirmAsync(int narudzbaId, int vrijemeDostave)
        {
            if (HttpContext.Session.GetInt32("ZaposlenikId") == null)
                return RedirectToPage("./Login");

            var narudzba = await _context.Narudzbe
                .Include(n => n.Stavke)
                    .ThenInclude(s => s.Jelo)
                .FirstOrDefaultAsync(n => n.Id == narudzbaId);

            if (narudzba != null)
            {
                narudzba.Status = "U pripremi";
                narudzba.PredvijenoVrijemeDostave = vrijemeDostave;
                narudzba.ZaposlenikId = HttpContext.Session.GetInt32("ZaposlenikId");
                await _context.SaveChangesAsync();
                SuccessMessage = $"Narudžba #{narudzbaId} potvrđena!";

                if (!string.IsNullOrWhiteSpace(narudzba.Email))
                {
                    var sent = await _emailService.SendOrderConfirmationAsync(narudzba);
                    if (sent)
                        SuccessMessage += $" Potvrda poslana na {narudzba.Email}.";
                }
            }

            var presenter = new EmployeeOrdersPresenter(this, _context);
            await presenter.LoadOrdersAsync();
            ZaposlenikIme = HttpContext.Session.GetString("ZaposlenikIme") ?? "";
            return Page();
        }

        public async Task<IActionResult> OnPostRejectAsync(int narudzbaId, string razlog)
        {
            if (HttpContext.Session.GetInt32("ZaposlenikId") == null)
                return RedirectToPage("./Login");

            var narudzba = await _context.Narudzbe.FindAsync(narudzbaId);
            if (narudzba != null)
            {
                narudzba.Status = "Odbijeno";
                narudzba.RazlogOdbijanja = razlog;
                narudzba.ZaposlenikId = HttpContext.Session.GetInt32("ZaposlenikId");
                await _context.SaveChangesAsync();
                SuccessMessage = $"Narudžba #{narudzbaId} odbijena.";
            }

            var presenter = new EmployeeOrdersPresenter(this, _context);
            await presenter.LoadOrdersAsync();
            ZaposlenikIme = HttpContext.Session.GetString("ZaposlenikIme") ?? "";
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int narudzbaId, string noviStatus)
        {
            if (HttpContext.Session.GetInt32("ZaposlenikId") == null)
                return RedirectToPage("./Login");

            var narudzba = await _context.Narudzbe.FindAsync(narudzbaId);
            if (narudzba != null)
            {
                narudzba.Status = noviStatus;
                await _context.SaveChangesAsync();
                SuccessMessage = $"Status narudžbe #{narudzbaId} ažuriran na '{noviStatus}'.";
            }

            var presenter = new EmployeeOrdersPresenter(this, _context);
            await presenter.LoadOrdersAsync();
            ZaposlenikIme = HttpContext.Session.GetString("ZaposlenikIme") ?? "";
            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("./Login");
        }
    }
}