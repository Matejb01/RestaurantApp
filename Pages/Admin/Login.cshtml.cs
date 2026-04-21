using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;

namespace RestaurantApp.Pages.Admin
{
    public class LoginModel : PageModel
    {
        private readonly RestaurantDbContext _context;

        public LoginModel(RestaurantDbContext context)
        {
            _context = context;
        }

        [BindProperty] public string KorisnickoIme { get; set; } = string.Empty;
        [BindProperty] public string Lozinka { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var zaposlenik = await _context.Zaposlenici
                .FirstOrDefaultAsync(z => z.KorisnickoIme == KorisnickoIme
                                       && z.Lozinka == Lozinka
                                       && z.Uloga == "Administrator");

            if (zaposlenik == null)
            {
                ErrorMessage = "Pogrešno korisničko ime ili lozinka.";
                return Page();
            }

            HttpContext.Session.SetInt32("AdminId", zaposlenik.Id);
            HttpContext.Session.SetString("AdminIme", zaposlenik.Ime);

            return RedirectToPage("./MenuManagement");
        }
    }
}