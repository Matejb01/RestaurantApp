using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using RestaurantApp.Models;

namespace RestaurantApp.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _email;
        private readonly RestaurantInfo _restaurant;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(
            IOptions<EmailSettings> email,
            IOptions<RestaurantInfo> restaurant,
            ILogger<SmtpEmailService> logger)
        {
            _email = email.Value;
            _restaurant = restaurant.Value;
            _logger = logger;
        }

        public async Task<bool> SendOrderConfirmationAsync(Narudzba narudzba)
        {
            if (string.IsNullOrWhiteSpace(narudzba.Email))
                return false;

            if (string.IsNullOrWhiteSpace(_email.SmtpHost) ||
                string.IsNullOrWhiteSpace(_email.SenderEmail))
            {
                _logger.LogWarning(
                    "Email not sent for order #{Id}: SMTP host or sender email is not configured in appsettings.",
                    narudzba.Id);
                return false;
            }

            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_email.SenderEmail, _email.SenderName),
                    Subject = $"Potvrda narudžbe #{narudzba.Id} - {_restaurant.Naziv}",
                    Body = BuildBody(narudzba),
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8,
                };
                message.To.Add(narudzba.Email);

                using var client = new SmtpClient(_email.SmtpHost, _email.SmtpPort)
                {
                    EnableSsl = _email.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_email.Username, _email.Password),
                };

                await client.SendMailAsync(message);
                _logger.LogInformation("Receipt email sent to {Email} for order #{Id}.",
                    narudzba.Email, narudzba.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send receipt for order #{Id}.", narudzba.Id);
                return false;
            }
        }

        private string BuildBody(Narudzba n)
        {
            var sb = new StringBuilder();
            sb.Append("<div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;'>");
            sb.Append($"<h2>Hvala na narudžbi, {WebUtility.HtmlEncode(n.Ime)}!</h2>");
            sb.Append($"<p>Vaša narudžba <strong>#{n.Id}</strong> je potvrđena ");
            sb.Append($"({n.DatumVrijeme:dd.MM.yyyy HH:mm}).</p>");

            if (n.PredvijenoVrijemeDostave.HasValue)
            {
                sb.Append("<p><strong>Predviđeno vrijeme: </strong>");
                sb.Append($"{n.PredvijenoVrijemeDostave} minuta</p>");
            }

            sb.Append("<h3>Stavke</h3>");
            sb.Append("<table style='width:100%;border-collapse:collapse;'>");
            sb.Append("<thead><tr style='background:#f0f0f0;'>");
            sb.Append("<th style='text-align:left;padding:8px;border-bottom:1px solid #ddd;'>Jelo</th>");
            sb.Append("<th style='text-align:center;padding:8px;border-bottom:1px solid #ddd;'>Kol.</th>");
            sb.Append("<th style='text-align:right;padding:8px;border-bottom:1px solid #ddd;'>Cijena</th>");
            sb.Append("<th style='text-align:right;padding:8px;border-bottom:1px solid #ddd;'>Ukupno</th>");
            sb.Append("</tr></thead><tbody>");

            foreach (var s in n.Stavke)
            {
                var naziv = s.Jelo?.Naziv ?? "(nepoznato)";
                var ukupno = s.Kolicina * s.CijenaStavke;
                sb.Append("<tr>");
                sb.Append($"<td style='padding:8px;border-bottom:1px solid #eee;'>{WebUtility.HtmlEncode(naziv)}</td>");
                sb.Append($"<td style='padding:8px;border-bottom:1px solid #eee;text-align:center;'>{s.Kolicina}</td>");
                sb.Append($"<td style='padding:8px;border-bottom:1px solid #eee;text-align:right;'>{s.CijenaStavke:F2} €</td>");
                sb.Append($"<td style='padding:8px;border-bottom:1px solid #eee;text-align:right;'>{ukupno:F2} €</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody><tfoot><tr>");
            sb.Append("<td colspan='3' style='padding:8px;text-align:right;font-weight:bold;'>Ukupno:</td>");
            sb.Append($"<td style='padding:8px;text-align:right;font-weight:bold;'>{n.UkupnaCijena:F2} €</td>");
            sb.Append("</tr></tfoot></table>");

            sb.Append("<h3>Podaci o dostavi</h3>");
            sb.Append($"<p><strong>Način:</strong> {WebUtility.HtmlEncode(n.NacinPreuzimanja)}<br>");
            if (!string.IsNullOrEmpty(n.Adresa))
                sb.Append($"<strong>Adresa:</strong> {WebUtility.HtmlEncode(n.Adresa)}<br>");
            sb.Append($"<strong>Telefon:</strong> {WebUtility.HtmlEncode(n.Telefon)}</p>");

            sb.Append("<hr style='border:0;border-top:1px solid #ddd;margin:24px 0;'>");
            sb.Append($"<p style='color:#666;font-size:14px;'><strong>{WebUtility.HtmlEncode(_restaurant.Naziv)}</strong>");
            if (!string.IsNullOrEmpty(_restaurant.Adresa))
                sb.Append($"<br>{WebUtility.HtmlEncode(_restaurant.Adresa)}");
            if (!string.IsNullOrEmpty(_restaurant.Telefon))
                sb.Append($"<br>Tel: {WebUtility.HtmlEncode(_restaurant.Telefon)}");
            if (!string.IsNullOrEmpty(_restaurant.Email))
                sb.Append($"<br>{WebUtility.HtmlEncode(_restaurant.Email)}");
            sb.Append("</p></div>");

            return sb.ToString();
        }
    }
}
