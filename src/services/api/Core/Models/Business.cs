﻿namespace api.Core.Models;

public class Service
{
    public string Name  { get; private set; }
    public Money  Price { get; private set; }
}

public abstract class Business : Entity
{
    public string        Name            { get; set; }
    public string        Description     { get; set; }
    public Address       Address         { get; set; }
    public Money         PricePerUnit    { get; set; }
    public User          Owner           { get; private set; }
    public List<string>  Images          { get; private set; } = new();
    public List<Rule>    Rules           { get; private set; } = new();
    public List<Service> Services        { get; private set; } = new();
    public List<Slot>    Availability    { get; private set; } = new();
    public List<Review>  Reviews         { get; private set; } = new();
    public List<User>    Subscribers     { get; private set; } = new();
    public int           NumberOfReviews { get; private set; }
    public int           Rating          { get; private set; }
    public virtual       TimeSpan Unit   { get; }

    public Business() {}

    public Business(
        string name,
        string description,
        Address address,
        Money pricePerUnit,
        User owner,
        List<string> images,
        List<Rule> rules,
        List<Service> services
    )
    {
        Name = name;
        Description = description;
        Address = address;
        PricePerUnit = pricePerUnit;
        Owner = owner;
        Images = images;
        Rules = rules;
        Services = services;
    }

    public bool IsAvailable(DateTime start, DateTime end)
    {
        return Availability.Exists(s => s.Contains(start, end) && s.Available) &&
              !Availability.Exists(s => s.Intersects(start, end) && !s.Available);
    }

    public void Review(User user, double rating, string content)
    {
        Reviews.Add(new Review
        {
            User = user,
            Business = this, 
            Rating = rating, 
            Content = content 
        });
        Rating = (NumberOfReviews * Rating + Rating) / ++NumberOfReviews;
    }

    private int GetTotalUnits(DateTime start, DateTime end)
    {
        double totalUnits = 1;
        if (Unit.Hours == 1)
        {
            totalUnits = (end - start).TotalHours;
        }
        else if (Unit.Hours == 24)
        {
            totalUnits = (end - start).TotalDays;
        }
        return (int)Math.Round(totalUnits);
    }

    public Money Price
    (
        DateTime start,
        DateTime end,
        int people,
        double discountPercentage,
        List<Service> chosenServices
    )
    {
        decimal discount = 1 - Convert.ToDecimal(discountPercentage / 100);
        decimal amount = GetTotalUnits(start, end) *
                         people                    *
                         PricePerUnit.Amount       +
                         chosenServices.Sum(s => s.Price.Amount);

        return new(amount * discount, PricePerUnit.Currency);
    }
}