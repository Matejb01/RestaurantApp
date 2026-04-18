using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantApp.Data;
using RestaurantApp.Interfaces;
using RestaurantApp.Models;
using RestaurantApp.Presenters;

namespace RestaurantApp.Pages.Customer
{
    public class OrderStatusModel : PageModel, IOrderStatusView
    {
        private readonly RestaurantDbContext _context;

        public OrderStatusModel(RestaurantDbContext context)
        {
            _context = context;
        }

        // IView
        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        // IOrderStatusView
        public Narudzba? Narudzba { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SearchId { get; set; }

        public async Task OnGetAsync()
        {
            if (SearchId > 0)
            {
                var presenter = new OrderStatusPresenter(this, _context);
                await presenter.LoadOrderAsync();
            }
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            var presenter = new OrderStatusPresenter(this, _context);
            await presenter.LoadOrderAsync();
            return Page();
        }
    }
}