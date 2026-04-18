using RestaurantApp.Data;
using RestaurantApp.Interfaces;
using RestaurantApp.Models;

namespace RestaurantApp.Presenters
{
    public class CartPresenter : BasePresenter<ICartView>
    {
        private readonly RestaurantDbContext _context;

        public CartPresenter(ICartView view, RestaurantDbContext context)
            : base(view)
        {
            _context = context;
        }

        public async Task<int> SubmitOrderAsync()
        {
            try
            {
                if (!View.CartItems.Any())
                {
                    View.ErrorMessage = "Košarica je prazna!";
                    return 0;
                }

                if (string.IsNullOrWhiteSpace(View.Ime))
                {
                    View.ErrorMessage = "Ime je obavezno!";
                    return 0;
                }

                if (View.NacinPreuzimanja == "Dostava" && string.IsNullOrWhiteSpace(View.Adresa))
                {
                    View.ErrorMessage = "Adresa je obavezna za dostavu!";
                    return 0;
                }

                if (string.IsNullOrWhiteSpace(View.Telefon))
                {
                    View.ErrorMessage = "Broj telefona je obavezan!";
                    return 0;
                }

                var narudzba = new Narudzba
                {
                    Ime = View.Ime,
                    Adresa = View.Adresa ?? string.Empty,
                    Telefon = View.Telefon,
                    Email = View.Email,
                    NacinPreuzimanja = View.NacinPreuzimanja,
                    Status = "Zaprimljeno",
                    DatumVrijeme = DateTime.Now,
                    UkupnaCijena = View.UkupnaCijena,
                    Stavke = View.CartItems.Select(c => new StavkaNarudzbe
                    {
                        JeloId = c.JeloId,
                        Kolicina = c.Kolicina,
                        CijenaStavke = c.Cijena
                    }).ToList()
                };

                _context.Narudzbe.Add(narudzba);
                await _context.SaveChangesAsync();

                View.SuccessMessage = "Narudžba uspješno poslana!";
                return narudzba.Id;
            }
            catch (Exception ex)
            {
                View.ErrorMessage = $"Greška pri slanju narudžbe: {ex.Message}";
                return 0;
            }
        }
    }
}