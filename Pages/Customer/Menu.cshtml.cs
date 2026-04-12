using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantApp.Data;
using RestaurantApp.Interfaces;
using RestaurantApp.Models;
using RestaurantApp.Presenters;
using System.Text.Json;

namespace RestaurantApp.Pages.Customer
{
    public class MenuModel : PageModel, IMenuView
    {
        private readonly RestaurantDbContext _context;

        public MenuModel(RestaurantDbContext context)
        {
            _context = context;
        }

        // IView
        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        // IMenuView
        public List<Jelo> Jela { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string FilterVrsta { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var presenter = new MenuPresenter(this, _context);
            await presenter.LoadMenuAsync();
        }

        public IActionResult OnPostAddToCart(int jeloId, string naziv, decimal cijena)
        {
            // Get existing cart from session
            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = cartJson != null
                ? JsonSerializer.Deserialize<List<CartItem>>(cartJson)!
                : new List<CartItem>();

            // Check if item already in cart
            var existing = cart.FirstOrDefault(c => c.JeloId == jeloId);
            if (existing != null)
                existing.Kolicina++;
            else
                cart.Add(new CartItem { JeloId = jeloId, Naziv = naziv, Cijena = cijena, Kolicina = 1 });

            // Save back to session
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));

            SuccessMessage = $"{naziv} dodano u košaricu!";

            // Reload menu
            var presenter = new MenuPresenter(this, _context);
            presenter.LoadMenuAsync().Wait();

            return Page();
        }
    }
}