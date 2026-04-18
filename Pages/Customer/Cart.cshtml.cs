using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantApp.Data;
using RestaurantApp.Interfaces;
using RestaurantApp.Models;
using RestaurantApp.Presenters;
using System.Text.Json;

namespace RestaurantApp.Pages.Customer
{
    public class CartModel : PageModel, ICartView
    {
        private readonly RestaurantDbContext _context;

        public CartModel(RestaurantDbContext context)
        {
            _context = context;
        }

        // IView
        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        // ICartView
        public List<CartItem> CartItems { get; set; } = new();
        public decimal UkupnaCijena { get; set; }

        [BindProperty] public string Ime { get; set; } = string.Empty;
        [BindProperty] public string Adresa { get; set; } = string.Empty;
        [BindProperty] public string Telefon { get; set; } = string.Empty;
        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string NacinPreuzimanja { get; set; } = "Dostava";

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            return cartJson != null
                ? JsonSerializer.Deserialize<List<CartItem>>(cartJson)!
                : new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }

        public void OnGet()
        {
            CartItems = GetCart();
            UkupnaCijena = CartItems.Sum(c => c.Ukupno);
        }

        public IActionResult OnPostRemove(int jeloId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.JeloId == jeloId);
            SaveCart(cart);
            return RedirectToPage();
        }

        public IActionResult OnPostUpdateQuantity(int jeloId, int kolicina)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.JeloId == jeloId);
            if (item != null)
            {
                if (kolicina <= 0)
                    cart.Remove(item);
                else
                    item.Kolicina = kolicina;
            }
            SaveCart(cart);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSubmitOrderAsync()
        {
            CartItems = GetCart();
            UkupnaCijena = CartItems.Sum(c => c.Ukupno);

            var presenter = new CartPresenter(this, _context);
            var narudzbaId = await presenter.SubmitOrderAsync();

            if (narudzbaId > 0)
            {
                // Clear cart after successful order
                SaveCart(new List<CartItem>());
                return RedirectToPage("./OrderStatus", new { searchId = narudzbaId });
            }

            return Page();
        }
    }
}