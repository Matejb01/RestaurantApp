using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantApp.Data;
using RestaurantApp.Models;
using RestaurantApp.Presenters;

namespace RestaurantApp.Pages.Reports
{
    public class ReportsModel : PageModel
    {
        private readonly RestaurantDbContext _context;

        public ReportsModel(RestaurantDbContext context)
        {
            _context = context;
        }

        // Shared
        public string ErrorMessage { get; set; } = string.Empty;
        public string AdminIme { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public DateTime DatumOd { get; set; } = DateTime.Today.AddDays(-7);

        [BindProperty(SupportsGet = true)]
        public DateTime DatumDo { get; set; } = DateTime.Today;

        [BindProperty(SupportsGet = true)]
        public string ActiveReport { get; set; } = "aktivne";

        // Report data
        public List<Narudzba> AktivneNarudzbe { get; set; } = new();
        public List<DnevniPrometItem> DnevniPromet { get; set; } = new();
        public List<PopularnoJeloItem> PopularnaJela { get; set; } = new();
        public VrijemeDostaveReport VrijemeDostave { get; set; } = new();

        private bool IsAdminLoggedIn() =>
            HttpContext.Session.GetInt32("AdminId") != null;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("/Admin/Login");

            AdminIme = HttpContext.Session.GetString("AdminIme") ?? "";
            await LoadReportAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("/Admin/Login");

            AdminIme = HttpContext.Session.GetString("AdminIme") ?? "";
            await LoadReportAsync();
            return Page();
        }

        private async Task LoadReportAsync()
        {
            var presenter = new ReportPresenter(_context);

            switch (ActiveReport)
            {
                case "aktivne":
                    AktivneNarudzbe = await presenter.GetAktivneNarudzbeAsync();
                    break;
                case "promet":
                    DnevniPromet = await presenter.GetDnevniPrometAsync(DatumOd, DatumDo);
                    break;
                case "popularna":
                    PopularnaJela = await presenter.GetNajcesceNarucenaJelaAsync(DatumOd, DatumDo);
                    break;
                case "dostava":
                    VrijemeDostave = await presenter.GetProsjecnoVrijemeDostaveAsync(DatumOd, DatumDo);
                    break;
            }
        }
    }
}