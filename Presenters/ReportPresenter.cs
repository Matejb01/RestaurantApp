using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Models;

namespace RestaurantApp.Presenters
{
    public class ReportPresenter
    {
        private readonly RestaurantDbContext _context;

        public ReportPresenter(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<List<Narudzba>> GetAktivneNarudzbeAsync()
        {
            return await _context.Narudzbe
                .Include(n => n.Stavke)
                .ThenInclude(s => s.Jelo)
                .Where(n => n.Status != "Dostavljeno" && n.Status != "Odbijeno")
                .OrderBy(n => n.DatumVrijeme)
                .ToListAsync();
        }

        public async Task<List<DnevniPrometItem>> GetDnevniPrometAsync(DateTime od, DateTime do_)
        {
            var narudzbe = await _context.Narudzbe
                .Where(n => n.DatumVrijeme.Date >= od.Date
                         && n.DatumVrijeme.Date <= do_.Date
                         && n.Status != "Odbijeno")
                .ToListAsync();

            return narudzbe
                .GroupBy(n => n.DatumVrijeme.Date)
                .Select(g => new DnevniPrometItem
                {
                    Datum = g.Key,
                    BrojNarudzbi = g.Count(),
                    UkupniIznos = g.Sum(n => n.UkupnaCijena)
                })
                .OrderBy(x => x.Datum)
                .ToList();
        }

        public async Task<List<PopularnoJeloItem>> GetNajcesceNarucenaJelaAsync(DateTime od, DateTime do_)
        {
            var stavke = await _context.StavkeNarudzbe
                .Include(s => s.Jelo)
                .Include(s => s.Narudzba)
                .Where(s => s.Narudzba.DatumVrijeme.Date >= od.Date
                         && s.Narudzba.DatumVrijeme.Date <= do_.Date
                         && s.Narudzba.Status != "Odbijeno")
                .ToListAsync();

            return stavke
                .GroupBy(s => new { s.JeloId, s.Jelo.Naziv })
                .Select(g => new PopularnoJeloItem
                {
                    Naziv = g.Key.Naziv,
                    UkupnaKolicina = g.Sum(s => s.Kolicina),
                    BrojNarudzbi = g.Count()
                })
                .OrderByDescending(x => x.UkupnaKolicina)
                .ToList();
        }

        public async Task<VrijemeDostaveReport> GetProsjecnoVrijemeDostaveAsync(DateTime od, DateTime do_)
        {
            var narudzbe = await _context.Narudzbe
                .Where(n => n.DatumVrijeme.Date >= od.Date
                         && n.DatumVrijeme.Date <= do_.Date
                         && n.Status == "Dostavljeno"
                         && n.PredvijenoVrijemeDostave.HasValue)
                .ToListAsync();

            if (!narudzbe.Any())
                return new VrijemeDostaveReport();

            return new VrijemeDostaveReport
            {
                BrojDostava = narudzbe.Count,
                ProsjecnoVrijeme = narudzbe.Average(n => n.PredvijenoVrijemeDostave!.Value),
                MinVrijeme = narudzbe.Min(n => n.PredvijenoVrijemeDostave!.Value),
                MaxVrijeme = narudzbe.Max(n => n.PredvijenoVrijemeDostave!.Value)
            };
        }
    }

    // Helper models for reports
    public class DnevniPrometItem
    {
        public DateTime Datum { get; set; }
        public int BrojNarudzbi { get; set; }
        public decimal UkupniIznos { get; set; }
    }

    public class PopularnoJeloItem
    {
        public string Naziv { get; set; } = string.Empty;
        public int UkupnaKolicina { get; set; }
        public int BrojNarudzbi { get; set; }
    }

    public class VrijemeDostaveReport
    {
        public int BrojDostava { get; set; }
        public double ProsjecnoVrijeme { get; set; }
        public int MinVrijeme { get; set; }
        public int MaxVrijeme { get; set; }
    }
}