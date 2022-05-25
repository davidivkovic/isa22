using API.Core.Model;
using Scriban;
using System.Globalization;

namespace API.Services.Email.Messages;

public class ReservationCreated : IMessage
{
    public string TemplateName => "reservation-created.html";
    public string Subject => "New reservation on adventure.com";

    private readonly Reservation reservation;
    private readonly string contactUrl;
    private readonly string businessImageUrl;

    public ReservationCreated(Reservation reservation, string businessImageUrl, string contactUrl)
    {
        this.reservation = reservation;
        this.contactUrl = contactUrl;
        this.businessImageUrl = businessImageUrl;
    }

    public decimal CalculateBase(Reservation reservation)
    {
        return reservation.Business.PricePerUnit.Amount /
               (1 + (decimal)reservation.Payment.TaxPercentage / 100) *
                reservation.Business.People *
                reservation.Units;
    }

    public decimal CalculateServices(Reservation reservation)
    {
        return reservation.Services.Sum(s => s.Price.Amount) /
               (1 + (decimal)reservation.Payment.TaxPercentage / 100);
    }

    public decimal CalculateSubtotal(Reservation reservation)
    {
        return CalculateBase(reservation) + CalculateServices(reservation);
    }

    public object Costs(Reservation reservation) 
    {
        var symbol = reservation.Payment.Price.Symbol;
        var total = reservation.Payment.Price.Amount;
        var tax = (decimal)(1 + reservation.Payment.TaxPercentage / 100);
        var culture = CultureInfo.InvariantCulture;

        return new
        {
            BaseCost = symbol + CalculateBase(reservation).ToString("F", culture),
            BaseDetails = $"{reservation.Business.People} x {reservation.Units} x {symbol}{reservation.Business.PricePerUnit.Amount.ToString("F", culture)}",
            ServicesTotal = symbol + CalculateServices(reservation).ToString("F", culture),
            Services = reservation.Services.Select(s => new
            {
                Name = s.Name,
                Price = (s.Price.Amount / tax).ToString("F", culture)
            }),
            Subtotal = symbol + CalculateSubtotal(reservation).ToString("F", culture),
            Discount = (CalculateSubtotal(reservation) * (decimal)(reservation.Payment.DiscountPercentage / 100)).ToString("F", culture),
            Tax = (total - total / tax).ToString("F", culture),
            Total = symbol + total.ToString("F", culture)
        };
    }

    public string Render(Template messageTemplate)
    {
        return messageTemplate.Render(new
        {
            Cost = Costs(reservation),
            reservation,
            businessImageUrl,
            contactUrl
        });
    }
}