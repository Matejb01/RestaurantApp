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
        private readonly IWebHostEnvironment _env;
        private const long MaxImageBytes = 2 * 1024 * 1024;
        private static readonly string[] AllowedImageExt = { ".jpg", ".jpeg", ".png" };

        public MenuManagementModel(RestaurantDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

        public async Task<IActionResult> OnPostSaveAsync(IFormFile? slika, bool ukloniSliku = false)
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("./Login");

            var presenter = new AdminMenuPresenter(this, _context);

            if (ukloniSliku && !string.IsNullOrEmpty(FormJelo.SlikaUrl))
            {
                TryDeleteImage(FormJelo.SlikaUrl);
                FormJelo.SlikaUrl = null;
            }

            if (slika is { Length: > 0 })
            {
                var saved = await TrySaveImageAsync(slika);
                if (saved == null)
                {
                    await presenter.LoadJelaAsync();
                    AdminIme = HttpContext.Session.GetString("AdminIme") ?? "";
                    return Page();
                }

                if (!string.IsNullOrEmpty(FormJelo.SlikaUrl))
                    TryDeleteImage(FormJelo.SlikaUrl);

                FormJelo.SlikaUrl = saved;
            }

            await presenter.SaveJeloAsync(FormJelo);
            await presenter.LoadJelaAsync();

            AdminIme = HttpContext.Session.GetString("AdminIme") ?? "";
            FormJelo = new Jelo();
            return Page();
        }

        private async Task<string?> TrySaveImageAsync(IFormFile file)
        {
            if (file.Length > MaxImageBytes)
            {
                ErrorMessage = "Slika je prevelika (max 2 MB).";
                return null;
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedImageExt.Contains(ext))
            {
                ErrorMessage = "Dozvoljeni formati: JPG, PNG.";
                return null;
            }

            var folder = Path.Combine(_env.WebRootPath, "images", "jela");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            await using var fs = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(fs);

            return $"/images/jela/{fileName}";
        }

        private void TryDeleteImage(string relativeUrl)
        {
            try
            {
                var trimmed = relativeUrl.TrimStart('/');
                var fullPath = Path.Combine(_env.WebRootPath, trimmed.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }
            catch
            {
                // best-effort cleanup; ignore failures
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int jeloId)
        {
            if (!IsAdminLoggedIn())
                return RedirectToPage("./Login");

            var existing = await _context.Jela.FindAsync(jeloId);
            var imageToDelete = existing?.SlikaUrl;

            var presenter = new AdminMenuPresenter(this, _context);
            await presenter.DeleteJeloAsync(jeloId);
            await presenter.LoadJelaAsync();

            if (!string.IsNullOrEmpty(imageToDelete))
                TryDeleteImage(imageToDelete);

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